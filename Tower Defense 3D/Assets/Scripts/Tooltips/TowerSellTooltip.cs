using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSellTooltip : MonoBehaviour
{
    public static TowerSellTooltip Instance { get; private set; }

    [SerializeField] GameObject tooltip;

    [Header("Tower elements")]
    [SerializeField] TMP_Text towerHeaderText;
    [SerializeField] TMP_Text towerCostText;

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

    public void Show(TowerData towerData, Vector2 position)
    {
        tooltip.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.GetComponent<RectTransform>());

        _tooltipRect.transform.position = position;

        towerHeaderText.text = towerData.Name;
        towerCostText.text = towerData.GetSellValue().ToString();
    }

    public void Hide()
    {
        if (tooltip == null)
            return;

        tooltip.SetActive(false);
    }
}