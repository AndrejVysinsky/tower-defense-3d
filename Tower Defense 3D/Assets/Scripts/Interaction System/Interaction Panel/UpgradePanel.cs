using Assets.Scripts.Network;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] GameObject upgradeButton;

    private TowerData _upgradeOption;
    private int _upgradeIndex;

    private bool _isUpgradeAvailable;

    public void SetUpgrade(TowerData upgradeOption, int upgradeIndex)
    {
        _isUpgradeAvailable = true;
        upgradeButton.SetActive(true);

        _upgradeOption = upgradeOption;
        _upgradeIndex = upgradeIndex;

        //enable cursor hand
    }

    public void SetNoUpgradeAvailable()
    {
        _isUpgradeAvailable = false;
        upgradeButton.SetActive(false);

        //disable cursor hand
    }

    public TowerData GetUpgradeData()
    {
        if (_isUpgradeAvailable == false)
            return null;

        return _upgradeOption;
    }

    public void UpgradeTower()
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