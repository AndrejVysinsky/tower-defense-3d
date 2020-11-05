using UnityEngine;

public class CanonTower : TowerBase
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
        if (IsUnderConstruction || Level == 0)
            return;

        _timer += Time.deltaTime;

        if (towerTargeting.Target != null && _timer >= TowerData.GetLevelData(Level).AttackDelay)
        {
            _timer = 0;
            Shoot(towerTargeting.Target);
        }
    }

    public void Shoot(GameObject target)
    {
        var projectile = Instantiate(projectilePrefab, towerTargeting.GetFirePoint().transform.position, transform.rotation);

        projectile.GetComponent<CanonProjectile>().Initialize(target.transform.position, TowerData.GetLevelData(Level).Damage);        
    }

    public override void Upgrade()
    {
        base.Upgrade();
    }

    public override void Sell()
    {
        base.Sell();
    }

    public override void OnConstructionStarted()
    {
        towerTargeting.enabled = false;

        base.OnConstructionStarted();
    }

    public override void OnConstructionFinished()
    {
        towerTargeting.enabled = true;

        base.OnConstructionFinished();
    }

    public override void OnConstructionCanceled()
    {
        towerTargeting.enabled = true;

        base.OnConstructionCanceled();
    }
}
