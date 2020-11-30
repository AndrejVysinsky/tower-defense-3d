using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    [Serializable]
    public class PartRotation
    {
        public GameObject part;
        public Vector3 rotation;
    }

    [SerializeField] GameObject firePoint;

    [SerializeField] List<PartRotation> lookAtTargetParts;
    [SerializeField] List<PartRotation> lookInDirectionParts;
    
    private List<Enemy> _enemiesInRange;
    
    public RangeRenderer RangeRenderer { get; private set; }
    public GameObject Target { get; private set; }

    private bool _isActive;

    private void Awake()
    {
        _enemiesInRange = new List<Enemy>();
        //RangeRenderer = new RangeRenderer(rangeLineRenderer, GetComponent<SphereCollider>().radius);
    }

    private void Update()
    {
        if (_isActive == false)
            return;

        //when target dies get another one
        if (Target == null)
        {
            Target = GetTarget();
        }

        if (Target != null)
        {
            LookAt(Target.transform.position);
        }
    }

    private void LookAt(Vector3 position)
    {
        for (int i = 0; i < lookAtTargetParts.Count; i++)
        {
            lookAtTargetParts[i].part.transform.LookAt(position);

            var rotation = lookAtTargetParts[i].part.transform.rotation;
            rotation.eulerAngles += lookAtTargetParts[i].rotation;
            lookAtTargetParts[i].part.transform.rotation = rotation;
        }

        for (int i = 0; i < lookInDirectionParts.Count; i++)
        {
            position.y = lookInDirectionParts[i].part.transform.position.y;

            lookInDirectionParts[i].part.transform.LookAt(position);

            var rotation = lookInDirectionParts[i].part.transform.rotation;
            rotation.eulerAngles += lookInDirectionParts[i].rotation;
            lookInDirectionParts[i].part.transform.rotation = rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemiesInRange.Add(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemiesInRange.Remove(other.gameObject.GetComponent<Enemy>());
            Target = GetTarget();
        }
    }

    private GameObject GetTarget()
    {
        //sometimes target on top of queue is null
        _enemiesInRange.RemoveAll(enemy => enemy == null);

        if (_enemiesInRange.Count == 0)
            return null;

        float remainingDistance = _enemiesInRange.Min(enemy => enemy.GetRemainingDistance());

        return _enemiesInRange.Where(enemy => enemy.GetRemainingDistance() == remainingDistance).FirstOrDefault().gameObject;
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
