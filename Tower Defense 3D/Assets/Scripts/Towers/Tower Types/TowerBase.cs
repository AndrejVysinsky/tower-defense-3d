using UnityEngine;
using UnityEngine.Events;

public class TowerBase : MonoBehaviour, IConstruction
{
    [SerializeField] TowerData towerData;

    public int Level { get; private set; }
    public TowerData TowerData => towerData;

    public bool IsUnderConstruction { get; private set; }
    public bool IsAbleToStartConstruction => GameController.Instance.Currency >= TowerData.GetLevelData(Level + 1).Price;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        var interactions = GetComponent<InteractionList>().Interactions;

        interactions.Find(x => x.InteractionName == "Upgrade").InteractionActions.Add(new UnityAction(Upgrade));
        interactions.Find(x => x.InteractionName == "Sell").InteractionActions.Add(new UnityAction(Sell));
    }

    public virtual void Upgrade()
    {
        Level++;
        GetComponent<MeshRenderer>().material = TowerData.GetLevelData(Level).Material;

        var price = TowerData.GetLevelData(Level).Price;

        GameController.Instance.ModifyCurrencyBy(-price, transform.position);

        if (Level == TowerData.MaxLevel)
        {
            var interactionMenu = GetComponent<InteractionList>();

            EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionRemoved("Upgrade", new UnityAction(Upgrade)));
        }
    }

    public virtual void Sell()
    {
        EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionHidden());

        var sellValue = TowerData.GetSellValue(Level);

        GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }

    public virtual void OnConstructionStarted()
    {
        IsUnderConstruction = true;

        //simulate construction time
        //after finished
        OnConstructionFinished();
    }

    public virtual void OnConstructionFinished()
    {
        IsUnderConstruction = false;
        Upgrade();
    }

    public virtual void OnConstructionCanceled()
    {
        IsUnderConstruction = false;
    }
}