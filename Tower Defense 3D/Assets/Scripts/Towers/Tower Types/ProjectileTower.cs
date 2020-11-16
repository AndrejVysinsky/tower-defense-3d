using UnityEngine;

public class ProjectileTower : TowerBase
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] TowerTargeting towerTargeting;

    private float _timer;

    protected override void Awake()
    {
        base.Awake();

        towerTargeting.enabled = false;
    }

    private void Update()
    {
        if (IsUnderUpgrade)
            return;

        _timer += Time.deltaTime;

        if (towerTargeting.Target != null && _timer >= TowerData.AttackDelay)
        {
            _timer = 0;
            Shoot(towerTargeting.Target);
        }
    }

    public void Shoot(GameObject target)
    {
        var projectile = Instantiate(projectilePrefab, towerTargeting.GetFirePoint().transform.position, transform.rotation);

        projectile.GetComponent<IProjectileMovement>().Initialize(target.transform.position, TowerData.Damage);        
    }

    public override void OnUpgradeStarted(IUpgradeOption upgradeOption, out bool upgradeStarted)
    {
        base.OnUpgradeStarted(upgradeOption, out upgradeStarted);

        if (upgradeStarted)
        {
            towerTargeting.enabled = false;
        }
    }

    public override void OnUpgradeFinished(IUpgradeOption upgradeOption)
    {
        base.OnUpgradeFinished(upgradeOption);

        towerTargeting.enabled = true;
    }

    public override void OnUpgradeCanceled()
    {
        base.OnUpgradeCanceled();

        towerTargeting.enabled = true;
    }

    public override void Sell()
    {
        base.Sell();
    }       
}
