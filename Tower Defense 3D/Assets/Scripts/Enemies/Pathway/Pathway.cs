using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] MapSaveManager map;
    [SerializeField] GameObject lineRendererPrefab;

    private List<GameObject> _checkpoints = new List<GameObject>();
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    private bool _canUpdatePosition;

    public int NumberOfCheckpoints => _checkpoints.Count;

    public Vector3 GetPositionOfLastCheckpoint()
    {
        if (_checkpoints.Count == 0)
            return Vector3.zero;

        if (_checkpoints.Count < 2)
            return _checkpoints[0].transform.position;

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
        //remove arrows
    }

    public int AddCheckpoint(GameObject checkPoint)
    {
        _checkpoints.Add(checkPoint);

        if (_checkpoints.Count > 1)
        {
            var lineObject = Instantiate(lineRendererPrefab, map.transform);
            _lineRenderers.Add(lineObject.GetComponent<LineRenderer>());
        }

        _canUpdatePosition = true;

        return _checkpoints.Count;
    }

    public void RemovePath()
    {
        _checkpoints.ForEach(x => Destroy(x));
        _checkpoints.Clear();
    }

    public void ConfirmPathCreation()
    {
        //make first checkpoint as portal start

        map.ObjectRemoved(_checkpoints[0].GetInstanceID());

        //var startCheckpoint = Instantiate(startCheckpointPrefab, map.transform);
        //startCheckpoint.transform.localPosition = _checkpoints[0].transform.localPosition;

        Destroy(_checkpoints[0]);

        //_checkpoints[0] = startCheckpoint;

        ///map.ObjectPlaced(_checkpoints[0], startCheckpointPrefab);
    }

    public void CheckpointPlaced()
    {
        _canUpdatePosition = false;


    }
}

