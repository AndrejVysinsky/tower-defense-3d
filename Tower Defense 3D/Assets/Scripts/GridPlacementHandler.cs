using UnityEngine;
using UnityEngine.EventSystems;

public class GridPlacementHandler : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] int sizeX;
    [SerializeField] int sizeZ;
    [SerializeField] float cellSize;
    [SerializeField] OverlapHandler overlapHandler;
    [SerializeField] GameObject mapContainer;
    [SerializeField] GridDisplay gridDisplay;

    private int _objectElevation;
    private float _objectOriginY;

    private GameObject _objectToPlacePrefab;
    private GameObject _objectToPlace;
    private Collider _objectToPlaceCollider;
    
    //settings
    private bool _continuousBuilding;
    private bool _snapToGrid;
    private bool _autoHeight;

    private void Awake()
    {
        var position = transform.position;

        position.x = transform.localScale.x / 2;
        position.z = transform.localScale.y / 2;

        transform.position = position;

        overlapHandler = Instantiate(overlapHandler);

        gridDisplay.CalculateGrid(sizeX, sizeZ, cellSize, _objectElevation);
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
                DestroyCurrentObject();
            }
        }
    }

    private void ChangeRotation()
    {
        _objectToPlace.transform.Rotate(Vector3.up, 90f);
    }

    private void ChangeElevation()
    {
        if (_autoHeight)
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
        if (RayCaster.RayCastNearestGameObject(out RaycastHit hitInfo))
        {
            SetObjectPosition(hitInfo.point);
        }
    }

    private void SetObjectPosition(Vector3 position)
    {
        if (_autoHeight)
        {
            _objectElevation = Mathf.RoundToInt(position.y);
        }
        else
        {
            _objectElevation = gridDisplay.GetGridElevation();
        }

        if (_snapToGrid)
        {
            position = GetNearestPointOnGrid(position);
        }

        position = ClampPosition(position);

        _objectToPlace.transform.position = position;
        overlapHandler.SetPosition(position);
    }

    private Vector3 GetNearestPointOnGrid(Vector3 point)
    {
        int xCount = (int)(point.x / cellSize);
        int yCount = (int)(point.y / cellSize);
        int zCount = (int)(point.z / cellSize);

        var result = new Vector3(xCount + 1, yCount, zCount + 1);

        result *= cellSize;

        result = ShiftPosition(result);
        result = ClampPosition(result);

        return result;
    }

    private Vector3 ShiftPosition(Vector3 position)
    {
        if ((int)_objectToPlaceCollider.bounds.size.x / cellSize % 2 != 0)
        {
            position.x -= cellSize / 2;
        }

        if ((int)_objectToPlaceCollider.bounds.size.z / cellSize % 2 != 0)
        {
            position.z -= cellSize / 2;
        }

        return position;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        var bounds = _objectToPlaceCollider.bounds;

        var offsetX = bounds.extents.x;
        var offsetZ = bounds.extents.z;

        position.x = Mathf.Clamp(position.x, 0 + offsetX, transform.localScale.x - offsetX);
        position.z = Mathf.Clamp(position.z, 0 + offsetZ, transform.localScale.y - offsetZ);

        position.y = _objectOriginY + _objectElevation;

        return position;
    }

    private void PlaceObject()
    {
        //check if UI was clicked, if not check if building plane was clicked, then place object
        if (RayCaster.RayCastNearestUIObject(out RaycastResult raycastResult) == false)
        {
            if (RayCaster.RayCastNearestGameObject(out RaycastHit hitInfo))
            {
                _objectToPlace.layer = (int)LayerEnum.Default;
                _objectToPlace = null;

                overlapHandler.RemoveOverlappedObjects();

                if (_continuousBuilding)
                {
                    InstantiatePrefab();
                }
            }
        }
    }

    public void OnBuildOptionClicked(GameObject gameObject)
    {
        DestroyCurrentObject();

        _objectToPlacePrefab = gameObject;

        InstantiatePrefab();
    }

    private void DestroyCurrentObject()
    {
        if (_objectToPlace != null)
        {
            overlapHandler.ParentDestroyed();
            Destroy(_objectToPlace);
        }
        _objectToPlace = null;
    }

    private void InstantiatePrefab()
    {
        _objectToPlace = Instantiate(_objectToPlacePrefab, mapContainer.transform);
        _objectToPlaceCollider = _objectToPlace.GetComponent<Collider>();
        _objectToPlace.layer = (int)LayerEnum.IgnoreRayCast;

        _objectOriginY = _objectToPlace.transform.position.y + _objectToPlaceCollider.bounds.extents.y - _objectToPlaceCollider.bounds.center.y;

        overlapHandler.RegisterParent(_objectToPlace);
        overlapHandler.ResizeCollider(_objectToPlaceCollider.bounds.size);

        SetObjectPosition(transform.position);
    }

    public void OnBuildingModeChanged(bool continuous)
    {
        _continuousBuilding = continuous;
    }

    public void OnGridSnappingChanged(bool snapToGrid)
    {
        _snapToGrid = snapToGrid;
    }

    public void OnHideCollidingObjectsChanged(bool hideCollidingObjects)
    {
        overlapHandler.SetHideCollidingObjects(hideCollidingObjects);
    }

    public void OnAutoHeightChanged(bool autoHeight)
    {
        _autoHeight = autoHeight;

        if (_autoHeight)
        {
            gridDisplay.ResetElevation();
        }
    }
}
