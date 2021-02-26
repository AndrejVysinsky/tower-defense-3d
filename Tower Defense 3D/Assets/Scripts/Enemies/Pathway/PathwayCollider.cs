using UnityEngine;

/// <summary>
/// Collider between pathway checkpoints
/// </summary>
public class PathwayCollider : MonoBehaviour, IGridObjectTryToRemove, IGridObjectTryToReplace
{
    public bool OnGridObjectTryToRemove()
    {
        return false;
    }

    public bool OnGridObjectTryToReplace()
    {
        return false;
    }
}