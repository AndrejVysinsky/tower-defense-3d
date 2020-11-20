using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpButton : MonoBehaviour
{
    [Serializable]
    public class SpeedOption
    {
        [SerializeField] Color speedColor;
        [SerializeField] float speed;

        public Color SpeedColor => speedColor;
        public float Speed => speed;
    }

    [SerializeField] List<SpeedOption> speedOptions;

    [SerializeField] Image imageBackground;
    [SerializeField] TextMeshProUGUI speedText;

    private int _speedIndex = 0;

    private void Awake()
    {
        SetSpeedOption(_speedIndex);
    }

    public void ToggleSpeedUp()
    {
        _speedIndex++;

        if (_speedIndex >= speedOptions.Count)
        {
            _speedIndex = 0;
        }

        SetSpeedOption(_speedIndex);
    }

    private void SetSpeedOption(int index)
    {
        float speed = speedOptions[_speedIndex].Speed;

        Time.timeScale = speed;
        speedText.text = speed + "x";

        Color color = speedOptions[_speedIndex].SpeedColor;

        //speedText.color = color;
        imageBackground.color = color;
    }
}
