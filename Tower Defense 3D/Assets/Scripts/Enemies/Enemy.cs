using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    private NavMeshAgent _agent;

    private HealthScript _healthScript;

    private float _difficultyModifier;

    private Vector3 _start;
    private Vector3 _end;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Path path, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _start = path.GetStartPosition();
        _end = path.GetEndPosition();
        
        //var spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = sprite;
        //spriteRenderer.color = color;

        //_healthScript = GetComponent<HealthScript>();
        //_healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));

        _difficultyModifier = difficultyMultiplier;

        MoveToStart();
    }


    private void Update()
    {
        if (transform.position.x == _end.x && transform.position.z == _end.z)
        {
            DealDamageToPlayer();
            MoveToStart();
        }
    }

    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        //GameController.Instance.ModifyLivesBy(-damage, transform.position);
    }

    private void MoveToStart()
    {
        transform.position = _start;

        _agent.SetDestination(_end);
        
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
}
