using UnityEngine;

public class GridSnapping : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] float cellSize;
    [SerializeField] int maxElevation;

    private int _elevation;

    private GameObject _initializedObject;
    private float _originY;

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

        Debug.Log(_elevation);
    }

    private void MoveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 position;

            position = GetNearestPointOnGrid(hitInfo.point);

            position.y = _originY + _elevation;

            _initializedObject.transform.position = position;
        }
    }

    private Vector3 GetNearestPointOnGrid(Vector3 point)
    {
        point -= transform.position;

        int xCount = Mathf.RoundToInt(point.x / cellSize);
        int yCount = Mathf.RoundToInt(point.y / cellSize);
        int zCount = Mathf.RoundToInt(point.z / cellSize);

        var result = new Vector3(xCount, yCount, zCount);

        result *= cellSize;
        result += transform.position;

        return result;
    }

    private void PlaceObject()
    {
        _initializedObject = null;
    }

    public void OnBuildOptionClicked(GameObject gameObject)
    {
        if (_initializedObject != null)
        {
            Destroy(_initializedObject);
        }

        _initializedObject = Instantiate(gameObject);
        _originY = _initializedObject.transform.position.y;

        var material = _initializedObject.GetComponent<MeshRenderer>().material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
