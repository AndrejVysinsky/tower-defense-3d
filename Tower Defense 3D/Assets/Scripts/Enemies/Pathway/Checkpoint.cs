using UnityEngine;

public class Checkpoint : MonoBehaviour, IGridObjectPositionUpdated, IGridObjectTryToRemove, IGridObjectRemoved, IGridObjectPlaced, IMapSaved, IMapLoaded
{
    private SaveableCheckpoint _saveableCheckpoint;
    private Pathway _pathway;

    private int checkpointNumber;

    private void Start()
    {
        _pathway = FindObjectOfType<Pathway>();
        checkpointNumber = _pathway.AddCheckpoint(gameObject);
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
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
        _pathway.UpdateCheckpointConnectorsPosition(position);

        return position;
    }

    public bool OnGridObjectTryToRemove()
    {
        return _pathway.IsCheckpointLast(gameObject);
    }

    public void OnGridObjectRemoved()
    {
        _pathway.LastCheckpointDestroyed();
    }

    public void OnGridObjectPlaced()
    {
        _pathway.CheckpointPlaced();
    }

    public void OnMapBeingSaved(MapSaveData mapSaveData)
    {
        var saveableBase = mapSaveData.GetSaveableObject(gameObject.GetInstanceID());

        _saveableCheckpoint = new SaveableCheckpoint(saveableBase, checkpointNumber);

        mapSaveData.UpdateSaveableObject(gameObject.GetInstanceID(), _saveableCheckpoint);
    }

    public void OnMapBeingLoaded(MapSaveData mapSaveData)
    {
        _saveableCheckpoint = (SaveableCheckpoint)mapSaveData.GetSaveableObject(gameObject.GetInstanceID());
    }
}