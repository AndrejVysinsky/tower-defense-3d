using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;

    private IUpgradable _upgradable;
    private int _upgradeIndex;

    public void SetUpgrade(IUpgradable upgradable, IUpgradeOption upgradeOption, int upgradeIndex)
    {
        _upgradable = upgradable;
        _upgradeIndex = upgradeIndex;

        upgradeImage.sprite = upgradeOption.Sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _upgradable.Upgrade(_upgradeIndex);

        InteractionSystem.Instance.RefreshInteractions();
    }
}