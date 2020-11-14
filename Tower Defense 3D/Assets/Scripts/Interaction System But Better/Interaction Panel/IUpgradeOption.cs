using UnityEngine;

public interface IUpgradeOption
{
    int Price { get; }
    Sprite Sprite { get; }
    BuyableTooltip Tooltip { get; }
}