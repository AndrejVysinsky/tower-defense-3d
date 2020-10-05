﻿using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour, IBuildOptionClicked
{
    [SerializeField] float cellSize;
    [SerializeField] int maxElevation;
    [SerializeField] OverlapHandler overlapHandler;
    [SerializeField] GameObject mapContainer;

    private int _elevation;

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
        if (_initializedObject != null)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    ChangeRotation();
                }
                else
                {
                    ChangeElevation();
                }
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
        result = ClampPosition(result);

        return result;
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
        //only if was clicked on building plane to avoid placing block while clicking on UI
        if (RayCastAll(out RaycastHit hitInfo))
        {
            _initializedObject = null;

            overlapHandler.RemoveOverlappedObjects();

            if (_continuousBuilding)
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
            overlapHandler.ParentDestroyed();
            Destroy(_initializedObject);
        }
        _initializedObject = null;
    }

    private void InstantiatePrefab()
    {
        _initializedObject = Instantiate(_cachedPrefab, mapContainer.transform);
        _initializedObjectCollider = _initializedObject.GetComponent<Collider>();

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
    }
}