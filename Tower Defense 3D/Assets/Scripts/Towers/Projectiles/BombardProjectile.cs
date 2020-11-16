using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BombardProjectile : MonoBehaviour, IProjectileWithAreaEffect
{
    [SerializeField] float speed;
    [Tooltip("Sets sphere collider radius")]
    [SerializeField] float damageRange;

    private Vector3 _moveDirection;

    private float _targetDistance;
    private float _travelledDistance;
    private float _damage;

    public List<GameObject> TargetsInRange { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = damageRange;
    }

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
        if (_moveDirection == null || _travelledDistance >= _targetDistance)
        {
            ApplyEffectOnImpact(TargetsInRange);
            return;
        }

        var distanceDelta = _moveDirection * speed * Time.deltaTime;

        _travelledDistance += distanceDelta.magnitude;
        transform.position += distanceDelta;

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

    public void ApplyEffectOnImpact(List<GameObject> targets)
    {
        targets.ForEach(target =>
        {
            var enemy = target.GetComponent<Enemy>();

            var distance = Vector3.Distance(target.transform.position, transform.position);

            var damageFactor = 1 - distance / damageRange;

            target.GetComponent<Enemy>().TakeDamage(_damage * damageFactor);
        });

        Destroy(gameObject);
    }
}
