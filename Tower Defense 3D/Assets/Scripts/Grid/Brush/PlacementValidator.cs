using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] Material validPlacementMaterial;
    [SerializeField] Material invalidPlacementMaterial;

    private List<ValidityIndicator> _validityIndicators;

    private List<GameObject> _objectsInRange;
    private List<GameObject> _overlappedObjects;

    private Transform _myTransform;
    private Collider[] _colliderBuffer = new Collider[500];
    private BrushObjectsHolder _brushObjectsHolder;
    private readonly float _maxRangeTollerance = 0.05f;

    public bool IsOverlapping => _overlappedObjects.Count > 0;
    public bool IsOnGround { get; private set; }
    public BoxCollider PlacementCollider { get; private set; }

    public GridSettings GridSettings { get; set; }

    private void Awake()
    {
        _myTransform = transform;

        _validityIndicators = new List<ValidityIndicator>();

        PlacementCollider = GetComponent<BoxCollider>();

        _objectsInRange = new List<GameObject>();
        _overlappedObjects = new List<GameObject>();
        _brushObjectsHolder = GetComponent<BrushObjectsHolder>();
    }

    public void RegisterChildren(List<GameObject> children, Vector3 totalSize)
    {
        _validityIndicators.Clear();

        for (int i = 0; i < children.Count; i++)
        {
            var validityIndicator = new ValidityIndicator(validPlacementMaterial, invalidPlacementMaterial);
            validityIndicator.RegisterObjectMaterials(children[i]);

            _validityIndicators.Add(validityIndicator);
        }

        ResizeCollider(totalSize);
    }

    public void DeregisterChildren()
    {
        for (int i = 0; i < _validityIndicators.Count; i++)
        {
            _validityIndicators[i].SwitchBackObjectMaterials();
        }
        _validityIndicators.Clear();
    }

    private void ResizeCollider(Vector3 size)
    {
        size = GetRotatedColliderSize(size);

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
        if (_myTransform.position == position)
            return;

        _myTransform.position = position;

        StartCoroutine(CheckValidity());
    }

    public void Rotate(Vector3 direction, float angle)
    {
        _myTransform.Rotate(direction, angle);

        StartCoroutine(CheckValidity());
    }

    private IEnumerator CheckValidity()
    {
        yield return new WaitForFixedUpdate();

        CheckOverlap();

        IsOnGround = IsWholeObjectOnGround();

        bool isValidPlacement = IsOverlapping == false && IsOnGround;

        for (int i = 0; i < _validityIndicators.Count; i++)
        {
            _validityIndicators[i].SetMaterial(isValidPlacement);
        }
    }

    private void CheckOverlap()
    {
        var bounds = PlacementCollider.bounds;

        _overlappedObjects.Clear();
        _objectsInRange.RemoveAll(x => x == null);

        foreach (var overlappedObject in _objectsInRange)
        {
            var overlappedCollider = overlappedObject.GetComponent<Collider>();

            if (IsHigherOrLowerThan(overlappedObject))
            {
                continue;
            }

            var point = overlappedObject.GetComponent<Collider>().ClosestPoint(_myTransform.position);

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
        int colliderCount = 0;

        for (int i = 0; i < _objectsInRange.Count; i++)
        {
            var collider = _objectsInRange[i].GetComponent<Collider>();

            if (IsTouchingBottom(collider.bounds))
            {
                _colliderBuffer[colliderCount++] = collider;
            }
        }

        if (colliderCount == 0)
            return true;

        //check their range
        var myColliderSize = GetRotatedColliderSize(PlacementCollider.size);

        float minX = _myTransform.position.x - myColliderSize.x / 2;
        float maxX = _myTransform.position.x + myColliderSize.x / 2;

        float minZ = _myTransform.position.z - myColliderSize.z / 2;
        float maxZ = _myTransform.position.z + myColliderSize.z / 2;

        var point = Vector3.zero;
        point.y = _myTransform.position.y - myColliderSize.y / 2;

        //check every corner of collider with incrementing step of smallest grid unit
        for (float i = minX; i <= maxX; i += GridSettings.cellSize)
        {
            for (float j = minZ; j <= maxZ; j += GridSettings.cellSize)
            {
                point.x = i;
                point.z = j;

                bool isPointOnGround = false;

                for (int k = 0; k < colliderCount; k++)
                {
                    if (GridSettings.avoidUnbuildableTerrain && _colliderBuffer[k].gameObject.layer == (int)LayerEnum.UnbuildableTerrain)
                        continue;

                    if (IsPointInsideOtherBounds(_colliderBuffer[k].bounds.center, _colliderBuffer[k].bounds.extents, point))
                    {
                        isPointOnGround = true;
                        break;
                    }
                }

                if (isPointOnGround == false)
                    return false;
            }
        }
        return true;
    }

    private bool IsTouchingBottom(Bounds otherBounds)
    {
        float upperYOfStaticGameObject = otherBounds.center.y + otherBounds.extents.y;
        float lowerYOfMovingGameObject = _myTransform.position.y - PlacementCollider.bounds.extents.y;

        return Mathf.Abs(upperYOfStaticGameObject - lowerYOfMovingGameObject) <= _maxRangeTollerance;
    }

    private bool IsPointInsideOtherBounds(Vector3 center, Vector3 extents, Vector3 point)
    {
        float minX = center.x - extents.x - _maxRangeTollerance;
        float maxX = center.x + extents.x + _maxRangeTollerance;

        float minZ = center.z - extents.z - _maxRangeTollerance;
        float maxZ = center.z + extents.z + _maxRangeTollerance;

        if (point.x >= minX && point.x <= maxX && point.z >= minZ && point.z <= maxZ)
        {
            return true;
        }
        return false;
    }

    private bool IsPointInsideMyBounds(Bounds bounds, Vector3 point)
    {
        float minX = _myTransform.position.x - bounds.extents.x + _maxRangeTollerance;
        float maxX = _myTransform.position.x + bounds.extents.x - _maxRangeTollerance;

        float minZ = _myTransform.position.z - bounds.extents.z + _maxRangeTollerance;
        float maxZ = _myTransform.position.z + bounds.extents.z - _maxRangeTollerance;

        if (point.x > minX && point.x < maxX && point.z > minZ && point.z < maxZ)
        {
            return true;
        }
        return false;
    }

    private bool IsHigherOrLowerThan(GameObject otherObject)
    {
        var otherBounds = otherObject.GetComponent<Collider>().bounds;

        var upperYOfStaticGameObject = otherObject.transform.position.y + otherBounds.extents.y - _maxRangeTollerance;
        var lowerYOfStaticGameObject = otherObject.transform.position.y - otherBounds.extents.y + _maxRangeTollerance;

        var upperYOfMovingGameObject = _myTransform.position.y + PlacementCollider.bounds.extents.y - _maxRangeTollerance;
        var lowerYOfMovingGameObject = _myTransform.position.y - PlacementCollider.bounds.extents.y + _maxRangeTollerance;

        return lowerYOfMovingGameObject >= upperYOfStaticGameObject || upperYOfMovingGameObject <= lowerYOfStaticGameObject;
    }

    public List<int> DestroyOverlappedObjects()
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
        if (_brushObjectsHolder.IsHoldingObjects == false || _objectsInRange.Contains(other.gameObject) || other.gameObject.layer == (int)LayerEnum.UI
            || other.gameObject.layer == (int)LayerEnum.IgnoreRayCast)
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);

        StartCoroutine(CheckValidity());
    }

    private void OnTriggerExit(Collider other)
    {
        if (_brushObjectsHolder.IsHoldingObjects == false)
        {
            return;
        }

        _objectsInRange.Remove(other.gameObject);
    }

    public Vector3 GetRotatedColliderSize(Vector3 size)
    {
        var rotatedSize = Quaternion.Euler(transform.rotation.eulerAngles) * size;

        rotatedSize.x = Mathf.Abs(rotatedSize.x);
        rotatedSize.y = Mathf.Abs(rotatedSize.y);
        rotatedSize.z = Mathf.Abs(rotatedSize.z);

        return rotatedSize;
    }
}
