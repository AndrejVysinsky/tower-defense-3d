using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementValidator : MonoBehaviour
{
    [SerializeField] Material validPlacementMaterial;
    [SerializeField] Material invalidPlacementMaterial;

    private List<ValidityIndicator> _validityIndicators;

    private Collider[] _colliderBuffer;
    private Transform _myTransform;
    private readonly float _maxRangeTollerance = 0.05f;

    public List<Collider> OverlappedColliders { get; private set; }
    public bool IsOverlapping => OverlappedColliders.Count > 0;
    public bool IsOnGround { get; private set; }
    public BoxCollider PlacementCollider { get; private set; }

    public GridSettings GridSettings { get; set; }

    private void Awake()
    {
        _myTransform = transform;

        _validityIndicators = new List<ValidityIndicator>();

        PlacementCollider = GetComponent<BoxCollider>();

        _colliderBuffer = new Collider[500];
        OverlappedColliders = new List<Collider>();
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
        for (int i = 0; i < OverlappedColliders.Count; i++)
        {
            if (OverlappedColliders[i].gameObject.activeSelf == false)
                OverlappedColliders[i].gameObject.SetActive(true);
        }

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

    public void RefreshCollider()
    {
        PlacementCollider.enabled = false;
        PlacementCollider.enabled = true;

        CheckValidity();
    }

    public void SetPosition(Vector3 position, GameObject objectToPlacePrefab)
    {
        if (_myTransform.position == position)
            return;

        if (objectToPlacePrefab.TryGetComponent(out IGridObjectPositionUpdated gridEvent))
        {
            position = gridEvent.OnGridObjectPositionUpdated(position);
        }

        _myTransform.position = position;

        CheckValidity();
    }

    public void Rotate(Vector3 direction, float angle)
    {
        _myTransform.Rotate(direction, angle);

        CheckValidity();
    }

    private void CheckValidity()
    {
        CheckOverlap();

        IsOnGround = IsWholeObjectOnGround();

        bool isValidPlacement = IsOverlapping == false && IsOnGround;

        if (GridSettings.replaceOnCollision)
        {
            for (int i = 0; i < _validityIndicators.Count; i++)
            {
                _validityIndicators[i].SwitchBackObjectMaterials();
            }

            //hide colliding objects
            foreach (var overlappedCollider in OverlappedColliders)
            {
                overlappedCollider.gameObject.SetActive(false);
            }
        }

        if (GridSettings.replaceOnCollision == false || IsOnGround == false)
        {
            for (int i = 0; i < _validityIndicators.Count; i++)
            {
                _validityIndicators[i].SetMaterial(isValidPlacement);
            }
        }
    }

    private void CheckOverlap()
    {
        var bounds = PlacementCollider.bounds;

        //show colliding objects from last update
        foreach (var overlappedCollider in OverlappedColliders)
        {
            //sometimes null, no idea why
            if (overlappedCollider == null)
                continue;

            if (overlappedCollider.gameObject.activeSelf == false)
                overlappedCollider.gameObject.SetActive(true);
        }

        OverlappedColliders.Clear();

        /*
            smaller extents in all directions so Overlap does not contain colliders that only touch
        */
        var modifiedExtents = bounds.extents;
        modifiedExtents.y -= 0.01f;
        modifiedExtents.x -= 0.01f;
        modifiedExtents.z -= 0.01f;

        int bufferSize = Physics.OverlapBoxNonAlloc(transform.position, modifiedExtents, _colliderBuffer, Quaternion.identity, Physics.AllLayers ^ Physics.IgnoreRaycastLayer);

        if (bufferSize == 1 && _colliderBuffer[0].gameObject.name == "Grid Base")
        {
            return;
        }

        for (int i = 0; i < bufferSize; i++)
        {
            if (_colliderBuffer[i].gameObject.name == "Grid Base")
                continue;

            OverlappedColliders.Add(_colliderBuffer[i]);
        }
    }

    private bool IsWholeObjectOnGround()
    {
        Vector3 bottomCenter = _myTransform.position;
        bottomCenter.y -= PlacementCollider.bounds.extents.y;

        Vector3 halfExtents = PlacementCollider.bounds.extents;
        halfExtents.x -= 0.01f;
        halfExtents.z -= 0.01f;
        halfExtents.y = 0.01f;

        int bufferSize = Physics.OverlapBoxNonAlloc(bottomCenter, halfExtents, _colliderBuffer, Quaternion.identity, Physics.AllLayers ^ Physics.IgnoreRaycastLayer);

        if (bufferSize == 0)
            return false;

        //check their range
        var myColliderSize = PlacementCollider.bounds.size;

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

                for (int k = 0; k < bufferSize; k++)
                {
                    if (GridSettings.avoidUnbuildableTerrain && _colliderBuffer[k].gameObject.layer == (int)LayerEnum.UnbuildableTerrain)
                        continue;

                    if (IsPointInsideOtherBounds(_colliderBuffer[k].bounds, point))
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

    private bool IsPointInsideOtherBounds(Bounds others, Vector3 point)
    {
        var center = others.center;
        var extents = others.extents;

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

    public List<int> DestroyOverlappedObjects()
    {
        List<int> result = new List<int>();
        result.AddRange(OverlappedColliders.Select(collider => collider.gameObject.GetInstanceID()));

        OverlappedColliders.ForEach(collider => Destroy(collider.gameObject));
        OverlappedColliders.Clear();

        return result;
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
