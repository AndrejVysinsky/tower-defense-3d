using UnityEngine;

public class ProjectileTower : TowerBase
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] TowerTargeting towerTargeting;

    private float _timer;

    protected override void Awake()
    {
        base.Awake();

        towerTargeting.SetTargeting(false);
    }

    private void Update()
    {
        if (IsUnderUpgrade)
            return;

        _timer += Time.deltaTime;

        if (towerTargeting.Target != null && towerTargeting.IsLookingAtTarget && _timer >= TowerData.AttackDelay)
        {
            _timer = 0;
            Shoot(towerTargeting.Target);
        }
    }

    public void Shoot(Enemy target)
    {
        var projectile = Instantiate(projectilePrefab, towerTargeting.GetFirePointPosition(), transform.rotation);

        projectile.GetComponent<IProjectileMovement>().Initialize(target.GetEnemyHitPoint(), TowerData.Damage);        
    }

    public override void OnUpgradeStarted(IUpgradeOption upgradeOption, out bool upgradeStarted)
    {
        base.OnUpgradeStarted(upgradeOption, out upgradeStarted);

        if (upgradeStarted)
        {
            towerTargeting.SetTargeting(false);
        }
    }

    public override void OnUpgradeFinished(IUpgradeOption upgradeOption)
    {
        base.OnUpgradeFinished(upgradeOption);

        towerTargeting.SetTargeting(true);
    }

    public override void OnUpgradeCanceled()
    {
        base.OnUpgradeCanceled();

        towerTargeting.SetTargeting(true);
    }

    public override void Sell()
    {
        base.Sell();
    }       
}
