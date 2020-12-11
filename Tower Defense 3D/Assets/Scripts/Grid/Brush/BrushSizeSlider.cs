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

    public void OnValueChanged(float value)
    {
        int brushSize = (int)value;

        textValue.text = brushSize.ToString();

        gridController.OnBrushSizeChanged(brushSize);
    }
}