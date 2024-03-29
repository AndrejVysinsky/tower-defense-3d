﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerBase : NetworkBehaviour, IUpgradeable, ISellable, IInteractable, IEntity, IEntityDamage
{
    [SerializeField] TowerData towerData;
    [SerializeField] SellableTooltip sellableTooltip;
    [SerializeField] TowerColor towerColor;

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

    [SyncVar] private uint _playerId;
    public uint PlayerId => _playerId;

    [SyncVar] private Color _playerColor;
    public Color PlayerColor => _playerColor;

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
    // Network
    //============================================
    [Server]
    public void SetPlayerId(uint playerId)
    {
        _playerId = playerId;

        var player = FindObjectsOfType<NetworkPlayer>().FirstOrDefault(x => x.MyInfo.netId == _playerId);
        _playerColor = player.MyInfo.color;
    }

    [ClientRpc]
    public void RpcSetTarget(uint networkIdentityId)
    {
        var enemy = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == networkIdentityId).GetComponent<Enemy>();

        SetTarget(enemy);
    }

    [ClientRpc]
    public void RpcResetTarget()
    {
        SetTarget(null);
    }

    [ClientRpc]
    public void RpcSetLookingAtTarget(bool isLookingAtTarget)
    {
        SetLookingAtTarget(isLookingAtTarget);
    }

    protected virtual void SetTarget(Enemy enemy)
    {

    }

    protected virtual void SetLookingAtTarget(bool isLookingAtTarget)
    {

    }    

    //============================================
    // IUpgradeable
    //============================================
    public virtual void OnUpgradeStarted(IUpgradeOption upgradeOption, out bool upgradeStarted)
    {
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

        UpdateTowerColor();
    }

    //not used
    public virtual void OnUpgradeCanceled()
    {
        IsUnderUpgrade = false;
    }

    //============================================
    // ISellable
    //============================================
    public virtual void Sell()
    {
        if (hasAuthority == false)
        {
            return;
        }

        CmdSell(PlayerId);
    }

    [Command]
    public void CmdSell(uint playerId)
    {
        var sellValue = TowerData.GetSellValue();

        var identity = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == playerId);
        identity.GetComponent<NetworkPlayer>().UpdateCurrency(PlayerId, sellValue, GetFloatTextPosition());

        NetworkServer.Destroy(gameObject);
    }

    //============================================
    // Misc
    //============================================
    public virtual Vector3 GetFloatTextPosition()
    {
        var position = transform.position;
        position.y += GetComponent<Collider>().bounds.size.y / 2;

        return position;
    }

    private void UpdateTowerColor()
    {
        towerColor.ChangeTowerColor(_playerColor);
    }
}