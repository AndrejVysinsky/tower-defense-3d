using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] LineRenderer laser;
    [SerializeField] TowerTargeting towerTargeting;

    protected override void Awake()
    {
        base.Awake();

        towerTargeting.enabled = false;
    }

    private void Update()
    {
        if (IsUnderConstruction)
            return;

        if (towerTargeting.Target != null)
        {
            laser.enabled = true;
            Shoot(towerTargeting.Target);
        }
        else
        {
            laser.enabled = false;
        }
    }

    public void Shoot(GameObject target)
    {
        laser.SetPosition(0, towerTargeting.GetFirePoint().transform.position);
        laser.SetPosition(1, target.transform.position);

        target.GetComponent<Enemy>().TakeDamage(TowerData.Damage * Time.deltaTime);
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
