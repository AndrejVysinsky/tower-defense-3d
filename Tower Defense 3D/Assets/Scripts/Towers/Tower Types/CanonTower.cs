using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CanonTower : MonoBehaviour, ITowerType
{
    [SerializeField] TowerData towerData;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] TowerTargeting towerTargeting;

    private float _timer;

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
        if (Level == towerData.MaxLevel)
        {
            //MethodBase.GetCurrentMethod().Name dava blbosti
            var interactionMenu = GetComponent<InteractionList>();

            EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionRemoved("Upgrade", new UnityAction(Upgrade)));
            return;
        }

        Level++;
        //towerTargeting.TowerSprite.sprite = towerData.GetLevelData(Level).Sprite;

        var price = towerData.GetLevelData(Level).Price;

        //GameController.Instance.ModifyCurrencyBy(-price, transform.position);
    }

    public void Sell()
    {
        EventManager.ExecuteEvent<IInteractionChanged>((x, y) => x.OnInteractionHidden());

        //GridTowerPlacement.Instance.FreeTilePosition(transform.position);

        var sellValue = (int)(towerData.GetLevelData(Level).Price * towerData.SellFactor);

        //GameController.Instance.ModifyCurrencyBy(sellValue, transform.position);

        Destroy(gameObject);
    }
}
