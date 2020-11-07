using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TooltipBase tooltipBase;

    public void SetTooltip(TooltipBase tooltip)
    {
        tooltipBase = tooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Instance.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

    private void OnDisable()
    {
        TooltipSystem.Instance.Hide();
    }
}
