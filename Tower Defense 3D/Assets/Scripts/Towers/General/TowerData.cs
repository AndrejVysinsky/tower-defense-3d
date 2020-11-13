using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject, IUpgradeOption
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] int hitPoints;

    [SerializeField] int price;
    [SerializeField] float sellFactor;

    [SerializeField] float damage;
    [SerializeField] float attackDelay;
    [SerializeField] float radius;

    [SerializeField] Material material; //temporary solution

    [SerializeField] List<TowerData> nextUpgrades;

    public string Name => name;
    public Sprite Sprite => sprite;
    public int HitPoints => hitPoints;

    public int Price => price;

    public float Damage => damage;
    public float AttackDelay => attackDelay;
    public float Radius => radius;

    public Material Material => material;

    public List<TowerData> NextUpgrades => nextUpgrades;

    public DynamicTooltip DynamicTooltip => throw new System.NotImplementedException();

    public int GetSellValue()
    {
        return (int)(Price * sellFactor);
    }
}