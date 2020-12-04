using System.Collections.Generic;
using UnityEngine;

public class PortalEnd : MonoBehaviour
{
    [SerializeField] List<GameObject> endPositions;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.OnPortalEndReached();
        }
    }

    public Vector3 GetRandomEndPosition()
    {
        return endPositions[Random.Range(0, endPositions.Count - 1)].transform.position;
    }
}