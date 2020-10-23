using UnityEngine;

public class LaserTower : MonoBehaviour, ITowerType
{
    [SerializeField] TowerData towerData;
    [SerializeField] LineRenderer laser;
    [SerializeField] TowerTargeting towerTargeting;
    
    public int Level { get; private set; }
    public TowerData TowerData => towerData;

    private void Start()
    {
        Upgrade();
    }

    private void Update()
    {
        if (towerTargeting.Target != null)
        {
            laser.enabled = true;
            Shoot(towerTargeting.Target);
        }
        else
        {
            laser.enabled = false;
        }
    }

    public void Shoot(GameObject target)
    {
        laser.SetPosition(0, towerTargeting.GetFirePoint().transform.position);
        laser.SetPosition(1, target.transform.position);

        target.GetComponent<Enemy>().TakeDamage(towerData.GetLevelData(Level).Damage * Time.deltaTime);
    }

    public void Upgrade()
    {
        Level++;
        towerTargeting.TowerSprite.sprite = towerData.GetLevelData(Level).Sprite;

        var price = towerData.GetLevelData(Level).Price;
        //GameController.Instance.ModifyCurrencyBy(-price, transform.position);
    }

    public void Sell()
    {
        //GridTowerPlacement.Instance.FreeTilePosition(transform.position);

        var sellValue = (int)(towerData.GetLevelData(Level).Price * towerData.SellFactor);

        //GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }
}
