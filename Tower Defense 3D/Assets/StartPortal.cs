using UnityEngine;

public class StartPortal : MonoBehaviour
{
    [SerializeField] GameObject timer;

    public void SetTimerActive(bool value)
    {
        timer.SetActive(value);
    }
}
