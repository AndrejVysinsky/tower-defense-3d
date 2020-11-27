using System.Collections.Generic;
using UnityEngine;

public class BombardProjectile : MonoBehaviour, IProjectileWithAreaEffect
{
    [SerializeField] float speed;
    [Tooltip("Sets sphere collider radius")]
    [SerializeField] float damageRange;
    [SerializeField] float arcHeight;

    private float _targetDistance;
    private float _damage;

    private Vector3 _startingPosition;
    private Vector3 _currentPosition;
    private Vector3 _targetPosition;

    public List<GameObject> TargetsInRange { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        _startingPosition = transform.position;
        _currentPosition = transform.position;

        GetComponent<SphereCollider>().radius = damageRange;
    }

    public void Initialize(Vector3 targetPosition, float effectValue)
    {
        _targetDistance = Vector3.Distance(_startingPosition, targetPosition);

        _targetPosition = targetPosition;
        _damage = effectValue;
    }

    private void Update()
    {
        MoveInPositionOfTarget();
    }

    public void MoveInPositionOfTarget()
    {
        _currentPosition = Vector3.MoveTowards(_currentPosition, _targetPosition, Time.deltaTime * _targetDistance * speed);

        float arc = MathfArc.GetArcHeightAtPosition(_startingPosition, _currentPosition, _targetPosition, arcHeight);

        transform.position = new Vector3(_currentPosition.x, _currentPosition.y + arc, _currentPosition.z);

        if (_currentPosition == _targetPosition)
        {
            ApplyEffectOnImpact(TargetsInRange);
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

    public void ApplyEffectOnImpact(List<GameObject> targets)
    {
        foreach (var target in targets)
        {
            if (target == null)
                continue;

            var distance = Vector3.Distance(target.transform.position, transform.position);

            var damageFactor = 1 - distance / damageRange;

            target.GetComponent<Enemy>().TakeDamage(_damage * damageFactor);
        }

        Destroy(gameObject);
    }
}
