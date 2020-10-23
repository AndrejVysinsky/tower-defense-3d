using TMPro;
using UnityEngine;

public class TowerBuyScript : MonoBehaviour, ICurrencyChanged
{
    [SerializeField] GameObject towerPrefab;
    
    [SerializeField] TowerIndicatorData towerIndicatorData;
    
    //parent for better scene hierarchy
    [SerializeField] GameObject towerContainer;

    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] CurrencyColorData currencyColorData;
    
    private Camera _mainCamera;
    private GridTowerPlacement _gridTowerPlacement;

    private TowerBuyingIndicator _towerBuyingIndicator;

    private void Start()
    {
        _mainCamera = Camera.main;
        _gridTowerPlacement = GridTowerPlacement.Instance;

        var towerIndicator = Instantiate(towerIndicatorData.TowerIndicator);
        var towerPlacementIndicator = Instantiate(towerIndicatorData.TowerPlacementIndicator);
        var touchInputIndicator = Instantiate(towerIndicatorData.TouchInputIndicator);

        _towerBuyingIndicator = new TowerBuyingIndicator(towerIndicator, towerPlacementIndicator, touchInputIndicator);
        _towerBuyingIndicator.HideIndicators();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener(gameObject);
    }

    private void Update()
    {
        if (_towerBuyingIndicator.AreIndicatorsActive())
        {
            var position = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            _towerBuyingIndicator.MoveTouchIndicator(position, Time.deltaTime);
        }
    }

    void OnMouseDown()
    {
        _towerBuyingIndicator.ShowIndicators(transform.position);
    }

    void OnMouseDrag()
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        if (_gridTowerPlacement.IsTileEmpty(mousePosition))
        {
            var tilePosition = _gridTowerPlacement.GetTilePosition(mousePosition);
            
            _towerBuyingIndicator.MoveTowerIndicators(tilePosition);
        }
    }

    private void OnMouseUp()
    {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (_gridTowerPlacement.IsTileEmpty(mousePosition))
        {
            _gridTowerPlacement.OccupyTilePosition(mousePosition);

            PlaceTower();
        }

        _towerBuyingIndicator.HideIndicators();
    }

    private void PlaceTower()
    {
        var placedTower = Instantiate(towerPrefab, towerContainer.transform);
        placedTower.transform.position = _towerBuyingIndicator.GetTowerPosition();
    }

    public void OnCurrencyChanged(int total)
    {
        var price = towerPrefab.GetComponent<ITowerType>().TowerData.GetLevelData(1).Price;

        if (price <= total)
        {
            priceText.color = currencyColorData.ActiveColor;
            GetComponent<Collider2D>().enabled = true;
        }
        else
        {
            priceText.color = currencyColorData.InactiveColor;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
