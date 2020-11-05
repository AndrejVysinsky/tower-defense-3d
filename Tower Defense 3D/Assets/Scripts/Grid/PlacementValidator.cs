﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] Material validPlacementMaterial;
    [SerializeField] Material invalidPlacementMaterial;

    private ValidityIndicator _validityIndicator;
    private GameObject _parentObject;

    private List<GameObject> _objectsInRange;
    private List<GameObject> _overlappedObjects;
    
    private readonly float _maxRangeTollerance = 0.05f;

    private IConstruction _construction;

    public bool IsOverlapping => _overlappedObjects.Count > 0;
    public bool IsOnGround { get; private set; }
    public BoxCollider PlacementCollider { get; private set; }

    public GridSettings GridSettings { get; set; }

    private void Awake()
    {
        _validityIndicator = new ValidityIndicator(validPlacementMaterial, invalidPlacementMaterial);

        PlacementCollider = GetComponent<BoxCollider>();

        _objectsInRange = new List<GameObject>();
        _overlappedObjects = new List<GameObject>();
    }

    public void RegisterParent(GameObject parent, IConstruction construction)
    {
        _parentObject = parent;
        _validityIndicator.RegisterObjectMaterials(parent);
        
        _construction = construction;

        ResizeCollider(parent.GetComponent<Collider>().bounds.size);
    }

    public void DeregisterParent()
    {
        if (_parentObject != null)
        {
            _parentObject = null;
            _validityIndicator.SwitchBackObjectMaterials();
        }
    }

    public void ResizeCollider(Vector3 size)
    {
        Vector3 newSize = size;

        if (GridSettings.snapToGrid)
        {
            newSize.x = GridSettings.cellSize;
            newSize.z = GridSettings.cellSize;
            newSize.y = size.y;

            while (newSize.x < size.x && Mathf.Approximately(newSize.x, size.x) == false)
                newSize.x += GridSettings.cellSize;

            while (newSize.z < size.z && Mathf.Approximately(newSize.z, size.z) == false)
                newSize.z += GridSettings.cellSize;
        }

        PlacementCollider.size = newSize;

        //to register OnTriggerEnter
        PlacementCollider.enabled = false;
        PlacementCollider.enabled = true;
    }

    public void SetPosition(Vector3 position)
    {
        if (transform.position == position)
            return;

        transform.position = position;

        CheckValidity();
    }

    private void CheckValidity()
    {
        CheckOverlap();

        IsOnGround = IsWholeObjectOnGround();

        bool isValidPlacement = IsOverlapping == false && IsOnGround;

        if (_construction != null)
        {
            isValidPlacement &= _construction.IsAbleToStartConstruction;
        }

        _validityIndicator.SetMaterial(isValidPlacement);
    }

    private void CheckOverlap()
    {
        var bounds = PlacementCollider.bounds;

        _overlappedObjects.Clear();
        _objectsInRange.RemoveAll(x => x == null);

        foreach (var overlappedObject in _objectsInRange)
        {
            if (IsHigherOrLowerThan(overlappedObject))
            {
                continue;
            }

            var point = overlappedObject.GetComponent<Collider>().ClosestPoint(transform.position);

            if (IsPointInsideMyBounds(bounds, point))
            {                
                _overlappedObjects.Add(overlappedObject);
            }
        }
    }

    private bool IsWholeObjectOnGround()
    {
        if (_objectsInRange.Count == 0)
            return true;
        
        //get lower touching objects
        List<Collider> colliders = new List<Collider>();

        _objectsInRange.ForEach(x =>
        {
            if (IsTouchingBottom(x))
            {
                colliders.Add(x.GetComponent<Collider>());
            }
        });

        if (colliders.Count == 0)
            return true;

        //check their range
        float minX = transform.position.x - PlacementCollider.size.x / 2;
        float maxX = transform.position.x + PlacementCollider.size.x / 2;

        float minZ = transform.position.z - PlacementCollider.size.z / 2;
        float maxZ = transform.position.z + PlacementCollider.size.z / 2;

        var point = Vector3.zero;
        point.y = transform.position.y - PlacementCollider.size.y / 2;

        for (float i = minX; i <= maxX; i += 0.5f)
        {
            for (float j = minZ; j <= maxZ; j += 0.5f)
            {
                point.x = i;
                point.z = j;

                bool isPointOnGround = false;

                foreach (var collider in colliders)
                {
                    if (GridSettings.avoidUnbuildableTerrain && collider.gameObject.layer == (int)LayerEnum.UnbuildableTerrain)
                        continue;

                    if (IsPointInsideOtherBounds(collider.bounds, point))
                    {
                        isPointOnGround = true;
                    }
                }

                if (isPointOnGround == false)
                    return false;
            }
        }
        return true;
    }

    private bool IsTouchingBottom(GameObject other)
    {
        var otherBounds = other.GetComponent<Collider>().bounds;

        float upperYOfStaticGameObject = other.transform.position.y + otherBounds.extents.y;
        float lowerYOfMovingGameObject = transform.position.y - PlacementCollider.bounds.extents.y;

        return Mathf.Abs(upperYOfStaticGameObject - lowerYOfMovingGameObject) <= _maxRangeTollerance;
    }

    private bool IsPointInsideOtherBounds(Bounds bounds, Vector3 point)
    {
        float minX = bounds.center.x - bounds.extents.x - _maxRangeTollerance;
        float maxX = bounds.center.x + bounds.extents.x + _maxRangeTollerance;

        float minZ = bounds.center.z - bounds.extents.z - _maxRangeTollerance;
        float maxZ = bounds.center.z + bounds.extents.z + _maxRangeTollerance;

        if (point.x >= minX && point.x <= maxX && point.z >= minZ && point.z <= maxZ)
        {
            return true;
        }
        return false;
    }

    private bool IsPointInsideMyBounds(Bounds bounds, Vector3 point)
    {
        float minX = transform.position.x - bounds.extents.x + _maxRangeTollerance;
        float maxX = transform.position.x + bounds.extents.x - _maxRangeTollerance;

        float minZ = transform.position.z - bounds.extents.z + _maxRangeTollerance;
        float maxZ = transform.position.z + bounds.extents.z - _maxRangeTollerance;

        if (point.x > minX && point.x < maxX && point.z > minZ && point.z < maxZ)
        {
            return true;
        }
        return false;
    }

    private bool IsHigherOrLowerThan(GameObject gameObject)
    {
        var otherBounds = gameObject.GetComponent<Collider>().bounds;

        var upperYOfStaticGameObject = gameObject.transform.position.y + otherBounds.extents.y - _maxRangeTollerance;
        var lowerYOfStaticGameObject = gameObject.transform.position.y - otherBounds.extents.y + _maxRangeTollerance;

        var upperYOfMovingGameObject = transform.position.y + PlacementCollider.bounds.extents.y - _maxRangeTollerance;
        var lowerYOfMovingGameObject = transform.position.y - PlacementCollider.bounds.extents.y + _maxRangeTollerance;

        return lowerYOfMovingGameObject >= upperYOfStaticGameObject || upperYOfMovingGameObject <= lowerYOfStaticGameObject;
    }

    public List<int> RemoveOverlappedObjects()
    {
        List<int> result = new List<int>();
        result.AddRange(_overlappedObjects.Select(x => x.GetInstanceID()));

        _objectsInRange.RemoveAll(x => _overlappedObjects.Contains(x));

        _overlappedObjects.ForEach(x => Destroy(x));
        _overlappedObjects.Clear();

        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _parentObject || _objectsInRange.Contains(other.gameObject) || other.gameObject.layer == (int)LayerEnum.UI
            || other.gameObject.layer == (int)LayerEnum.IgnoreRayCast)
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);

        CheckValidity();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _parentObject)
        {
            return;
        }

        _objectsInRange.Remove(other.gameObject);
    }
}
