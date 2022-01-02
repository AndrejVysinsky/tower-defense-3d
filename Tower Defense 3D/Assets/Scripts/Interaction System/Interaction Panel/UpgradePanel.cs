using Assets.Scripts.Network;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image upgradeImage;
    [SerializeField] Color upgradeAvailable;
    [SerializeField] Color noUpgradeAvailable;

    private TowerData _upgradeOption;
    private int _upgradeIndex;

    private bool _isUpgradeAvailable;

    public void SetUpgrade(TowerData upgradeOption, int upgradeIndex)
    {
        _isUpgradeAvailable = true;
        upgradeImage.color = upgradeAvailable;

        _upgradeOption = upgradeOption;
        _upgradeIndex = upgradeIndex;

        //enable cursor hand
    }

    public void SetNoUpgradeAvailable()
    {
        _isUpgradeAvailable = false;
        upgradeImage.color = noUpgradeAvailable;

        //disable cursor hand
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isUpgradeAvailable == false)
        {
            return;
        }

        var interactingObject = InteractionSystem.Instance.InteractingGameObject;

        if (interactingObject.TryGetComponent(out TowerBase towerBase))
        {
            var localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkPlayer>();
            localPlayer.UpgradeTower(towerBase, _upgradeOption.Price, _upgradeIndex);
        }
    }
}