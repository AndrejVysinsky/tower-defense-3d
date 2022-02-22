using UnityEngine;

/// <summary>
/// Collider between pathway checkpoints
/// </summary>
public class PathwayCollider : MonoBehaviour, IGridObjectTryToRemove, IGridObjectTryToReplace
{
    public void PlaceColliderBetweenPositions(Vector3 startPosition, Vector3 endPosition)
    {
        transform.position = (startPosition + endPosition) / 2;

        var localScale = transform.localScale;
        localScale.x = Mathf.Abs(startPosition.x - endPosition.x);
        localScale.z = Mathf.Abs(startPosition.z - endPosition.z);

        if (localScale.x == 0)
        {
            localScale.x = 0.5f;
            localScale.z = (localScale.z - 2) / 2;
        }
        else
        {
            localScale.z = 0.5f;
            localScale.x = (localScale.x - 2) / 2;
        }

        transform.localScale = localScale;
    }

    public bool OnGridObjectTryToRemove()
    {
        return false;
    }

    public bool OnGridObjectTryToReplace()
    {
        return false;
    }
}