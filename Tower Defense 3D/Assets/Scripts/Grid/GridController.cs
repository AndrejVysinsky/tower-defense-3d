using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour, IBuildOptionClicked, IMapLoaded, IMapSaved
{
    [SerializeField] GridSettings gridSettings;
    [SerializeField] GridDisplay gridDisplay;
    [SerializeField] PlacementValidator placementValidator;
    [SerializeField] BrushObjectsHolder brushObjectsHolder;
    
    [SerializeField] MapSaveManager map;

    private int _objectElevation;
    private float _objectOriginY;

    private GameObject _objectToPlacePrefab;

    public GridSettings GridSettings => gridSettings;

    public static bool IsBuildingModeActive { get; private set; } = false;

    public UnityEvent<int> OnBrushSizeChangedEvent;

    private void Awake()
    {
        placementValidator = Instantiate(placementValidator);
        brushObjectsHolder = placementValidator.GetComponent<BrushObjectsHolder>();
        placementValidator.GridSettings = GridSettings;
                
        gridDisplay.CalculateGrid(gridSettings.sizeX, gridSettings.sizeZ, gridSettings.cellSize);
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public void SetGridDimensions(int sizeX, int sizeZ, bool multiplyByCellSize)
    {
        gridSettings.sizeX = sizeX;
        gridSettings.sizeZ = sizeZ;

        if (multiplyByCellSize)
        {
            gridSettings.sizeX *= (int)gridSettings.cellSize;
            gridSettings.sizeZ *= (int)gridSettings.cellSize;
        }

        gridDisplay.CalculateGrid(gridSettings.sizeX, gridSettings.sizeZ, gridSettings.cellSize);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl))
        {
            ChangeElevation();
        }

        if (brushObjectsHolder.IsHoldingObjects)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeRotation();
            }

            MoveObject();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }

            if (Input.GetMouseButtonDown(1))
            {
                DestroyBrushObjects();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1) && gridSettings.editorOnlyDestruction)
            {
                DestroyClickedObject();
            }
        }
    }

    private void ChangeRotation()
    {
        placementValidator.Rotate(Vector3.up, 90f);
    }

    private void ChangeElevation()
    {
        if (gridSettings.autoHeight)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0)
        {
            gridDisplay.DecreaseElevation();
        }

        if (scroll > 0)
        {
            gridDisplay.IncreaseElevation();
        }
    }

    private void MoveObject()
    {
        if (gridSettings.autoHeight)
        {
            if (RayCaster.RayCastGameObject(out RaycastHit terrainHitInfo, "Terrain"))
            {
                SetObjectPosition(terrainHitInfo.point);
            }
            else if (RayCaster.RayCastGameObject(out RaycastHit gridHitInfo, "Grid"))
            {
                SetObjectPosition(gridHitInfo.point);
            }
        }
        else
        {
            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo, "Grid"))
            {
                SetObjectPosition(hitInfo.point);
            }
        }
    }

    private void SetObjectPosition(Vector3 position)
    {
        if (gridSettings.autoHeight)
        {
            _objectElevation = Mathf.RoundToInt(position.y);
        }
        else
        {
            _objectElevation = gridDisplay.GetGridElevation();
        }

        if (gridSettings.snapToGrid)
        {
            position = GetNearestPointOnGrid(position);
        }

        position = ClampPosition(position);
                
        placementValidator.SetPosition(position, _objectToPlacePrefab);
    }

    private Vector3 GetNearestPointOnGrid(Vector3 point)
    {
        int xCount = (int)(point.x / gridSettings.cellSize);
        int yCount = (int)(point.y / gridSettings.cellSize);
        int zCount = (int)(point.z / gridSettings.cellSize);

        var result = new Vector3(xCount + 1, yCount, zCount + 1);

        result *= gridSettings.cellSize;

        result = ShiftPosition(result);
        result = ClampPosition(result);

        return result;
    }

    private Vector3 ShiftPosition(Vector3 position)
    {
        if ((int)placementValidator.PlacementCollider.bounds.size.x / gridSettings.cellSize % 2 != 0)
        {
            position.x -= gridSettings.cellSize / 2;
        }

        if ((int)placementValidator.PlacementCollider.bounds.size.z / gridSettings.cellSize % 2 != 0)
        {
            position.z -= gridSettings.cellSize / 2;
        }

        return position;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        var bounds = placementValidator.PlacementCollider.bounds;

        var offsetX = bounds.extents.x;
        var offsetZ = bounds.extents.z;

        position.x = Mathf.Clamp(position.x, 0 + offsetX, gridSettings.sizeX - offsetX);
        position.z = Mathf.Clamp(position.z, 0 + offsetZ, gridSettings.sizeZ - offsetZ);

        position.y = _objectOriginY + _objectElevation;

        return position;
    }

    private void PlaceObject()
    {
        //if UI was clicked return
        if (RayCaster.RayCastUIObject(out RaycastResult raycastResult))
            return;

        //if grid was NOT clicked return
        if (RayCaster.RayCastGameObject(out RaycastHit hitInfo) == false)
            return;

        if (gridSettings.collisionDetection && gridSettings.replaceOnCollision == false)
        {
            if (placementValidator.IsOverlapping || placementValidator.IsOnGround == false)
            {
                return;
            }
        }

        if (gridSettings.replaceOnCollision == true && placementValidator.IsOverlapping)
        {
            for (int i = 0; i < placementValidator.OverlappedColliders.Count; i++)
            {
                if (placementValidator.OverlappedColliders[i].TryGetComponent(out IGridObjectTryToReplace objectTryToReplace))
                {
                    if (objectTryToReplace.OnGridObjectTryToReplace() == false)
                    {
                        return;
                    }
                }
            }
        }

        bool success = brushObjectsHolder.TryPlaceObjectsOnMap(map);

        if (success == false)
            return;

        brushObjectsHolder.ClearObjects();

        //destroy overlapped objects if setting is turned on
        if (gridSettings.collisionDetection && gridSettings.replaceOnCollision)
        {
            List<int> removedIds = placementValidator.DestroyOverlappedObjects();

            removedIds.ForEach(x => map.ObjectRemoved(x));
        }

        if (gridSettings.continuousBuilding)
        {
            InstantiateBrushObjects(placementValidator.transform.position);
        }
    }

    public void OnBuildOptionClicked(GameObject gameObject)
    {
        DestroyBrushObjects();

        _objectToPlacePrefab = gameObject;
        IsBuildingModeActive = true;

        GridSettings.brushSize = 1;
        OnBrushSizeChangedEvent?.Invoke(1);
        InstantiateBrushObjects(gridDisplay.GetGridBasePosition());
    }

    private void DestroyBrushObjects()
    {
        if (brushObjectsHolder.IsHoldingObjects)
        {
            brushObjectsHolder.DestroyObjects();
        }
        IsBuildingModeActive = false;
    }

    private void DestroyClickedObject()
    {
        if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
        {
            if (hitInfo.transform.IsChildOf(map.transform))
            {
                if (hitInfo.transform.gameObject.TryGetComponent(out IGridObjectTryToRemove gridObjectTryToRemove))
                {
                    if (gridObjectTryToRemove.OnGridObjectTryToRemove() == false)
                    {
                        return;
                    }
                }

                if (hitInfo.transform.gameObject.TryGetComponent(out IGridObjectRemoved gridObjectRemoved))
                {
                    gridObjectRemoved.OnGridObjectRemoved();
                }

                map.ObjectRemoved(hitInfo.transform.gameObject.GetInstanceID());
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }

    private void InstantiateBrushObjects(Vector3 position)
    {
        brushObjectsHolder.InstantiateObjects(_objectToPlacePrefab, gridSettings.brushSize);

        _objectOriginY = brushObjectsHolder.GetOriginY();

        SetObjectPosition(position);
    }

    public void OnBuildingModeChanged(bool continuous)
    {
        gridSettings.continuousBuilding = continuous;
    }

    public void OnAvoidUnbuildableTerrainChanged(bool avoid)
    {
        gridSettings.avoidUnbuildableTerrain = avoid;
    }

    public void OnGridSnappingChanged(bool snapToGrid)
    {
        gridSettings.snapToGrid = snapToGrid;
    }

    public void OnAutoHeightChanged(bool autoHeight)
    {
        gridSettings.autoHeight = autoHeight;

        if (gridSettings.autoHeight)
        {
            gridDisplay.ResetElevation();
        }
    }

    public void OnCollisionDetectionChanged(bool collisionDetection)
    {
        gridSettings.collisionDetection = collisionDetection;
    }

    public void OnReplaceOnCollisionChanged(bool replaceOnCollision)
    {
        gridSettings.replaceOnCollision = replaceOnCollision;
    }

    //public void OnSnapToTerrainOnlyChanged(bool snapToTerrainOnly)
    //{
    //    gridSettings.buildOnlyOnTerrain = snapToTerrainOnly;
    //}

    public void OnMapBeingLoaded(MapSaveData mapSaveData)
    {
        if (mapSaveData.GridSettings != null)
        {
            SetGridDimensions(mapSaveData.GridSettings.sizeX, mapSaveData.GridSettings.sizeZ, false);
        }
    }

    public void OnMapBeingSaved(MapSaveData mapSaveData)
    {
        mapSaveData.GridSettings = gridSettings;
    }

    public int OnBrushSizeChanged(int brushSize)
    {
        if (_objectToPlacePrefab == null || brushObjectsHolder.IsHoldingObjects == false)
            return 1;

        if (_objectToPlacePrefab.TryGetComponent(out IGridObjectTryChangeBrushSize gridObjectTryChangeBrushSize))
        {
            if (gridObjectTryChangeBrushSize.OnGridObjectTryChangeBrushSize(brushSize) == false)
            {
                OnBrushSizeChangedEvent?.Invoke(1);
                return 1;
            }
        }

        if (brushSize != gridSettings.brushSize)
        {
            gridSettings.brushSize = brushSize;

            if (brushObjectsHolder.IsHoldingObjects)
            {
                DestroyBrushObjects();
                InstantiateBrushObjects(placementValidator.transform.position);
            }
        }

        OnBrushSizeChangedEvent?.Invoke(gridSettings.brushSize);
        return gridSettings.brushSize;
    }
}
