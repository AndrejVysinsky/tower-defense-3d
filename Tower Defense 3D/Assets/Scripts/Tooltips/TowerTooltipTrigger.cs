using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TowerData towerData;
    [SerializeField] TooltipPositionEnum tooltipPosition;
    [SerializeField] RectTransform containerRect;
    [SerializeField] float positionOffset;

    [Serializable]
    public enum TooltipPositionEnum
    {
        Above,
        Under,
        LeftOf,
        RightOf
    }

    public void SetTowerData(TowerData newTowerData)
    {
        towerData = newTowerData;
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
        if (TryGetComponent(out UpgradePanel upgradePanel))
        {
            towerData = upgradePanel.GetUpgradeData();
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

        TowerTooltip.Instance.Show(towerData, position);
    }

    private void HideTooltip()
    {
        if (towerData == null || TowerTooltip.Instance == null)
            return;

        TowerTooltip.Instance.Hide();
    }
}
