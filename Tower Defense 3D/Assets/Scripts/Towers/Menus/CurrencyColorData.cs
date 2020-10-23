using UnityEngine;

[CreateAssetMenu(fileName = "Currency Color Data", menuName = "Data/Currency Color Data")]
public class CurrencyColorData : ScriptableObject
{
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    public Color ActiveColor => activeColor;
    public Color InactiveColor => inactiveColor;
}