using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    [SerializeField] LineRenderer rangeLineRenderer;
    [SerializeField] GameObject moveablePart;
    [SerializeField] GameObject firePoint;
    
    private PriorityQueue<GameObject, int> _targetsInRange;
    
    public RangeRenderer RangeRenderer { get; private set; }
    public GameObject Target { get; private set; }
    public SpriteRenderer TowerSprite { get; private set; }

    private void Awake()
    {
        _targetsInRange = new PriorityQueue<GameObject, int>();
        RangeRenderer = new RangeRenderer(rangeLineRenderer, GetComponent<SphereCollider>().radius);
        TowerSprite = moveablePart.GetComponent<SpriteRenderer>();
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

    public void SetEnemyTargeting(bool value)
    {
        GetComponent<Collider>().enabled = value;
    }

    public GameObject GetFirePoint()
    {
        return firePoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var newEnemy = other.gameObject.GetComponent<Enemy>();

            _targetsInRange.Insert(other.gameObject, newEnemy.GetPriority());

            Target = GetTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _targetsInRange.Remove(other.gameObject);

            Target = GetTarget();
        }
    }

    private GameObject GetTarget()
    {
        //sometimes target on top of queue is null

        while (_targetsInRange.Count() > 0 &&  _targetsInRange.Peek() == null)
        {
            _targetsInRange.Pop();
        }

        return _targetsInRange.Count() > 0 ? _targetsInRange.Peek() : null;
    }
}
