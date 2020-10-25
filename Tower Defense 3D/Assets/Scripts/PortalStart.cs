using UnityEngine;

public class PortalStart : MonoBehaviour
{
    [SerializeField] GameObject timer;

    public WaveTimer GetTimer()
    {
        if (timer.activeSelf == false)
            timer.SetActive(true);

        return timer.GetComponent<WaveTimer>();
    }
}
