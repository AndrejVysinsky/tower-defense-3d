using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TooltipBase tooltipBase;
    [SerializeField] bool isBuildOption;

    private bool _isQuitting = false;

    void OnEnable()
    {
        if (isBuildOption && tooltipBase != null)
        {
            var buildPrefab = GetComponent<BuildOption>().BuildPrefab;

            if (buildPrefab.TryGetComponent(out IUpgradeable upgradable))
            {
                if (tooltipBase is BuyableTooltip buyableTooltip)
                {
                    buyableTooltip.Price = upgradable.CurrentUpgrade.Price;
                }
            }
        }
    }

    public void SetTooltip(TooltipBase tooltip)
    {
        tooltipBase = tooltip;
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
        if (_isQuitting)
            return;

        HideTooltip();
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void ShowTooltip()
    {
        if (tooltipBase == null || TooltipSystem.Instance == null)
            return;

        TooltipSystem.Instance.Show(tooltipBase);
    }

    private void HideTooltip()
    {
        if (tooltipBase == null || TooltipSystem.Instance == null)
            return;

        TooltipSystem.Instance.Hide();
    }
}
