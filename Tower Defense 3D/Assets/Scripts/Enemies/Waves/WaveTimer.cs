using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveTimer : MonoBehaviour
{
    [SerializeField] GameObject timerContainer;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Image timeImage;

    private float _time;
    private float _remainingTime;

    public event Action OnTimerSkipped;

    private void Update()
    {
        if (timerContainer.activeSelf == false)
            return;

        _remainingTime -= Time.deltaTime;
        UpdateTime(_remainingTime);

        if (_remainingTime <= 0)
        {
            timerContainer.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    private void UpdateTime(float remainingTime)
    {
        int seconds = Mathf.RoundToInt(remainingTime);
        timeText.text = seconds.ToString();
        timeImage.fillAmount = remainingTime / _time;
    }

    public void RefreshTimer(float time)
    {
        timerContainer.SetActive(true);
        GetComponent<Collider>().enabled = true;

        _time = time;
        _remainingTime = time;
        UpdateTime(_time);
    }

    private void OnMouseDown()
    {
        OnTimerSkipped?.Invoke();

        timerContainer.SetActive(false);
        GetComponent<Collider>().enabled = false;
    } 
}
