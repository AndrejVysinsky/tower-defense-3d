using UnityEngine;

public abstract class TooltipBase : ScriptableObject
{
    [SerializeField] string description;

    public string Description => description;
}