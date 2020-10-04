using System.Linq;
using UnityEngine;

public class GridSnapping : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] float cellSize;
    [SerializeField] int maxElevation;
    [SerializeField] OverlapHandler overlapHandler;

    private int _elevation;

    private GameObject _cachedPrefab;
    private GameObject _initializedObject;
    private float _originY;

    private bool _isContinuous;

    private void Awake()
    {
        var position = transform.position;

        position.x += transform.localScale.x / 2;
        position.z += transform.localScale.y / 2;

        transform.position = position;

        overlapHandler = Instantiate(overlapHandler);
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
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ChangeElevation();
        }

        if (_initializedObject != null)
        {
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

    private void ChangeElevation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0 && _elevation > 0)
        {
            _elevation--;
        }

        if (scroll > 0 && _elevation < maxElevation)
        {
            _elevation++;
        }
    }

    private void MoveObject()
    {
        if (RayCastAll(out RaycastHit hitInfo))
        {
            Vector3 position = GetNearestPointOnGrid(hitInfo.point);

            _initializedObject.transform.position = position;
            overlapHandler.SetPosition(_initializedObject.GetComponent<Collider>().bounds.center);
        }
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

        var result = new Vector3(xCount, yCount, zCount);

        result *= cellSize;

        var bounds = _initializedObject.GetComponent<Collider>().bounds;

        var offsetX = bounds.extents.x;
        var offsetZ = bounds.extents.z;

        result.x += offsetX;
        result.z += offsetZ;

        result.x = Mathf.Clamp(result.x, 0 + offsetX, transform.localScale.x - offsetX);
        result.z = Mathf.Clamp(result.z, 0 + offsetZ, transform.localScale.y - offsetZ);

        result.y = _originY + _elevation;

        return result;
    }

    private void PlaceObject()
    {
        //only if was clicked on building plane to avoid placing block while clicking on UI
        if (RayCastAll(out RaycastHit hitInfo))
        {
            _initializedObject = null;

            overlapHandler.RemoveOverlappedObjects();

            if (_isContinuous)
            {
                InstantiatePrefab();
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
            Destroy(_initializedObject);
        }
        _initializedObject = null;
    }

    private void InstantiatePrefab()
    {
        _initializedObject = Instantiate(_cachedPrefab);

        _originY = _initializedObject.transform.position.y;
        _initializedObject.transform.position = GetNearestPointOnGrid(transform.position);

        overlapHandler.RegisterParent(_initializedObject);
        overlapHandler.ResizeCollider(_initializedObject.GetComponent<Collider>().bounds.size);

        //random color
        var material = _initializedObject.GetComponent<MeshRenderer>().material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void OnBuildingModeChanged(bool continuous)
    {
        _isContinuous = continuous;
    }
}
