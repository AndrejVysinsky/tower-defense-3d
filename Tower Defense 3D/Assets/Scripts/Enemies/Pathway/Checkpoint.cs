using UnityEngine;

public class Checkpoint : MonoBehaviour, 
    IGridObjectPositionUpdated, IGridObjectRemoved, IGridObjectPlaced, IGridObjectInitialized, 
    IGridObjectTryToRemove, IGridObjectTryChangeBrushSize, IGridObjectTryToReplace,
    IMapSaved, IMapLoaded
{
    [SerializeField] MeshRenderer myMeshRenderer;

    public SaveableCheckpoint SaveableCheckpoint { get; private set; }

    public int CheckpointNumber { get; set; }
    public bool IsPlaced { get; private set; }

    private Pathway _pathway;
    private Pathway Pathway 
    { 
        get 
        {
            if (_pathway == null)
            {
                _pathway = FindObjectOfType<Pathway>();
            }
            return _pathway;
        }
    }

    private void Awake()
    {
        IsPlaced = false;
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
        if (Pathway.NumberOfCheckpoints == 0)
            return position;

        var lastPosition = Pathway.GetPositionOfLastCheckpoint();

        if (Mathf.Abs(lastPosition.x - position.x) >= Mathf.Abs(lastPosition.z - position.z))
        {
            position.z = lastPosition.z;
        }
        else
        {
            position.x = lastPosition.x;
        }
        Pathway.UpdatePathwayLastPosition(position);

        return position;
    }

    public bool OnGridObjectTryToRemove()
    {
        return Pathway.IsCheckpointLast(gameObject);
    }

    public bool OnGridObjectTryToReplace()
    {
        return false;
    }

    public void OnGridObjectRemoved()
    {
        Pathway.LastCheckpointDestroyed();
    }

    public void OnGridObjectPlaced()
    {
        IsPlaced = true;
        CheckpointNumber = Pathway.CheckpointPlaced();
    }

    public void OnGridObjectInitialized()
    {
        Pathway.AddCheckpoint(gameObject);
    }

    public void OnMapBeingSaved(MapSaveData mapSaveData)
    {
        var saveableBase = mapSaveData.GetSaveableObject(gameObject.GetInstanceID());

        SaveableCheckpoint = new SaveableCheckpoint(saveableBase, CheckpointNumber);

        mapSaveData.UpdateSaveableObject(gameObject.GetInstanceID(), SaveableCheckpoint);
    }

    public void OnMapBeingLoaded(MapSaveData mapSaveData)
    {
        SaveableCheckpoint = (SaveableCheckpoint)mapSaveData.GetSaveableObject(gameObject.GetInstanceID());

        CheckpointNumber = SaveableCheckpoint.checkpointNumber;

        Pathway.AddCheckpoint(gameObject);
        IsPlaced = true;
        Pathway.CheckpointPlaced();
    }

    public bool OnGridObjectTryChangeBrushSize(int newBrushSize)
    {
        return false;
    }

    public void SetMaterial(Material material)
    {
        myMeshRenderer.material = material;
    }
}