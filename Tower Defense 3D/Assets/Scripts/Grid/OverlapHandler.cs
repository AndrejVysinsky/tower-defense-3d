using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverlapHandler : MonoBehaviour
{
    private BoxCollider _boxCollider;
    private GameObject _parentObject;

    private List<GameObject> _objectsInRange;
    private List<GameObject> _overlappedObjects;
    
    private readonly float _maxRangeTollerance = 0.05f;

    public bool IsOverlapping { get; private set; }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();

        _objectsInRange = new List<GameObject>();
        _overlappedObjects = new List<GameObject>();
    }

    public void RegisterParent(GameObject parent)
    {
        _parentObject = parent;
    }

    public void ResizeCollider(Vector3 size)
    {
        _boxCollider.size = size;

        //to register OnTriggerEnter
        _boxCollider.enabled = false;
        _boxCollider.enabled = true;
    }

    public void SetPosition(Vector3 position)
    {
        if (transform.position == position)
            return;

        transform.position = position;

        IsOverlapping = CheckOverlap();
    }

    private bool CheckOverlap()
    {
        var isOverlapping = false;

        var bounds = _boxCollider.bounds;

        _overlappedObjects.Clear();
        _objectsInRange.RemoveAll(x => x == null);

        foreach (var overlappedObject in _objectsInRange)
        {
            if (IsHigherOrLowerThan(overlappedObject))
            {
                continue;
            }

            var point = overlappedObject.GetComponent<Collider>().ClosestPoint(transform.position);

            if (IsPointInsideBounds(bounds, point))
            {                
                isOverlapping = true;
                _overlappedObjects.Add(overlappedObject);
            }
        }
        return isOverlapping;
    }

    private bool IsPointInsideBounds(Bounds bounds, Vector3 point)
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

        var upperYOfMovingGameObject = transform.position.y + _boxCollider.bounds.extents.y - _maxRangeTollerance;
        var lowerYOfMovingGameObject = transform.position.y - _boxCollider.bounds.extents.y + _maxRangeTollerance;

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
        if (other.gameObject.CompareTag("Grid") || other.gameObject == _parentObject)
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);

        CheckOverlap();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Grid") || other.gameObject == _parentObject)
        {
            return;
        }

        _objectsInRange.Remove(other.gameObject);
    }

    public void ParentDestroyed()
    {
        _objectsInRange.ForEach(x => x.GetComponent<MeshRenderer>().enabled = true);
        _objectsInRange.Clear();
    }
}
