using UnityEngine;
using UnityEngine.EventSystems;

public class SellPanel : MonoBehaviour
{
    private TowerBase _towerBase;

    public void SetTowerData(TowerBase towerBase)
    {
        _towerBase = towerBase;
    }

    public TowerData GetTowerData()
    {
        return _towerBase.TowerData;
    }

    public void SellTower()
    {
        _towerBase.Sell();

        InteractionSystem.Instance.RefreshInteractions();
    }
}