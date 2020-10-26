using UnityEngine;
using UnityEngine.Events;

public class LaserTower : MonoBehaviour, ITowerType
{
    [SerializeField] TowerData towerData;
    [SerializeField] LineRenderer laser;
    [SerializeField] TowerTargeting towerTargeting;

    public int Level { get; private set; } = 1;
    public TowerData TowerData => towerData;

    private void Awake()
    {
        var interactions = GetComponent<InteractionList>().Interactions;

        interactions.Find(x => x.InteractionName == "Upgrade").InteractionActions.Add(new UnityAction(Upgrade));
        interactions.Find(x => x.InteractionName == "Sell").InteractionActions.Add(new UnityAction(Sell));
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
        GetComponent<MeshRenderer>().material = towerData.GetLevelData(Level).Material;

        var price = towerData.GetLevelData(Level).Price;

        //GameController.Instance.ModifyCurrencyBy(-price, transform.position);

        if (Level == towerData.MaxLevel)
        {
            var interactionMenu = GetComponent<InteractionList>();

            EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionRemoved("Upgrade", new UnityAction(Upgrade)));
        }
    }

    public void Sell()
    {
        //GridTowerPlacement.Instance.FreeTilePosition(transform.position);

        var sellValue = (int)(towerData.GetLevelData(Level).Price * towerData.SellFactor);

        //GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }
}
