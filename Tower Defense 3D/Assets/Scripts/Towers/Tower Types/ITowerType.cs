using UnityEngine;

public interface ITowerType
{
    int Level { get; }
    TowerData TowerData { get; }

    void Shoot(GameObject target);
    void Upgrade();
    void Sell();
}