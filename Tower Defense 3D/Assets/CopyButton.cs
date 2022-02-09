using TMPro;
using UnityEngine;

public class CopyButton : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = input.text;
    }
}
