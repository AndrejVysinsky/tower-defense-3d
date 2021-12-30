using Mirror;
using System.Linq;
using UnityEngine;

public class LaserTower : TowerBase, IGridObjectInitialized, IGridObjectPlaced
{
    [SerializeField] LineRenderer laser;
    [SerializeField] ParticleSystem laserParticles;
    [SerializeField] TowerTargeting towerTargeting;

    [SyncVar]
    private bool _isShooting = false;

    private float _timer;
    private float _syncInterval = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        laserParticles = Instantiate(laserParticles).GetComponent<ParticleSystem>();

        laser.enabled = false;
        laserParticles.Stop();
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
        if (towerTargeting.Target != null)
        {
            UpdateLaserPosition();
        }

        if (!hasAuthority)
            return;

        if (IsUnderUpgrade)
            return;

        if (towerTargeting.Target == null)
        {
            if (_isShooting)
            {
                CmdStopShooting();
            }
            return;
        }
        
        _timer += Time.deltaTime;

        if (_isShooting == false)
        {
            CmdShoot();
        }
        else
        {
            if (_timer >= _syncInterval)
            {
                CmdDealDamage(_timer);
                _timer = 0;
            }
        }
    }

    [Command]
    public void CmdDealDamage(float time)
    {
        if (towerTargeting.Target == null)
            return;

        towerTargeting.Target.TakeDamage(TowerData.Damage * time);
    }

    [Command]
    public void CmdShoot()
    {
        if (towerTargeting.Target == null)
            return;

        _isShooting = true;
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
        if (towerTargeting.Target == null)
        {
            Debug.Log("Target null on RpcShowLaser");
            return;
        }

        laser.enabled = true;        
        if (laserParticles.isStopped)
        {
            laserParticles.Play();
        }
        UpdateLaserPosition();
    }

    private void UpdateLaserPosition()
    {
        var startPosition = towerTargeting.GetFirePointPosition();
        var endPosition = towerTargeting.Target.GetEnemyHitPoint();
        var particlePosition = towerTargeting.Target.transform.position;
        var particleOrientation = transform.position;

        laser.SetPosition(0, startPosition);
        laser.SetPosition(1, endPosition);

        laserParticles.transform.position = particlePosition;
        laserParticles.transform.LookAt(particleOrientation);
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

        if (hasAuthority)
        {
            CmdStopShooting();
        }
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
