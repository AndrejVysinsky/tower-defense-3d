using UnityEngine;
using UnityEngine.Events;

public class CanonTower : MonoBehaviour, ITowerType, IConstruction
{
    [SerializeField] TowerData towerData;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] TowerTargeting towerTargeting;

    private float _timer;

    public int Level { get; private set; } = 0;
    public TowerData TowerData => towerData;

    public bool IsUnderConstruction { get; private set; }
    public bool IsAbleToStartConstruction => GameController.Instance.Currency >= towerData.GetLevelData(Level + 1).Price; 

    private void Awake()
    {
        towerTargeting.enabled = false;

        var interactions = GetComponent<InteractionList>().Interactions;

        interactions.Find(x => x.InteractionName == "Upgrade").InteractionActions.Add(new UnityAction(Upgrade));
        interactions.Find(x => x.InteractionName == "Sell").InteractionActions.Add(new UnityAction(Sell));
    }

    private void Update()
    {
        if (IsUnderConstruction || Level == 0)
            return;

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
        GetComponent<MeshRenderer>().material = towerData.GetLevelData(Level).Material;

        var price = towerData.GetLevelData(Level).Price;

        GameController.Instance.ModifyCurrencyBy(-price, transform.position);

        if (Level == towerData.MaxLevel)
        {
            var interactionMenu = GetComponent<InteractionList>();

            EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionRemoved("Upgrade", new UnityAction(Upgrade)));
        }
    }

    public void Sell()
    {
        EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionHidden());

        var sellValue = (int)(towerData.GetLevelData(Level).Price * towerData.SellFactor);

        GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }

    public void OnConstructionStarted()
    {
        towerTargeting.enabled = false;
        IsUnderConstruction = true;

        //simulate construction time

        //after finished
        OnConstructionFinished();
    }

    public void OnConstructionFinished()
    {
        towerTargeting.enabled = true;
        IsUnderConstruction = false;
        Upgrade();
    }

    public void OnConstructionCanceled()
    {
        IsUnderConstruction = false;
    }
}
