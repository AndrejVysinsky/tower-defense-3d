using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathway : MonoBehaviour, IMapCleared
{
    [SerializeField] MapSaveManager map;
    [SerializeField] GameObject lineRendererPrefab;

    private List<GameObject> _checkpoints = new List<GameObject>();
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    private bool _canUpdatePosition;

    public int NumberOfCheckpoints => _checkpoints.Count;

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

    public void UpdateCheckpointConnectorsPosition(Vector3 newCheckpointPosition)
    {
        if (_checkpoints.Count < 2)
            return;

        if (_canUpdatePosition == false)
            return;

        var lastCheckpointPosition = _checkpoints[_checkpoints.Count - 2].transform.position;

        var lineRenderer = _lineRenderers[_lineRenderers.Count - 1];

        lineRenderer.SetPosition(0, lastCheckpointPosition);
        lineRenderer.SetPosition(1, newCheckpointPosition);
    }    

    public bool IsCheckpointLast(GameObject checkpoint)
    {
        return _checkpoints[_checkpoints.Count - 1] == checkpoint;
    }

    public void LastCheckpointDestroyed()
    {
        _checkpoints.RemoveAt(_checkpoints.Count - 1);

        if (_lineRenderers.Count == 0)
            return;

        Destroy(_lineRenderers[_lineRenderers.Count - 1].gameObject);
        _lineRenderers.RemoveAt(_lineRenderers.Count - 1);

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

        if (_checkpoints.Count > 1)
        {
            InstantiateRequiredNumberOfLineRenderers();
            var lineRenderer = _lineRenderers[_lineRenderers.Count - 1];

            lineRenderer.SetPosition(0, _checkpoints[_checkpoints.Count - 2].transform.position);
            lineRenderer.SetPosition(1, _checkpoints[_checkpoints.Count - 1].transform.position);
        }

        _canUpdatePosition = true;
    }

    public void LoadCheckpoint(GameObject checkpoint)
    {
        _checkpoints.Add(checkpoint);
        _checkpoints = _checkpoints.OrderBy(c => c.GetComponent<Checkpoint>().CheckpointNumber).ToList();

        InstantiateRequiredNumberOfLineRenderers();

        for (int i = 0; i < _checkpoints.Count - 1; i++)
        {
            var currentCheckpoint = _checkpoints[i].GetComponent<Checkpoint>();
            var nextCheckpoint = _checkpoints[i + 1].GetComponent<Checkpoint>();

            if (nextCheckpoint.CheckpointNumber - currentCheckpoint.CheckpointNumber == 1)
            {
                _lineRenderers[i].SetPosition(0, currentCheckpoint.transform.position);
                _lineRenderers[i].SetPosition(1, nextCheckpoint.transform.position);
            }
        }
    }

    private void InstantiateRequiredNumberOfLineRenderers()
    {
        while (_checkpoints.Count - 1 > _lineRenderers.Count)
        {
            var lineObject = Instantiate(lineRendererPrefab, map.transform);

            var lineRenderer = lineObject.GetComponent<LineRenderer>();

            _lineRenderers.Add(lineRenderer);
        }
    }

    public int CheckpointPlaced()
    {
        _canUpdatePosition = false;

        return _checkpoints.Count;
    }

    public void OnMapBeingCleared()
    {
        _checkpoints.Clear();

        _lineRenderers.ForEach(line => Destroy(line.gameObject));
        _lineRenderers.Clear();
    }
}

