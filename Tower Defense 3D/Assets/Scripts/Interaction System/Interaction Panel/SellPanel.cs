using UnityEngine;
using UnityEngine.EventSystems;

public class SellPanel : MonoBehaviour, IPointerDownHandler
{
    private ISellable _sellable;

    public void SetSell(ISellable sellable)
    {
        _sellable = sellable;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _sellable.Sell();

        InteractionSystem.Instance.RefreshInteractions();
    }
}