using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;

    private IUpgradeable _upgradable;
    private IUpgradeOption _upgradeOption;

    public void SetUpgrade(IUpgradeable upgradable, IUpgradeOption upgradeOption)
    {
        _upgradable = upgradable;
        _upgradeOption = upgradeOption;

        upgradeImage.sprite = upgradeOption.Sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _upgradable.OnUpgradeStarted(_upgradeOption, out bool upgradeStarted);

        if (upgradeStarted)
        {
            InteractionSystem.Instance.RefreshInteractions();
        }
    }
}