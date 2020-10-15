using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    private List<Vector3> _wayPoints;

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    void Start()
    {
        _wayPoints = new List<Vector3>();

        var start = GameObject.FindGameObjectWithTag("Start");
        var end = GameObject.FindGameObjectWithTag("End");

        _startPosition = start.transform.position;
        _endPosition = end.transform.position;

        start.GetComponent<StartPortal>().SetTimerActive(true);

        _wayPoints.Add(start.transform.position);
        _wayPoints.Add(end.transform.position);
    }

    public Vector3 GetWayPointAtIndex(int index)
    {
        return _wayPoints[index];
    }

    public int GetNumberOfWayPoints()
    {
        return _wayPoints.Count;
    }

    public WaveTimer GetTimer()
    {
        var start = GameObject.FindGameObjectWithTag("Start");

        return start.GetComponentInChildren<WaveTimer>();
    }

    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public Vector3 GetEndPosition()
    {
        return _endPosition;
    }
}
