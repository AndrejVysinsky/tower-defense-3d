using Mirror;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour, IInteractable, IEntity
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] HealthScript healthScript;
    [SerializeField] Animator animator;

    [SyncVar]
    private float _difficultyModifier;

    private Pathway _pathway;
    private int _currentCheckpoint;
    private Vector3 _currentCheckpointPosition;

    //============================================
    // IEntity
    //============================================
    public string Name => enemyData.Name;
    public Sprite Sprite => enemyData.Sprite;
    public int CurrentHitPoints => (int)healthScript.Health;
    public int TotalHitPoints => (int)healthScript.MaxHealth;
    public EnemyData EnemyData => enemyData;
    public int ScaledReward => (int)(enemyData.RewardToPlayer * (1 + _difficultyModifier));

    public bool IsDead { get; private set; } = false;
    public uint PlayerId => _pathway.PlayerId;

    public void Initialize(Pathway pathway, float difficultyMultiplier)
    {
        _pathway = pathway;
        _difficultyModifier = difficultyMultiplier;
        transform.position = _pathway.GetCheckpointGroundPosition(0);
        SetNextCheckpoint();        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        healthScript.Initialize(enemyData.Health * (1 + _difficultyModifier));
    }

    [ServerCallback]
    private void Update()
    {
        if (IsDead)
            return;

        if (transform.position == _currentCheckpointPosition)
        {
            SetNextCheckpoint();
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentCheckpointPosition, enemyData.Speed * Time.deltaTime);
    }

    [Server]
    private void SetNextCheckpoint()
    {
        if (_currentCheckpoint >= _pathway.NumberOfCheckpoints - 1)
        {
            DealDamageToPlayer();

            _currentCheckpoint = 0;
            transform.position = _pathway.GetCheckpointGroundPosition(_currentCheckpoint);
        }
        
        _currentCheckpoint++;

        _currentCheckpointPosition = _pathway.GetCheckpointGroundPosition(_currentCheckpoint);

        transform.LookAt(_currentCheckpointPosition);
    }

    [Server]
    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        var identity = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == PlayerId);
        identity.GetComponent<NetworkPlayer>().UpdateLives(PlayerId, -damage, GetEnemyHitPoint());
    }

    [Server]
    public void TakeDamage(float amount)
    {
        healthScript.SubtractHealth(amount);

        if (healthScript.Health == 0 && IsDead == false)
        {
            IsDead = true;

            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            var identity = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == PlayerId);
            identity.GetComponent<NetworkPlayer>().UpdateCurrency(PlayerId, (int)reward, GetEnemyHitPoint());

            OnDeath();
        }
    }

    [Server]
    private void OnDeath()
    {
        GetComponent<Collider>().enabled = false;

        var player = FindObjectsOfType<NetworkPlayer>().FirstOrDefault(x => x.MyInfo.netId == PlayerId);
        player.RemoveEnemyFromCreepCount();

        RpcDeathEffect();

        Destroy(gameObject, 2f);
    }

    [ClientRpc]
    private void RpcDeathEffect()
    {
        animator.Play("Death");
    }

    public float GetRemainingDistance()
    {
        var remainingDistance = Vector3.Distance(transform.position, _currentCheckpointPosition);

        for (int i = _currentCheckpoint + 1; i < _pathway.NumberOfCheckpoints; i++)
        {
            remainingDistance += Vector3.Distance(_pathway.GetCheckpointGroundPosition(i - 1), _pathway.GetCheckpointGroundPosition(i));
        }

        return remainingDistance;
    }

    public Vector3 GetEnemyHitPoint()
    {
        return GetComponent<Collider>().bounds.center;
    }
}
