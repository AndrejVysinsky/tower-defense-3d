using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] float cellSize;
    [SerializeField] int maxElevation;
    [SerializeField] OverlapHandler overlapHandler;
    [SerializeField] GameObject mapContainer;
    [SerializeField] GridDisplay gridDisplay;

    private int _elevation;
    private float _heightOffSet = 0.01f;

    private GameObject _cachedPrefab;
    private GameObject _initializedObject;
    private Collider _initializedObjectCollider;
    private float _originY;

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

        gridDisplay.CalculateGrid((int)transform.localScale.x, (int)transform.localScale.y, cellSize, _elevation);
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

        if (_initializedObject != null)
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
        _initializedObject.transform.Rotate(Vector3.up, 90f);
    }

    private void ChangeElevation()
    {
        if (_autoHeight)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0 && _elevation > 0)
        {
            _elevation--;
            gridDisplay.ChangeGridElevation(_elevation);

            var position = transform.position;
            position.y = _elevation + _heightOffSet;
            transform.position = position;
        }

        if (scroll > 0 && _elevation < maxElevation)
        {
            _elevation++;
            gridDisplay.ChangeGridElevation(_elevation);

            var position = transform.position;
            position.y = _elevation + _heightOffSet;
            transform.position = position;
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
            _elevation = Mathf.RoundToInt(position.y);
        }

        if (_snapToGrid)
        {
            position = GetNearestPointOnGrid(position);
        }

        position = ClampPosition(position);

        _initializedObject.transform.position = position;
        overlapHandler.SetPosition(position);
    }

    private bool RayCastAll(out RaycastHit hitInfo)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var hitInfos = Physics.RaycastAll(ray).ToList();

        hitInfos.RemoveAll(x => x.transform.gameObject == _initializedObject || x.transform.gameObject == overlapHandler.gameObject);

        hitInfos = hitInfos.OrderBy(x => x.distance).ToList();

        if (hitInfos.Count > 0)
        {
            hitInfo = hitInfos[0];
            return true;
        }
        else
        {
            hitInfo = new RaycastHit();
            return false;
        }
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
        if ((int)_initializedObjectCollider.bounds.size.x / cellSize % 2 != 0)
        {
            position.x -= cellSize / 2;
            Debug.Log("x shifted");
        }

        if ((int)_initializedObjectCollider.bounds.size.z / cellSize % 2 != 0)
        {
            position.z -= cellSize / 2;
            Debug.Log("z shifted");
        }

        return position;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        var bounds = _initializedObjectCollider.bounds;

        var offsetX = bounds.extents.x;
        var offsetZ = bounds.extents.z;

        position.x = Mathf.Clamp(position.x, 0 + offsetX, transform.localScale.x - offsetX);
        position.z = Mathf.Clamp(position.z, 0 + offsetZ, transform.localScale.y - offsetZ);

        position.y = _originY + _elevation;

        return position;
    }

    private void PlaceObject()
    {
        //check if UI was clicked, if not check if building plane was clicked, then place object
        if (RayCaster.RayCastNearestUIObject(out RaycastResult raycastResult) == false)
        {
            if (RayCaster.RayCastNearestGameObject(out RaycastHit hitInfo))
            {
                _initializedObject.layer = (int)LayerEnum.Default;
                _initializedObject = null;

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

        _cachedPrefab = gameObject;

        InstantiatePrefab();
    }

    private void DestroyCurrentObject()
    {
        if (_initializedObject != null)
        {
            overlapHandler.ParentDestroyed();
            Destroy(_initializedObject);
        }
        _initializedObject = null;
    }

    private void InstantiatePrefab()
    {
        _initializedObject = Instantiate(_cachedPrefab, mapContainer.transform);
        _initializedObjectCollider = _initializedObject.GetComponent<Collider>();
        _initializedObject.layer = (int)LayerEnum.IgnoreRayCast;

        _originY = _initializedObject.transform.position.y + _initializedObjectCollider.bounds.extents.y - _initializedObjectCollider.bounds.center.y;

        overlapHandler.RegisterParent(_initializedObject);
        overlapHandler.ResizeCollider(_initializedObjectCollider.bounds.size);

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

        //if (_autoHeight)
        //    _elevation = 0;
    }
}
