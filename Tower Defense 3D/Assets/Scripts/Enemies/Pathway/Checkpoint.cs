using UnityEngine;

public class Checkpoint : MonoBehaviour, IGridObjectPositionUpdated, IGridObjectPlacementCanceled, IGridObjectTryToRemove, IGridObjectRemoved
{
    private Pathway _pathway;

    private void Start()
    {
        _pathway = FindObjectOfType<Pathway>();
        _pathway.AddCheckpoint(gameObject);
    }

    public Vector3 OnGridObjectPositionUpdated(Vector3 position)
    {
        if (_pathway == null)
        {
            _pathway = FindObjectOfType<Pathway>();
        }

        if (_pathway.NumberOfCheckpoints == 0)
            return position;

        var lastPosition = _pathway.GetPositionOfLastCheckpoint();

        if (Mathf.Abs(lastPosition.x - position.x) >= Mathf.Abs(lastPosition.z - position.z))
        {
            position.z = lastPosition.z;
        }
        else
        {
            position.x = lastPosition.x;
        }
        return position;
    }

    public void OnGridObjectPlacementCanceled()
    {
        _pathway.RemovePath();
    }

    public bool OnGridObjectTryToRemove()
    {
        return _pathway.IsCheckpointLast(gameObject);
    }
}