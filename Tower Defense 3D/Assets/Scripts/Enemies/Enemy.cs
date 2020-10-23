using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    private NavMeshAgent _agent;

    private HealthScript _healthScript;

    private float _difficultyModifier;

    private PortalStart _portalStart;
    private PortalEnd _portalEnd;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Path path, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _portalStart = path.PortalStart;
        _portalEnd = path.PortalEnd;
        
        //var spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = sprite;
        //spriteRenderer.color = color;

        //_healthScript = GetComponent<HealthScript>();
        //_healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));

        _difficultyModifier = difficultyMultiplier;

        MoveToStart();
    }

    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        //GameController.Instance.ModifyLivesBy(-damage, transform.position);
    }

    private void MoveToStart()
    {
        _agent.Warp(_portalStart.transform.position);

        _agent.SetDestination(_portalEnd.transform.position);
    }

    public void OnPortalEndReached()
    {
        DealDamageToPlayer();
        MoveToStart();
    }

    public void TakeDamage(float amount)
    {
        _healthScript.SubtractHealth(amount);

        if (_healthScript.Health == 0)
        {
            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            //GameController.Instance.ModifyCurrencyBy((int)reward, transform.position);

            Destroy(gameObject);
        }
    }

    public int GetPriority()
    {
        return (int)_agent.remainingDistance;
    }
}
