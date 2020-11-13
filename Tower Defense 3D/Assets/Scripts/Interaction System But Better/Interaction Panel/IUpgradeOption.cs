using UnityEngine;

public interface IUpgradeOption
{
    int Price { get; }
    Sprite Sprite { get; }

    DynamicTooltip DynamicTooltip { get; }
}