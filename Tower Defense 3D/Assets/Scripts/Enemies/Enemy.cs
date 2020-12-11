using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IInteractable, IEntity
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] HealthScript healthScript;
    [SerializeField] Animator animator;

    private NavMeshAgent _agent;

    private float _difficultyModifier;

    private PortalStart _start;
    private PortalEnd _end;

    //rotation
    [SerializeField] float rotationSpeed = 100f;
    private Vector3 _movingDirection;
    private Quaternion _targetRotation;

    //============================================
    // IEntity
    //============================================
    public string Name => enemyData.Name;
    public Sprite Sprite => enemyData.Sprite;
    public int CurrentHitPoints => (int)healthScript.Health;
    public int TotalHitPoints => (int)healthScript.MaxHealth;

    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        //_agent.speed = enemyData.Speed;
        _agent.updateRotation = false;
    }

    public void Initialize(PortalStart startPortal, PortalEnd endPortal, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _start = startPortal;
        _end = endPortal;

        healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));
        _difficultyModifier = difficultyMultiplier;

        MoveToStart();
    }

    private void LateUpdate()
    {
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            if (_movingDirection != _agent.velocity.normalized)
            {
                _movingDirection = _agent.velocity.normalized;

                var lookVector = _agent.velocity.normalized;
                lookVector.y = 0;

                _targetRotation = Quaternion.LookRotation(lookVector);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
        }
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

        StartCoroutine(RotateOnFrameEnd());
    }

    IEnumerator RotateOnFrameEnd()
    {
        while (_agent.velocity.normalized == Vector3.zero)
        {
            yield return new WaitForEndOfFrame();
        }
        
        transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
    }

    public void OnPortalEndReached()
    {
        DealDamageToPlayer();
        MoveToStart();
    }

    public void TakeDamage(float amount)
    {
        healthScript.SubtractHealth(amount);

        if (healthScript.Health == 0 && IsDead == false)
        {
            IsDead = true;

            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            GameController.Instance.ModifyCurrencyBy((int)reward, transform.position);

            OnDeath();
        }
    }

    private void OnDeath()
    {
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        animator.Play("Death");

        Destroy(gameObject, 2f);
    }

    public float GetRemainingDistance()
    {
        return _agent.remainingDistance;
    }
}
