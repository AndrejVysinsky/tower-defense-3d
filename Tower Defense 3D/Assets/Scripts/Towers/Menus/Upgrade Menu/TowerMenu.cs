using TMPro;
using UnityEngine;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] TowerTargeting towerTargeting;
    [SerializeField] GameObject menu;
    [SerializeField] UpgradeOption upgradeOption;
    [SerializeField] TextMeshProUGUI sellText;
    [SerializeField] string menuOptionTag;

    private ITowerType _tower;

    private void Start()
    {
        _tower = GetComponentInParent<ITowerType>();

        upgradeOption.SetTower(_tower);
        upgradeOption.ShowNextUpgrade();

        var sellValue = (int)(_tower.TowerData.GetLevelData(_tower.Level).Price * _tower.TowerData.SellFactor);
        sellText.text = sellValue.ToString();

        menu.SetActive(false);
    }

    private void OnEnable()
    {
        //SelectionManager.Instance.OnGameObjectSelected += GameObjectSelected;
    }

    private void OnDisable()
    {
        //SelectionManager.Instance.OnGameObjectSelected -= GameObjectSelected;
    }

    private void GameObjectSelected(GameObject selectedGameObject)
    {
        //if (InputHandler.ClickedOnObjectInHierarchy(selectedGameObject, gameObject, menuOptionTag))
        //{
        //    ShowMenu();
        //}
        //else
        //{
        //    HideMenu();
        //}
    }

    private void ShowMenu()
    {
        if (menu.activeSelf)
            return;

        towerTargeting.RangeRenderer.ShowRange();
        menu.SetActive(true);
    }

    public void HideMenu()
    {
        if (menu.activeSelf == false)
            return;

        towerTargeting.RangeRenderer.HideRange();
        menu.SetActive(false);
    }

    public void UpgradeTower()
    {
        _tower.Upgrade();
        upgradeOption.ShowNextUpgrade();

        var sellValue = (int)(_tower.TowerData.GetLevelData(_tower.Level).Price * _tower.TowerData.SellFactor);
        sellText.text = sellValue.ToString();
    }

    public void SellTower()
    {
        _tower.Sell();
    }
}
