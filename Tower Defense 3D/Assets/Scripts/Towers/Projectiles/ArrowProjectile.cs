using Mirror;
using UnityEngine;

public class ArrowProjectile : NetworkBehaviour, IProjectileSingleTarget
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float speed;
    [SerializeField] float travelledDistanceAfterReachingTarget;

    private uint _playerId;
    private Enemy _enemy;
    [SyncVar] private Vector3 _targetPosition;
    private Vector3 _moveDirection;
    private float _targetDistance;
    private float _damage;

    private float _travelledDistance;

    [Server]
    public void Initialize(uint playerId, Enemy enemy, float effectValue)
    {
        _playerId = playerId;
        _enemy = enemy;
        _damage = effectValue;
        _targetPosition = _enemy.GetEnemyHitPoint();

        _targetDistance = Vector3.Distance(transform.position, _targetPosition);
        _moveDirection = (_targetPosition - transform.position).normalized;
        transform.LookAt(_targetPosition);
    }

    private void Update()
    {
        if (isServer)
        {
            if (Vector3.SqrMagnitude(_enemy.GetEnemyHitPoint() - _targetPosition) < 0.1)
            {
                _targetPosition = _enemy.GetEnemyHitPoint();
            }
        }

        MoveInPositionOfTarget();
    }

    public void MoveInPositionOfTarget()
    {
        if (_moveDirection == null || _travelledDistance >= travelledDistanceAfterReachingTarget + _targetDistance)
        {
            Destroy(gameObject);
            return;
        }
        
        _targetDistance = Vector3.Distance(transform.position, _targetPosition);
        _moveDirection = (_targetPosition - transform.position).normalized;
        transform.LookAt(_targetPosition);

        var distanceDelta = _moveDirection * speed * Time.deltaTime;

        _travelledDistance += distanceDelta.magnitude;
        transform.position += distanceDelta;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isServer)
            {
                ApplyEffectOnImpact(collision.gameObject);
            }
            else
            {
                //prevents projectile in clients from overshooting - going through enemy
                HideProjectile();
            }
        }
    }
    
    [Server]
    public void ApplyEffectOnImpact(GameObject target)
    {
        target.GetComponent<Enemy>().TakeDamage(_playerId, _damage);
        Destroy(gameObject);
    }

    private void HideProjectile()
    {
        meshRenderer.enabled = false;
    }
}
