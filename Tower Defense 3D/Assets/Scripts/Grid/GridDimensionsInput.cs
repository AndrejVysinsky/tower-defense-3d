using TMPro;
using UnityEngine;

public class GridDimensionsInput : MonoBehaviour
{
    [SerializeField] TMP_InputField inputSizeX;
    [SerializeField] TMP_InputField inputSizeZ;
    [SerializeField] GridController gridController;

    public void SetGridDimensions()
    {
        var inputTextSizeX = inputSizeX.text;
        var inputTextSizeZ = inputSizeZ.text;

        bool isInputValid = true;

        if (int.TryParse(inputTextSizeX, out int sizeX) == false)
        {
            Debug.LogError("Invalid value X.");
            isInputValid = false;
        }

        if (int.TryParse(inputTextSizeZ, out int sizeZ) == false)
        {
            Debug.LogError("Invalid value Z.");
            isInputValid = false;
        }

        if (sizeX == 0 || sizeX % 2 != 0 || sizeZ == 0 || sizeZ % 2 != 0)
        {
            Debug.Log("Size should be greater than 0 and even number.");
            isInputValid = false;
        }

        if (isInputValid)
        {
            gridController.SetGridDimensions(sizeX, sizeZ);
        }
    }
}