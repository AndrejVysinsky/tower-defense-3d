using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTargeting : NetworkBehaviour
{
    [Serializable]
    public class PartRotation
    {
        public GameObject part;
        public Vector3 rotation;
    }

    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] GameObject firePoint;

    [SerializeField] List<PartRotation> lookAtTargetParts;
    [SerializeField] List<PartRotation> lookInDirectionParts;

    private List<Enemy> _enemiesInRange;

    public RangeRenderer RangeRenderer { get; private set; }

    public Enemy Target { get; set; }
    public bool IsLookingAtTarget { get; set; }

    private Quaternion _targetRotation;
    private bool _isActive;

    private TowerBase _towerBase;

    [ServerCallback]
    private void Awake()
    {
        _towerBase = GetComponentInParent<TowerBase>();
        _enemiesInRange = new List<Enemy>();
        
        //RangeRenderer = new RangeRenderer(rangeLineRenderer, GetComponent<SphereCollider>().radius);

        //fix for strange initial rotation
        LookAt(transform.position + transform.forward, 1000000);
    }

    [ServerCallback]
    private void Update()
    {
        if (_isActive == false)
            return;

        if (Target == null || Target.IsDead)
        {
            IsLookingAtTarget = false;
            _towerBase.RpcSetLookingAtTarget(IsLookingAtTarget);

            Target = GetTarget();
            if (Target != null)
            {
                _towerBase.RpcSetTarget(Target.GetComponent<NetworkIdentity>().netId);
            }
            else
            {
                _towerBase.RpcResetTarget();
            }
        }

        if (Target != null)
        {
            LookAt(Target.GetEnemyHitPoint(), rotationSpeed);
        }
    }

    [Server]
    private void LookAt(Vector3 position, float rotationSpeed)
    {
        float currentAngle = 180;

        for (int i = 0; i < lookAtTargetParts.Count; i++)
        {
            _targetRotation = Quaternion.LookRotation(position - lookAtTargetParts[i].part.transform.position);
            _targetRotation.eulerAngles += lookAtTargetParts[i].rotation;

            var angle = lookAtTargetParts[i].part.transform.rotation.eulerAngles.magnitude - _targetRotation.eulerAngles.magnitude;
            currentAngle = Mathf.Min(currentAngle, Mathf.Abs(angle));

            lookAtTargetParts[i].part.transform.rotation = Quaternion.RotateTowards(lookAtTargetParts[i].part.transform.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
        }

        for (int i = 0; i < lookInDirectionParts.Count; i++)
        {
            position.y = lookInDirectionParts[i].part.transform.position.y;

            _targetRotation = Quaternion.LookRotation(position - lookInDirectionParts[i].part.transform.position);
            _targetRotation.eulerAngles += lookInDirectionParts[i].rotation;

            var angle = lookInDirectionParts[i].part.transform.rotation.eulerAngles.magnitude - _targetRotation.eulerAngles.magnitude;
            currentAngle = Mathf.Min(currentAngle, Mathf.Abs(angle));

            lookInDirectionParts[i].part.transform.rotation = Quaternion.RotateTowards(lookInDirectionParts[i].part.transform.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (_isActive == false)
            return;

        IsLookingAtTarget = currentAngle < 5;

        if (lookAtTargetParts.Count == 0 && lookInDirectionParts.Count == 0)
        {
            IsLookingAtTarget = true;
        }

        _towerBase.RpcSetLookingAtTarget(IsLookingAtTarget);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();

            //enemy is assigned to player but player id does not match owner of tower
            if (enemy.PlayerId != 0 && enemy.PlayerId != _towerBase.PlayerId 
                && LobbyConfig.Instance.GetLobbyMode() == LobbyConfig.LobbyMode.Versus)
            {
                return;
            }

            _enemiesInRange.Add(enemy);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();

            //enemy is assigned to player but player id does not match owner of tower
            if (enemy.PlayerId != 0 && enemy.PlayerId != _towerBase.PlayerId
                && LobbyConfig.Instance.GetLobbyMode() == LobbyConfig.LobbyMode.Versus)
            {
                return;
            }

            _enemiesInRange.Remove(enemy);
            Target = GetTarget();
            if (Target != null)
            {
                _towerBase.RpcSetTarget(Target.GetComponent<NetworkIdentity>().netId);
            }
            else
            {
                _towerBase.RpcResetTarget();
            }
        }
    }

    private Enemy GetTarget()
    {
        //sometimes target on top of queue is null
        _enemiesInRange.RemoveAll(enemy => enemy == null || enemy.IsDead);

        if (_enemiesInRange.Count == 0)
            return null;

        float remainingDistance = _enemiesInRange.Min(enemy => enemy.GetRemainingDistance());

        return _enemiesInRange.FirstOrDefault(enemy => enemy.GetRemainingDistance() == remainingDistance);
    }

    public Vector3 GetFirePointPosition()
    {
        return firePoint.transform.position;
    }

    public void SetTargeting(bool active)
    {
        _isActive = active;
        GetComponent<Collider>().enabled = active;
    }
}
