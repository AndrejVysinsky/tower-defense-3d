using UnityEngine;

public abstract class TooltipBase : ScriptableObject
{
    [SerializeField] [TextArea(3, 10)] string description;

    public string Description => description;

    public abstract string GetTooltipText();
}