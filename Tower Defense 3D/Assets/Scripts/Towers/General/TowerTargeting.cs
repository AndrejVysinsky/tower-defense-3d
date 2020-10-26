using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    [SerializeField] LineRenderer rangeLineRenderer;
    [SerializeField] GameObject moveablePart;
    [SerializeField] GameObject firePoint;
    
    private List<Enemy> _enemiesInRange;
    
    public RangeRenderer RangeRenderer { get; private set; }
    public GameObject Target { get; private set; }

    private void Awake()
    {
        _enemiesInRange = new List<Enemy>();
        RangeRenderer = new RangeRenderer(rangeLineRenderer, GetComponent<SphereCollider>().radius);
    }

    private void Update()
    {
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
        Quaternion rotation = Quaternion.LookRotation(position - moveablePart.transform.position, moveablePart.transform.TransformDirection(Vector3.back));

        //rotate only z-axis
        //rotation.x = 0;
        //rotation.y = 0;

        moveablePart.transform.rotation = rotation;
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

    public GameObject GetFirePoint()
    {
        return firePoint;
    }
}
