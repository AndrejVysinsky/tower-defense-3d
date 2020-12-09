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
    
    private readonly float _maxRangeTollerance = 0.05f;

    public bool IsOverlapping => _overlappedObjects.Count > 0;
    public bool IsOnGround { get; private set; }
    public BoxCollider PlacementCollider { get; private set; }

    public GridSettings GridSettings { get; set; }

    private BrushObjectsHolder _brushObjectsHolder;

    private void Awake()
    {
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

    public void ResizeCollider(Vector3 size)
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
        if (transform.position == position)
            return;

        transform.position = position;

        StartCoroutine(CheckValidity());
    }

    public void Rotate(Vector3 direction, float angle)
    {
        transform.Rotate(direction, angle);

        StartCoroutine(CheckValidity());
    }

    private IEnumerator CheckValidity()
    {
        yield return new WaitForFixedUpdate();

        CheckOverlap();

        IsOnGround = IsWholeObjectOnGround();

        bool isValidPlacement = IsOverlapping == false && IsOnGround;

        _validityIndicators.ForEach(x => x.SetMaterial(isValidPlacement));
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
        var myColliderSize = GetRotatedColliderSize(PlacementCollider.size);

        float minX = transform.position.x - myColliderSize.x / 2;
        float maxX = transform.position.x + myColliderSize.x / 2;

        float minZ = transform.position.z - myColliderSize.z / 2;
        float maxZ = transform.position.z + myColliderSize.z / 2;

        var point = Vector3.zero;
        point.y = transform.position.y - myColliderSize.y / 2;

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
        if (_brushObjectsHolder.IsHoldingObjects == false || _objectsInRange.Contains(other.gameObject) || other.gameObject.layer == (int)LayerEnum.UI
            || other.gameObject.layer == (int)LayerEnum.IgnoreRayCast)
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);

        CheckValidity();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_brushObjectsHolder.IsHoldingObjects == false)
        {
            return;
        }

        _objectsInRange.Remove(other.gameObject);
    }

    private Vector3 GetRotatedColliderSize(Vector3 size)
    {
        var rotatedSize = Quaternion.Euler(transform.rotation.eulerAngles) * size;

        rotatedSize.x = Mathf.Abs(rotatedSize.x);
        rotatedSize.y = Mathf.Abs(rotatedSize.y);
        rotatedSize.z = Mathf.Abs(rotatedSize.z);

        return rotatedSize;
    }
}
