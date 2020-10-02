using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] GameObject[] wayPoints;

    public Vector3 GetWayPointAtIndex(int index)
    {
        return wayPoints[index].transform.position;
    }

    public int GetNumberOfWayPoints()
    {
        return wayPoints.Length;
    }
}
