using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour//, ICurrencyChanged
{
    [SerializeField] Image upgradeImage;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] CurrencyColorData currencyColorData;

    private ITowerType _tower;

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);

        //hmm... doesnt look right
       // OnCurrencyChanged(GameController.Instance.Currency);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public void SetTower(ITowerType tower)
    {
        _tower = tower;

        if (_tower.Level + 1 <= _tower.TowerData.MaxLevel)
        {
            priceText.text = _tower.TowerData.GetLevelData(_tower.Level + 1).Price.ToString();
        }
    }

    public void ShowNextUpgrade()
    {
        //check if after this upgrade theres next one
        if (_tower.Level + 1 <= _tower.TowerData.MaxLevel)
        {
            //upgrade available
            var levelData = _tower.TowerData.GetLevelData(_tower.Level + 1);

            upgradeImage.sprite = levelData.Sprite;
            priceText.text = levelData.Price.ToString();
        }
        else
        {
            //no next upgrade
            gameObject.SetActive(false);
        }
    }

    public void OnCurrencyChanged(int total)
    {
        if (_tower == null)
            return;

        if (_tower.TowerData.GetLevelData(_tower.Level + 1).Price <= total)
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