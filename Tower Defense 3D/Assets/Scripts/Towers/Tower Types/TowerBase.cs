using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour, IUpgradeable, ISellable, IInteractable, IEntity, IEntityDamage
{
    [SerializeField] TowerData towerData;
    [SerializeField] SellableTooltip sellableTooltip;

    public TowerData TowerData
    {
        get
        {
            return towerData;
        }
        private set
        {
            towerData = value;
        }
    }
        
    //============================================
    // IUpgradeable
    //============================================
    public List<IUpgradeOption> UpgradeOptions => new List<IUpgradeOption>(TowerData.NextUpgrades);
    public IUpgradeOption CurrentUpgrade => towerData;
    public bool ProgressUpgradeTree => throw new System.NotImplementedException();
    public bool IsUnderUpgrade { get; private set; }

    //============================================
    // ISellable
    //============================================
    public SellableTooltip Tooltip
    {
        get
        {
            sellableTooltip.Price = towerData.GetSellValue();
            return sellableTooltip;
        }
    }

    //============================================
    // IEntity
    //============================================
    public string Name => TowerData.Name;
    public Sprite Sprite => TowerData.Sprite;
    public int CurrentHitPoints => TowerData.HitPoints;
    public int TotalHitPoints => TowerData.HitPoints;

    //============================================
    // IEntityDamage
    //============================================
    public int DamageValue => (int)TowerData.Damage;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }    

    //============================================
    // IUpgradeable
    //============================================
    public virtual void OnUpgradeStarted(IUpgradeOption upgradeOption, out bool upgradeStarted)
    {
        if (GameController.Instance.Currency < upgradeOption.Price)
        {
            Debug.Log("Not enough gold!");
            upgradeStarted = false;
            return;
        }

        upgradeStarted = true;
        IsUnderUpgrade = true;

        //simulate construction time
        //after finished
        StartCoroutine(OnUpgradeRunning(upgradeOption));
    }

    public IEnumerator OnUpgradeRunning(IUpgradeOption upgradeOption)
    {
        yield return new WaitForEndOfFrame();

        OnUpgradeFinished(upgradeOption);
    }

    public virtual void OnUpgradeFinished(IUpgradeOption upgradeOption)
    {
        IsUnderUpgrade = false;

        if (upgradeOption != CurrentUpgrade)
        {
            var nextTowerData = TowerData.NextUpgrades.Find(x => x == upgradeOption);

            TowerData = nextTowerData;

            InteractionSystem.Instance.RefreshInteractions();
        }

        var price = TowerData.Price;

        var position = transform.position;
        position.y += GetComponent<Collider>().bounds.size.y / 2;

        GameController.Instance.ModifyCurrencyBy(-price, position);
    }

    public virtual void OnUpgradeCanceled()
    {
        IsUnderUpgrade = false;
    }

    //============================================
    // ISellable
    //============================================
    public virtual void Sell()
    {
        var sellValue = TowerData.GetSellValue();

        var position = transform.position;
        position.y += GetComponent<Collider>().bounds.size.y / 2;

        GameController.Instance.ModifyCurrencyBy(sellValue, position);

        Destroy(gameObject);
    }
    
}