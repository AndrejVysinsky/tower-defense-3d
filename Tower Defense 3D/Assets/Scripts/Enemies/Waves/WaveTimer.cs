using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    private float _time;
    private float _remainingTime;

    public event Action OnTimerSkipped;

    private void Update()
    {
        _remainingTime -= Time.deltaTime;
        UpdateTime(_remainingTime);

        if (_remainingTime <= 0)
        {
            timeText.text = "---";
        }
    }

    private void UpdateTime(float remainingTime)
    {
        int seconds = Mathf.RoundToInt(remainingTime);
        timeText.text = seconds.ToString();
        //timeImage.fillAmount = remainingTime / _time;
    }

    public void RefreshTimer(float time)
    {
        _time = time;
        _remainingTime = time;
        UpdateTime(_time);
    }

    public void SkipTimer()
    {
        OnTimerSkipped?.Invoke();

        _remainingTime = 0;
    }
}
