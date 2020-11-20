using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [SerializeField] Canvas tooltipCanvas;
    [SerializeField] GameObject tooltip;
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] LayoutElement tooltipLayoutElement;
    [SerializeField] float offsetYUnder;
    [SerializeField] float offsetYAbove;
    [SerializeField] float offsetXLeft;
    [SerializeField] float offsetXRight;

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

        float pivotX = 0;
        float pivotY = 1;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0f;

        float tooltipWidth = _tooltipRect.rect.width * tooltipCanvas.scaleFactor;
        float tooltipHeight = _tooltipRect.rect.height * tooltipCanvas.scaleFactor;

        bool isOutOfLeftEdge = mousePos.x - Mathf.Abs(offsetXLeft) - _tooltipRect.rect.width < 0;
        if (isOutOfLeftEdge)
            pivotX = 0;

        bool isOutOfRightEdge = mousePos.x + offsetXRight + tooltipWidth > Screen.width;
        if (isOutOfRightEdge)
            pivotX = 1;

        bool isOutOfTopEdge = mousePos.y + offsetYAbove + tooltipHeight > Screen.height;
        if (isOutOfTopEdge)
            pivotY = 1;

        bool isOutOfBottomEdge = mousePos.y - Mathf.Abs(offsetYUnder) - tooltipHeight < 0;
        if (isOutOfBottomEdge)
            pivotY = 0;


        _tooltipRect.pivot = new Vector2(pivotX, pivotY);

        var offset = new Vector3
        {
            x = pivotX == 0 ? offsetXRight : offsetXLeft,
            y = pivotY == 0 ? offsetYAbove : offsetYUnder,
            z = 0
        };

        _tooltipRect.transform.position = mousePos + offset;
    }

    public void Show(TooltipBase tooltipItem)
    {
        tooltip.SetActive(true);
        tooltipText.text = tooltipItem.GetTooltipText();

        var textSize = tooltipText.GetPreferredValues(tooltipText.text);

        tooltipLayoutElement.enabled = textSize.x > tooltipLayoutElement.preferredWidth;

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        if (tooltip == null)
            return;

        tooltip.SetActive(false);
    }
}