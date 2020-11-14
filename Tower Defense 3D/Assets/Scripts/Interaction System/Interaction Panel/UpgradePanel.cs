using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;

    private IUpgradable _upgradable;
    private IUpgradeOption _upgradeOption;
    private int _upgradeIndex;

    public void SetUpgrade(IUpgradable upgradable, IUpgradeOption upgradeOption, int upgradeIndex)
    {
        _upgradable = upgradable;
        _upgradeOption = upgradeOption;
        _upgradeIndex = upgradeIndex;

        upgradeImage.sprite = upgradeOption.Sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _upgradable.Upgrade(_upgradeOption, _upgradeIndex);
    }
}