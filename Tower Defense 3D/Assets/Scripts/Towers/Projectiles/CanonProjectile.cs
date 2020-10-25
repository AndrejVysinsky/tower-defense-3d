using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CanonProjectile : MonoBehaviour, IProjectileSingleTarget
{
    [SerializeField] float speed;
    [SerializeField] float travelledDistanceAfterReachingTarget;
    
    private Vector3 _moveDirection;

    private float _targetDistance;
    private float _travelledDistance;
    private float _damage;

    public void Initialize(Vector3 targetPosition, float effectValue)
    {
        _targetDistance = Vector3.Distance(transform.position, targetPosition);

        _moveDirection = (targetPosition - transform.position).normalized;
        _damage = effectValue;
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
            ApplyEffectOnImpact(collision.gameObject);
        }
    }
    
    public void ApplyEffectOnImpact(GameObject target)
    {
        target.GetComponent<Enemy>().TakeDamage(_damage);
        Destroy(gameObject);
    }
}
