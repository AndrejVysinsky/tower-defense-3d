using UnityEngine;

public interface ISellable
{
    int Price { get; }

    void Sell();
}