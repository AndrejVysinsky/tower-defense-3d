using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] MapSaveManager map;
    [SerializeField] GameObject startCheckpointPrefab;

    private List<GameObject> _checkpoints = new List<GameObject>();

    public int NumberOfCheckpoints => _checkpoints.Count;
        
    public Vector3 GetPositionOfLastCheckpoint()
    {
        if (_checkpoints.Count == 0)
            return Vector3.zero;

        if (_checkpoints.Count < 2)
            return _checkpoints[0].transform.position;

        return _checkpoints[_checkpoints.Count - 2].transform.position;
    }

    public bool IsCheckpointLast(GameObject checkpoint)
    {
        return _checkpoints[_checkpoints.Count - 1] == checkpoint;
    }

    public void AddCheckpoint(GameObject checkPoint)
    {
        _checkpoints.Add(checkPoint);
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

        var startCheckpoint = Instantiate(startCheckpointPrefab, map.transform);
        startCheckpoint.transform.localPosition = _checkpoints[0].transform.localPosition;

        Destroy(_checkpoints[0]);

        _checkpoints[0] = startCheckpoint;

        map.ObjectPlaced(_checkpoints[0], startCheckpointPrefab);
    }
}

