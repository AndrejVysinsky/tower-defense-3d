using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] HealthScript healthScript;

    private NavMeshAgent _agent;

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
        healthScript.SubtractHealth(amount);

        if (healthScript.Health == 0)
        {
            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            GameController.Instance.ModifyCurrencyBy((int)reward, transform.position);

            Destroy(gameObject);
        }
    }

    public float GetRemainingDistance()
    {
        return Vector3.Distance(transform.position, _portalEnd.transform.position);
    }
}
