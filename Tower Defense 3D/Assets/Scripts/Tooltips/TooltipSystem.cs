using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [SerializeField] Canvas tooltipCanvas;
    [SerializeField] GameObject tooltip;
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] Vector3 offset;
    [SerializeField] float padding;

    private RectTransform _tooltipRect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _tooltipRect = tooltip.GetComponent<RectTransform>();
    }

    private void Update()
    {
        FollowCursor();
    }

    private void FollowCursor()
    {
        if (tooltip.activeSelf == false)
            return;

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + _tooltipRect.rect.width * 2 * tooltipCanvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - _tooltipRect.rect.width * tooltipCanvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + _tooltipRect.rect.height * tooltipCanvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }
        _tooltipRect.transform.position = newPos;
    }

    public void Show(TooltipBase tooltipItem)
    {
        tooltip.SetActive(true);
        tooltipText.text = tooltipItem.GetTooltipText();

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        tooltip.SetActive(false);
    }
}