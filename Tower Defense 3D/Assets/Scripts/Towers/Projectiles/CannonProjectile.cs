using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : NetworkBehaviour, IProjectileWithAreaEffect
{
    [SerializeField] float speed;
    [Tooltip("Sets sphere collider radius")]
    [SerializeField] float damageRange;
    [SerializeField] float arcHeight;
    [SerializeField] ParticleSystem particlesPrefab;

    private uint _playerId;
    [SyncVar] private float _targetDistance;
    [SyncVar] private float _damage;
    [SyncVar] private Vector3 _targetPosition;

    private Vector3 _startingPosition;
    private Vector3 _currentPosition;

    private bool _targetReached = false;

    public List<GameObject> TargetsInRange { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        _startingPosition = transform.position;
        _currentPosition = transform.position;

        GetComponent<SphereCollider>().radius = damageRange;
    }

    [Server]
    public void Initialize(uint playerId, Enemy enemy, float effectValue)
    {
        _playerId = playerId;
        _targetDistance = Vector3.Distance(_startingPosition, enemy.GetEnemyHitPoint());

        _targetPosition = enemy.GetEnemyHitPoint();
        _damage = effectValue;
    }

    private void Update()
    {
        if (_targetReached)
            return;

        MoveInPositionOfTarget();
    }

    public void MoveInPositionOfTarget()
    {
        _currentPosition = Vector3.MoveTowards(_currentPosition, _targetPosition, Time.deltaTime * _targetDistance * speed);

        float arc = MathfArc.GetArcHeightAtPosition(_startingPosition, _currentPosition, _targetPosition, arcHeight);

        transform.position = new Vector3(_currentPosition.x, _currentPosition.y + arc, _currentPosition.z);

        if (_currentPosition == _targetPosition)
        {
            _targetReached = true;
            ShowParticleEffect();
            HideProjectile();
            if (isServer)
            {
                ApplyEffectOnImpact(TargetsInRange);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            AddTarget(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            RemoveTarget(other.gameObject);
        }
    }

    public void AddTarget(GameObject target)
    {
        TargetsInRange.Add(target);
    }

    public void RemoveTarget(GameObject target)
    {
        TargetsInRange.Remove(target);
    }

    [Server]
    public void ApplyEffectOnImpact(List<GameObject> targets)
    {
        foreach (var target in targets)
        {
            if (target == null)
                continue;

            var distance = Vector3.Distance(target.transform.position, transform.position);

            var damageFactor = 1 - distance / damageRange;

            target.GetComponent<Enemy>().TakeDamage(_playerId, _damage * damageFactor);
        }

        Destroy(gameObject, 0.5f);
    }

    private void ShowParticleEffect()
    {
        var particles = Instantiate(particlesPrefab);
        particles.transform.position = transform.position;

        Destroy(particles.gameObject, particles.main.duration);
    }

    private void HideProjectile()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
