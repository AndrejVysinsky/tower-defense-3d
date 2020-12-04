using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IInteractable, IEntity
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] HealthScript healthScript;

    private NavMeshAgent _agent;

    private float _difficultyModifier;

    private PortalStart _start;
    private PortalEnd _end;

    //============================================
    // IEntity
    //============================================
    public string Name => enemyData.Name;
    public Sprite Sprite => enemyData.Sprite;
    public int CurrentHitPoints => (int)healthScript.Health;
    public int TotalHitPoints => (int)healthScript.MaxHealth;

    private bool _isDead;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = enemyData.Speed;
    }

    public void Initialize(PortalStart startPortal, PortalEnd endPortal, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _start = startPortal;
        _end = endPortal;

        healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));
        _difficultyModifier = difficultyMultiplier;

        MoveToStart();
    }

    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        GameController.Instance.ModifyLivesBy(-damage, transform.position);
    }

    private void MoveToStart()
    {
        _agent.Warp(_start.GetRandomStartPosition());

        _agent.SetDestination(_end.GetRandomEndPosition());
    }

    public void OnPortalEndReached()
    {
        DealDamageToPlayer();
        MoveToStart();
    }

    public void TakeDamage(float amount)
    {
        healthScript.SubtractHealth(amount);

        if (healthScript.Health == 0 && _isDead == false)
        {
            _isDead = true;

            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            GameController.Instance.ModifyCurrencyBy((int)reward, transform.position);

            Destroy(gameObject);
        }
    }

    public float GetRemainingDistance()
    {
        return _agent.remainingDistance;
    }
}
