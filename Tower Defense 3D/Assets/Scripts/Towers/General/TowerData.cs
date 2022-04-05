using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject, IUpgradeOption
{
    [SerializeField] BuyableTooltip tooltip;
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] int hitPoints;

    [SerializeField] int price;

    [SerializeField] float damage;
    [SerializeField] float attackDelay;
    [SerializeField] float radius;

    [SerializeField] TowerData previousUpgrade;
    [SerializeField] List<TowerData> nextUpgrades;

    public string Name => name;
    public Sprite Sprite => sprite;
    public int HitPoints => hitPoints;

    public int Price => price;

    public float Damage => damage;
    public float AttackDelay => attackDelay;
    public float Radius => radius;

    [SerializeField] [TextArea(3, 10)] string description;
    public string Description => description;

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
        var totalPrice = Price;
        var previous = previousUpgrade;

        while (previous != null)
        {
            totalPrice += previous.Price;
            previous = previous.previousUpgrade;
        }

        return totalPrice;
    }
}