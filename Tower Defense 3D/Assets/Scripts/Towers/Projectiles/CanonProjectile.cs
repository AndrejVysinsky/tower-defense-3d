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

    [SyncVar] private Vector3 _moveDirection;
    [SyncVar] private float _targetDistance;
    [SyncVar] private float _damage;

    private float _travelledDistance;

    [Server]
    public void Initialize(Vector3 targetPosition, float effectValue)
    {
        _targetDistance = Vector3.Distance(transform.position, targetPosition);

        _moveDirection = (targetPosition - transform.position).normalized;
        _damage = effectValue;

        transform.LookAt(targetPosition);
    }

    private void Update()
    {
        MoveInPositionOfTarget();
    }

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
        target.GetComponent<Enemy>().TakeDamage(_damage);
        Destroy(gameObject);
    }

    private void HideProjectile()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
