using Assets.Scripts.Network;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;

    private IUpgradeable _upgradable;
    private IUpgradeOption _upgradeOption;
    private int _upgradeIndex;

    public void SetUpgrade(IUpgradeable upgradable, IUpgradeOption upgradeOption, int upgradeIndex)
    {
        _upgradable = upgradable;
        _upgradeOption = upgradeOption;
        _upgradeIndex = upgradeIndex;

        upgradeImage.sprite = upgradeOption.Sprite;
        upgradeImage.preserveAspect = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var interactingObject = InteractionSystem.Instance.InteractingGameObject;

        if (interactingObject.TryGetComponent(out TowerBase towerBase))
        {
            var localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkPlayer>();
            localPlayer.UpgradeTower(towerBase, _upgradeOption.Price, _upgradeIndex);
        }
    }
}