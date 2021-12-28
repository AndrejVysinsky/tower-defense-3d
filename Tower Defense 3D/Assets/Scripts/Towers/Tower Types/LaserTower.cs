using Mirror;
using UnityEngine;

public class LaserTower : TowerBase, IGridObjectInitialized, IGridObjectPlaced
{
    [SerializeField] LineRenderer laser;
    [SerializeField] ParticleSystem laserParticles;
    [SerializeField] TowerTargeting towerTargeting;

    [SyncVar]
    private bool _isShooting = false;

    [SyncVar]
    private Enemy _target = null;

    protected override void Awake()
    {
        base.Awake();

        laserParticles = Instantiate(laserParticles).GetComponent<ParticleSystem>();
    }

    public void OnGridObjectInitialized()
    {
        towerTargeting.SetTargeting(false);
    }

    public void OnGridObjectPlaced()
    {
        towerTargeting.SetTargeting(true);
    }

    private void Update()
    {
        if (!hasAuthority)
            return;

        if (IsUnderUpgrade)
            return;

        if (towerTargeting.Target != null)
        {
            CmdShoot();
        }
        else
        {
            CmdStopShooting();
        }
    }

    [Command]
    public void CmdShoot()
    {
        if (towerTargeting.Target == null)
            return;

        _target.TakeDamage(TowerData.Damage * Time.deltaTime);

        RpcShowLaser();
    }

    [Command]
    public void CmdStopShooting()
    {
        _isShooting = false;

        RpcHideLaser();
    }

    [ClientRpc]
    public void RpcShowLaser()
    {
        if (_target == null)
        {
            Debug.Log("Target null on RpcShowLaser");
            return;
        }

        var startPosition = towerTargeting.GetFirePointPosition();
        var endPosition = _target.GetEnemyHitPoint();
        var particlePosition = _target.transform.position;
        var particleOrientation = transform.position;

        laser.enabled = true;
        laser.SetPosition(0, startPosition);
        laser.SetPosition(1, endPosition);

        laserParticles.transform.position = particlePosition;
        laserParticles.transform.LookAt(particleOrientation);

        if (laserParticles.isStopped)
        {
            laserParticles.Play();
        }
    }

    [ClientRpc]
    public void RpcHideLaser()
    {
        laser.enabled = false;
        if (laserParticles.isPlaying)
        {
            laserParticles.Stop();
        }
    }

    protected override void SetTarget(Enemy enemy)
    {
        base.SetTarget(enemy);

        towerTargeting.Target = enemy;
        _target = enemy;
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
