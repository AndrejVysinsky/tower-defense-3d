using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    private Path _path;

    private Vector3 _currentWayPoint;
    private int _currentWayPointIndex;

    private HealthScript _healthScript;

    private float _difficultyModifier;
    
    public void Initialize(Path path, Sprite sprite, Color color, float difficultyMultiplier)
    {
        _path = path;
        
        //var spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = sprite;
        //spriteRenderer.color = color;

        //_healthScript = GetComponent<HealthScript>();
        //_healthScript.Initialize(enemyData.Health * (1 + difficultyMultiplier));

        _difficultyModifier = difficultyMultiplier;

        MoveToStart();
    }


    private void Update()
    {
        if (transform.position == _currentWayPoint)
        {
            SelectNextWayPoint();
        }
        
        Vector3 movePosition = Vector3.MoveTowards(transform.position, _currentWayPoint, enemyData.Speed * Time.deltaTime);
        transform.position = movePosition;
    }

    private void SelectNextWayPoint()
    {
        _currentWayPointIndex++;

        if (_currentWayPointIndex >= _path.GetNumberOfWayPoints())
        {
            DealDamageToPlayer();
            MoveToStart();
        }
        else
        {
            _currentWayPoint = _path.GetWayPointAtIndex(_currentWayPointIndex);
        }
    }

    private void DealDamageToPlayer()
    {
        var damage = enemyData.DamageToPlayer;

        //GameController.Instance.ModifyLivesBy(-damage, transform.position);
    }

    private void MoveToStart()
    {
        transform.position = _path.GetWayPointAtIndex(0);

        _currentWayPointIndex = 1;
        _currentWayPoint = _path.GetWayPointAtIndex(_currentWayPointIndex);
    }

    //how far is enemy
    public int GetPriority()
    {
        return _path.GetNumberOfWayPoints() - _currentWayPointIndex;
    }

    public void TakeDamage(float amount)
    {
        _healthScript.SubtractHealth(amount);

        if (_healthScript.Health == 0)
        {
            var reward = enemyData.RewardToPlayer * (1 + _difficultyModifier);

            //GameController.Instance.ModifyCurrencyBy((int)reward, transform.position);

            Destroy(gameObject);
        }
    }
}
