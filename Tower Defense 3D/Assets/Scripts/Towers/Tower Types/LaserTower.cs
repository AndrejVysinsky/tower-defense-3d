using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] LineRenderer laser;
    [SerializeField] ParticleSystem laserParticles;
    [SerializeField] TowerTargeting towerTargeting;

    protected override void Awake()
    {
        base.Awake();

        towerTargeting.SetTargeting(false);

        laserParticles = Instantiate(laserParticles).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (IsUnderUpgrade)
            return;

        if (towerTargeting.Target != null)
        {
            laser.enabled = true;
            Shoot(towerTargeting.Target);
        }
        else
        {
            laser.enabled = false;

            if (laserParticles.isPlaying)
                laserParticles.Stop();
        }
    }

    public void Shoot(Enemy target)
    {
        laser.SetPosition(0, towerTargeting.GetFirePointPosition());
        laser.SetPosition(1, target.GetEnemyHitPoint());

        laserParticles.transform.position = target.transform.position;
        laserParticles.transform.LookAt(transform.position);

        if (laserParticles.isStopped)
            laserParticles.Play();

        target.CmdTakeDamage(TowerData.Damage * Time.deltaTime);
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
