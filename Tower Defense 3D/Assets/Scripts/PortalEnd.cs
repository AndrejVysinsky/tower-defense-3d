using UnityEngine;

public class PortalEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.OnPortalEndReached();
        }
    }
}