using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] GridSettings gridSettings;
    [SerializeField] GridDisplay gridDisplay;
    [SerializeField] PlacementValidator placementValidator;
    
    [SerializeField] MapSaveManager map;

    private int _objectElevation;
    private float _objectOriginY;

    private GameObject _objectToPlacePrefab;
    private GameObject _objectToPlace;
    private IUpgradeable _objectToPlaceBuild;

    public GridSettings GridSettings => gridSettings;

    public static bool IsBuildingModeActive { get; private set; } = false;

    private void Awake()
    {
        placementValidator = Instantiate(placementValidator);
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

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl) == false)
        {
            ChangeElevation();
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
                DestroyObjectToPlace();
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
        if (Input.GetKey(KeyCode.Space))
        {
            _objectToPlace.transform.Rotate(Vector3.forward, 90f);
        }
        else
        {
            _objectToPlace.transform.Rotate(Vector3.up, 90f);
        }
        var size = Quaternion.Euler(_objectToPlace.transform.rotation.eulerAngles) * _objectToPlace.GetComponent<Collider>().bounds.size;

        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        size.z = Mathf.Abs(size.z);

        _objectOriginY = _objectToPlace.transform.position.y + size.y / 2 - _objectToPlace.GetComponent<Collider>().bounds.center.y; ;

        placementValidator.ResizeCollider(size);
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
        if (gridSettings.buildOnlyOnTerrain)
        {
            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo, "Terrain"))
            {
                SetObjectPosition(hitInfo.point);
            }
        }
        else
        {
            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
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

        _objectToPlace.transform.position = position;
        placementValidator.SetPosition(position);
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

        //if object implements IConstruction interface check if is able to construct and call construction start
        if (_objectToPlaceBuild != null)
        {
            _objectToPlaceBuild.OnUpgradeStarted(_objectToPlaceBuild.CurrentUpgrade, out bool upgradeStarted);

            if (upgradeStarted == false)
                return;
        }

        _objectToPlace.layer = (int)LayerEnum.Default;
        //_objectToPlace.GetComponentsInChildren<MonoBehaviour>().ToList().ForEach(x => x.enabled = true);

        map.ObjectPlaced(_objectToPlace, _objectToPlacePrefab);

        placementValidator.DeregisterParent();
        
        _objectToPlace = null;

        if (gridSettings.collisionDetection && gridSettings.replaceOnCollision)
        {
            List<int> removedIds = placementValidator.RemoveOverlappedObjects();

            removedIds.ForEach(x => map.ObjectRemoved(x));
        }

        if (gridSettings.continuousBuilding)
        {
            InstantiatePrefab();
        }
    }

    public void OnBuildOptionClicked(GameObject gameObject)
    {
        DestroyObjectToPlace();

        _objectToPlacePrefab = gameObject;
        IsBuildingModeActive = true;

        InstantiatePrefab();
    }

    private void DestroyObjectToPlace()
    {
        if (_objectToPlace != null)
        {
            Destroy(_objectToPlace);
        }
        _objectToPlace = null;
        IsBuildingModeActive = false;
    }

    private void DestroyClickedObject()
    {
        if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
        {
            if (hitInfo.transform.IsChildOf(map.transform))
            {
                map.ObjectRemoved(hitInfo.transform.gameObject.GetInstanceID());
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }

    private void InstantiatePrefab()
    {
        _objectToPlace = Instantiate(_objectToPlacePrefab, map.transform);
        _objectToPlace.layer = (int)LayerEnum.IgnoreRayCast;
        
        _objectToPlaceBuild = _objectToPlace.GetComponent<IUpgradeable>();
        var objectToPlaceBounds = _objectToPlace.GetComponent<Collider>().bounds;

        _objectOriginY = _objectToPlace.transform.position.y + objectToPlaceBounds.extents.y - objectToPlaceBounds.center.y;

        placementValidator.RegisterParent(_objectToPlace);

        SetObjectPosition(gridDisplay.GetGridBasePosition());
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
}
