using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] GridSettings gridSettings;
    [SerializeField] GridDisplay gridDisplay;
    [SerializeField] PlacementHandler placementHandler;
    
    [SerializeField] MapSaveManager map;

    private int _objectElevation;
    private float _objectOriginY;

    private GameObject _objectToPlacePrefab;
    private GameObject _objectToPlace;
    private Collider _objectToPlaceCollider;

    public GridSettings GridSettings => gridSettings;

    private void Awake()
    {
        placementHandler = Instantiate(placementHandler);

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

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl) == false)
        {
            ChangeElevation();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DestroyObject();
        }

        if (_objectToPlace != null)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl))
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
                DestroyObject();
            }
        }
    }

    private void ChangeRotation()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _objectToPlace.transform.Rotate(Vector3.forward, 90f);
        }
        else
        {
            _objectToPlace.transform.Rotate(Vector3.up, 90f);
        }
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
        if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
        {
            SetObjectPosition(hitInfo.point);
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

        _objectToPlace.transform.position = position;
        placementHandler.SetPosition(position);
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
        if ((int)_objectToPlaceCollider.bounds.size.x / gridSettings.cellSize % 2 != 0)
        {
            position.x -= gridSettings.cellSize / 2;
        }

        if ((int)_objectToPlaceCollider.bounds.size.z / gridSettings.cellSize % 2 != 0)
        {
            position.z -= gridSettings.cellSize / 2;
        }

        return position;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        var bounds = _objectToPlaceCollider.bounds;

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
            if (placementHandler.IsOverlapping || placementHandler.IsOnGround == false)
            {
                return;
            }
        }

        map.ObjectPlaced(_objectToPlace, _objectToPlacePrefab);

        placementHandler.DeregisterParent();

        _objectToPlace.layer = (int)LayerEnum.Default;
        _objectToPlace = null;

        if (gridSettings.collisionDetection && gridSettings.replaceOnCollision)
        {
            List<int> removedIds = placementHandler.RemoveOverlappedObjects();

            removedIds.ForEach(x => map.ObjectRemoved(x));
        }

        if (gridSettings.continuousBuilding)
        {
            InstantiatePrefab();
        }
    }

    public void OnBuildOptionClicked(GameObject gameObject)
    {
        DestroyObject();

        _objectToPlacePrefab = gameObject;

        InstantiatePrefab();
    }

    private void DestroyObject()
    {
        if (_objectToPlace != null)
        {
            placementHandler.ParentDestroyed();
            Destroy(_objectToPlace);
        }
        _objectToPlace = null;
    }

    private void InstantiatePrefab()
    {
        _objectToPlace = Instantiate(_objectToPlacePrefab, map.transform);
        _objectToPlaceCollider = _objectToPlace.GetComponent<Collider>();
        _objectToPlace.layer = (int)LayerEnum.IgnoreRayCast;

        _objectOriginY = _objectToPlace.transform.position.y + _objectToPlaceCollider.bounds.extents.y - _objectToPlaceCollider.bounds.center.y;

        placementHandler.RegisterParent(_objectToPlace);
        placementHandler.ResizeCollider(_objectToPlaceCollider.bounds.size);

        SetObjectPosition(gridDisplay.GetGridBasePosition());
    }

    public void OnBuildingModeChanged(bool continuous)
    {
        gridSettings.continuousBuilding = continuous;
    }

    public void OnAvoidUnbuildableTerrainChanged(bool avoid)
    {
        gridSettings.avoidUnbuildableTerrain = avoid;

        placementHandler.OnAvoidUnbuildableTerrainChanged(avoid);
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
}
