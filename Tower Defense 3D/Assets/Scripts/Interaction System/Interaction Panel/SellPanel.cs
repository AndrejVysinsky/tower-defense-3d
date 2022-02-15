using UnityEngine;
using UnityEngine.EventSystems;

public class SellPanel : MonoBehaviour, IPointerDownHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        _towerBase.Sell();

        InteractionSystem.Instance.RefreshInteractions();
    }
}