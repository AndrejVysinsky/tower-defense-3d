using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalette : MonoBehaviour
{
    [Header("Main Color")]
    [SerializeField] ColorData mainColor;
    [SerializeField] List<Image> mainParts;

    [Header("Secondary Color")]
    [SerializeField] ColorData secondaryColor;
    [SerializeField] List<Image> secondaryParts;

    [Header("Buttons")]
    [SerializeField] ColorData buttonColor;
    [SerializeField] List<Image> buttons;

    [Header("Inputs")]
    [SerializeField] ColorData inputColor;
    [SerializeField] List<Image> inputs;

    private void OnValidate()
    {
        UpdateColor(mainParts, mainColor);
        UpdateColor(secondaryParts, secondaryColor);
        UpdateColor(buttons, buttonColor);
        UpdateColor(inputs, inputColor);
    }

    private void UpdateColor(List<Image> parts, ColorData newColor)
    {
        if (parts == null)
            return;

        foreach (var part in parts)
        {
            if (part == null)
                continue;

            part.color = newColor.Color;
        }
    }
}
