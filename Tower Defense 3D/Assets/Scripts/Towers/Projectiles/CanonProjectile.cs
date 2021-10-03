using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CanonProjectile : NetworkBehaviour, IProjectileSingleTarget
{
    [SerializeField] float speed;
    [SerializeField] float travelledDistanceAfterReachingTarget;
    
    private Vector3 _moveDirection;

    private float _targetDistance;
    private float _travelledDistance;
    private float _damage;

    [Server]
    public void Initialize(Vector3 targetPosition, float effectValue)
    {
        _targetDistance = Vector3.Distance(transform.position, targetPosition);

        _moveDirection = (targetPosition - transform.position).normalized;
        _damage = effectValue;
    }

    [ServerCallback]
    private void Update()
    {
        MoveInPositionOfTarget();
    }

    [Server]
    public void MoveInPositionOfTarget()
    {
        if (_moveDirection == null || _travelledDistance >= travelledDistanceAfterReachingTarget + _targetDistance)
        {
            Destroy(gameObject);
            return;
        }

        var distanceDelta = _moveDirection * speed * Time.deltaTime;

        _travelledDistance += distanceDelta.magnitude;
        transform.position += distanceDelta;

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ApplyEffectOnImpact(collision.gameObject);
        }
    }
    
    [Server]
    public void ApplyEffectOnImpact(GameObject target)
    {
        target.GetComponent<Enemy>().TakeDamage(_damage);
        Destroy(gameObject);
    }
}
