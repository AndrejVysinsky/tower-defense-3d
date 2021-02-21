using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrushSizeSlider : MonoBehaviour
{
    [SerializeField] GridController gridController;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textValue;

    private void Start()
    {
        var range = typeof(GridSettings).GetField(nameof(GridSettings.brushSize)).GetCustomAttribute<RangeAttribute>();

        slider.minValue = range.min;
        slider.maxValue = range.max;
        slider.value = gridController.GridSettings.brushSize;
    }

    private void OnEnable()
    {
        gridController.OnBrushSizeChangedEvent.AddListener(UpdateSliderValue);
    }

    private void OnDisable()
    {
        gridController.OnBrushSizeChangedEvent.RemoveListener(UpdateSliderValue);
    }

    public void OnValueChanged(float value)
    {
        var newValue = gridController.OnBrushSizeChanged((int)value);

        textValue.text = newValue.ToString();
    }

    public void UpdateSliderValue(int value)
    {
        textValue.text = value.ToString();
        slider.value = value;
    }
}