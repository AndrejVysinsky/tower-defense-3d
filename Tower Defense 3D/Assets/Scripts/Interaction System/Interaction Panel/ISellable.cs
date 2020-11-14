using UnityEngine;

public interface ISellable
{
    void Sell();

    SellableTooltip Tooltip { get; }
}