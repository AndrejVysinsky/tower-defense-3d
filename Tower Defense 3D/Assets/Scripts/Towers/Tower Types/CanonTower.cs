using System.Collections;
using UnityEngine;

public class CanonTower : MonoBehaviour, ITowerType
{
    [SerializeField] TowerData towerData;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] TowerTargeting towerTargeting;

    private float _timer;

    public int Level { get; private set; }
    public TowerData TowerData => towerData;

    private void Start()
    {
        Upgrade();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (towerTargeting.Target != null && _timer >= towerData.GetLevelData(Level).AttackDelay)
        {
            _timer = 0;
            Shoot(towerTargeting.Target);
        }
    }

    public void Shoot(GameObject target)
    {
        var projectile = Instantiate(projectilePrefab, towerTargeting.GetFirePoint().transform.position, transform.rotation);

        projectile.GetComponent<CanonProjectile>().Initialize(target.transform.position, towerData.GetLevelData(Level).Damage);        
    }

    public void Upgrade()
    {
        Level++;
        towerTargeting.TowerSprite.sprite = towerData.GetLevelData(Level).Sprite;

        var price = towerData.GetLevelData(Level).Price;

        GameController.Instance.ModifyCurrencyBy(-price, transform.position);
    }

    public void Sell()
    {
        GridTowerPlacement.Instance.FreeTilePosition(transform.position);

        var sellValue = (int)(towerData.GetLevelData(Level).Price * towerData.SellFactor);

        GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }
}
