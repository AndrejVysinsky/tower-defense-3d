using System.Reflection;
using TMPro;
using UnityEngine;

public class BrushSizeSlider : MonoBehaviour
{
    [SerializeField] GridController gridController;
    [SerializeField] TextMeshProUGUI textValue;

    [Header("Brush size range")]
    [SerializeField] int minSize;
    [SerializeField] int maxSize;

    private int _currentSize = 1;

    private void Start()
    {
        _currentSize = gridController.GridSettings.brushSize;
    }

    private void OnEnable()
    {
        gridController.OnBrushSizeChangedEvent.AddListener(UpdateTextValue);
    }

    private void OnDisable()
    {
        gridController.OnBrushSizeChangedEvent.RemoveListener(UpdateTextValue);
    }

    public void IncreaseValue()
    {
        if (_currentSize >= maxSize)
        {
            return;
        }

        OnBrushSizeChanged(_currentSize + 1);
    }

    public void DecreaseValue()
    {
        if (_currentSize <= minSize)
        {
            return;
        }

        OnBrushSizeChanged(_currentSize - 1);
    }

    private void OnBrushSizeChanged(int newSize)
    {
        _currentSize = gridController.OnBrushSizeChanged(newSize);

        UpdateTextValue(_currentSize);
    }

    private void UpdateTextValue(int value)
    {
        textValue.text = value.ToString();
    }
}