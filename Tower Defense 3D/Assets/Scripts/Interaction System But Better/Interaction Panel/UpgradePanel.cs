using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;

    private IUpgradable _upgradable;
    private IUpgradeOption _upgradeOption;

    public void SetUpgrade(IUpgradable upgradable, IUpgradeOption upgradeOption)
    {
        _upgradable = upgradable;
        _upgradeOption = upgradeOption;

        upgradeImage.sprite = upgradeOption.Sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _upgradable.Upgrade(_upgradeOption);
    }
}