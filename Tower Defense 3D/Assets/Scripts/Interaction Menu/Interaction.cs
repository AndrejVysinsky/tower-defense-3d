using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Interaction
{
    [SerializeField] Sprite sprite;
    [SerializeField] UnityEvent unityAction;
    [SerializeField] DynamicTooltip tooltip;
    [SerializeField] bool tooltipEnabled;

    public Sprite InteractionSprite => sprite;
    public UnityEvent UnityAction => unityAction;
    public DynamicTooltip Tooltip => tooltip;
    public bool TooltipEnabled => tooltipEnabled;
}
