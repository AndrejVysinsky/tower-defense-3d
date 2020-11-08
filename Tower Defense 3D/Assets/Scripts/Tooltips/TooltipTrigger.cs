using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TooltipBase tooltipBase;

    private bool _isQuitting = false;

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
        if (tooltipBase == null)
            return;

        TooltipSystem.Instance.Show(tooltipBase);
    }

    private void HideTooltip()
    {
        if (tooltipBase == null)
            return;

        TooltipSystem.Instance.Hide();
    }
}
