﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerBase : MonoBehaviour, IConstruction, IUpgradable, IInteractable, IEntity, IEntityDamage
{
    [SerializeField] TowerData towerData;

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
    // IConstruction
    //============================================
    public bool IsUnderConstruction { get; private set; }
    public bool IsAbleToStartConstruction => GameController.Instance.Currency >= TowerData.Price;

    //============================================
    // IUpgradeable
    //============================================
    public List<IUpgradeOption> UpgradeOptions => new List<IUpgradeOption>() { TowerData };
    public bool ProgressUpgradeTree => throw new System.NotImplementedException();

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

    public virtual void Upgrade()
    {
        TowerData = TowerData.NextUpgrades[0];

        GetComponent<MeshRenderer>().material = TowerData.Material;

        var price = TowerData.Price;

        GameController.Instance.ModifyCurrencyBy(-price, transform.position);
    }

    public virtual void Sell()
    {
        var sellValue = TowerData.GetSellValue();

        GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }

    //============================================
    // IConstruction
    //============================================
    public virtual void OnConstructionStarted()
    {
        IsUnderConstruction = true;

        //simulate construction time
        //after finished
        OnConstructionFinished();
    }

    public virtual void OnConstructionFinished()
    {
        IsUnderConstruction = false;
        //Upgrade();
    }

    public virtual void OnConstructionCanceled()
    {
        IsUnderConstruction = false;
    }

    //============================================
    // IUpgradeable
    //============================================
    public void Upgrade(IUpgradeOption upgradeOption)
    {
        TowerData = TowerData.NextUpgrades[0];

        GetComponent<MeshRenderer>().material = TowerData.Material;

        var price = TowerData.Price;

        GameController.Instance.ModifyCurrencyBy(-price, transform.position);
    }
}