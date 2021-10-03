using Mirror;
using System;
using TMPro;
using UnityEngine;

public class WaveTimer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    private float _time;
    private float _remainingTime;

    public float RemainingTime => _remainingTime;

    public event Action OnTimerSkipped;

    [ServerCallback]
    private void Update()
    {
        if (_time == 0)
            return;

        _remainingTime -= Time.deltaTime;
        RpcUpdateTime(_remainingTime);
    }

    [ClientRpc]
    private void RpcUpdateTime(float remainingTime)
    {
        if (remainingTime <= 0)
        {
            timeText.text = "---";
        }
        else
        {
            int seconds = Mathf.RoundToInt(remainingTime);
            timeText.text = seconds.ToString();
        }
    }

    [Server]
    public void RefreshTimer(float time)
    {
        _time = time;
        _remainingTime = time;
        RpcUpdateTime(_time);
    }

    [Server]
    public void SkipTimer()
    {
        OnTimerSkipped?.Invoke();

        _remainingTime = 0;
    }
}
