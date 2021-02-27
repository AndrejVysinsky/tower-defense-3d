using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IInteractable, IEntity
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] HealthScript healthScript;
    [SerializeField] Animator animator;

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

    public bool IsDead { get; private set; } = false;

    public void Initialize(Pathway pathway, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _pathway = pathway;
        transform.position = _pathway.GetCheckpointGroundPosition(0);
        SetNextCheckpoint();

        healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));
        _difficultyModifier = difficultyMultiplier;
    }

    private void Update()
    {
        if (transform.position == _currentCheckpointPosition)
        {
            SetNextCheckpoint();
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentCheckpointPosition, enemyData.Speed * Time.deltaTime);
    }

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

    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        GameController.Instance.ModifyLivesBy(-damage, transform.position);
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
        GetComponent<Collider>().enabled = false;

        animator.Play("Death");

        Destroy(gameObject, 2f);
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
}
