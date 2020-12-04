using System.Collections.Generic;
using UnityEngine;

public class PortalStart : MonoBehaviour
{
    [SerializeField] GameObject timer;
    [SerializeField] List<GameObject> startPositions;

    public WaveTimer GetTimer()
    {
        if (timer.activeSelf == false)
            timer.SetActive(true);

        return timer.GetComponent<WaveTimer>();
    }

    public Vector3 GetRandomStartPosition()
    {
        return startPositions[Random.Range(0, startPositions.Count - 1)].transform.position;
    }
}
