using UnityEngine;

[CreateAssetMenu(fileName = "Color Data", menuName = "UI/Color Data")]
public class ColorData : ScriptableObject
{
    [SerializeField] Color color;

    public Color Color => color;
}