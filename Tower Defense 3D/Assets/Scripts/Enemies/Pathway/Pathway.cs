using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathway : MonoBehaviour, IMapCleared
{
    [SerializeField] MapSaveManager map;
    [SerializeField] GameObject lineRendererPrefab;

    private List<GameObject> _checkpoints = new List<GameObject>();
    private LineRenderer _pathwayLineRenderer;

    private bool _canUpdatePosition;

    public int NumberOfCheckpoints => _checkpoints.Count;

    private void Start()
    {
        if (_pathwayLineRenderer == null)
        {
            var pathwayRendererObject = Instantiate(lineRendererPrefab, map.transform);
            _pathwayLineRenderer = pathwayRendererObject.GetComponent<LineRenderer>();
            _pathwayLineRenderer.positionCount = 0;
        }

        
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public Vector3 GetPositionOfLastCheckpoint()
    {
        if (_checkpoints.Count == 0)
            return Vector3.zero;

        if (_checkpoints.Count < 2)
            return _checkpoints[0].transform.position;

        if (_canUpdatePosition == false)
            return _checkpoints[_checkpoints.Count - 1].transform.position;

        return _checkpoints[_checkpoints.Count - 2].transform.position;
    }

    public void UpdatePathwayLastPosition(Vector3 newCheckpointPosition)
    {
        if (_checkpoints.Count < 2)
            return;

        if (_canUpdatePosition == false)
            return;

        _pathwayLineRenderer.SetPosition(_pathwayLineRenderer.positionCount - 1, newCheckpointPosition);
    }

    public bool IsCheckpointLast(GameObject checkpoint)
    {
        return _checkpoints[_checkpoints.Count - 1] == checkpoint;
    }

    public void LastCheckpointDestroyed()
    {
        _checkpoints.RemoveAt(_checkpoints.Count - 1);

        if (_pathwayLineRenderer.positionCount > 0)
            _pathwayLineRenderer.positionCount--;

        _canUpdatePosition = false;
    }

    public void AddCheckpoint(GameObject checkpoint)
    {
        _checkpoints.Add(checkpoint);

        var checkpointScript = checkpoint.GetComponent<Checkpoint>();
        if (checkpointScript.CheckpointNumber == 0)
        {
            checkpointScript.CheckpointNumber = _checkpoints.Count;
        }

        _checkpoints = _checkpoints.OrderBy(c => c.GetComponent<Checkpoint>().CheckpointNumber).ToList();

        if (_pathwayLineRenderer == null)
        {
            var pathwayRendererObject = Instantiate(lineRendererPrefab, map.transform);
            _pathwayLineRenderer = pathwayRendererObject.GetComponent<LineRenderer>();
        }

        _pathwayLineRenderer.positionCount = _checkpoints.Count;
        _pathwayLineRenderer.SetPositions(_checkpoints.Select(x => x.transform.position).ToArray());

        _canUpdatePosition = true;
    }

    public int CheckpointPlaced()
    {
        _canUpdatePosition = false;

        return _checkpoints.Count;
    }

    public void OnMapBeingCleared()
    {
        _checkpoints.Clear();
    }
}

