using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSellTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TooltipPositionEnum tooltipPosition;
    [SerializeField] RectTransform containerRect;
    [SerializeField] float positionOffset;

    private TowerData towerData;

    [Serializable]
    public enum TooltipPositionEnum
    {
        Above,
        Under,
        LeftOf,
        RightOf
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void OnMouseEnter()
    {
        ShowTooltip();
    }

    private void OnMouseDown()
    {
        ShowTooltip();
    }

    private void OnMouseExit()
    {
        HideTooltip();
    }

    private void OnDisable()
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (TryGetComponent(out SellPanel sellPanel))
        {
            towerData = sellPanel.GetTowerData();
        }

        if (towerData == null || TowerTooltip.Instance == null)
            return;

        var position = containerRect.transform.position;

        switch (tooltipPosition)
        {
            case TooltipPositionEnum.Above:
                position.y += containerRect.rect.height / 2 + positionOffset;
                position.x = transform.position.x;
                break;
            case TooltipPositionEnum.Under:
                position.y -= containerRect.rect.height / 2 + positionOffset;
                position.x = transform.position.x;
                break;
            case TooltipPositionEnum.LeftOf:
                position.x -= containerRect.rect.width / 2 + positionOffset;
                position.y = transform.position.y;
                break;
            case TooltipPositionEnum.RightOf:
                position.x += containerRect.rect.width / 2 + positionOffset;
                position.y = transform.position.y;
                break;
        }

        TowerSellTooltip.Instance.Show(towerData, position);
    }

    private void HideTooltip()
    {
        if (towerData == null || TowerSellTooltip.Instance == null)
            return;

        TowerSellTooltip.Instance.Hide();
    }
}
