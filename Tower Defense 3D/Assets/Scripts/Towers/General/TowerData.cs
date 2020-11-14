using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject, IUpgradeOption
{
    [SerializeField] BuyableTooltip tooltip; 
    [SerializeField] Sprite sprite;
    [SerializeField] int hitPoints;

    [SerializeField] int price;
    [SerializeField] float sellFactor;

    [SerializeField] float damage;
    [SerializeField] float attackDelay;
    [SerializeField] float radius;

    [SerializeField] Material material; //temporary solution

    [SerializeField] List<TowerData> nextUpgrades;

    public string Name => tooltip.Header;
    public Sprite Sprite => sprite;
    public int HitPoints => hitPoints;

    public int Price => price;

    public float Damage => damage;
    public float AttackDelay => attackDelay;
    public float Radius => radius;

    public Material Material => material;

    public List<TowerData> NextUpgrades => nextUpgrades;

    public BuyableTooltip Tooltip
    {
        get
        {
            tooltip.Price = Price;
            return tooltip;
        }
    }

    public int GetSellValue()
    {
        return (int)(Price * sellFactor);
    }
}