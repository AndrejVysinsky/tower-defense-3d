using TMPro;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [SerializeField] GameObject tooltip;
    [SerializeField] TextMeshProUGUI tooltipText;

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

    public void Show(string text = "")
    {
        tooltip.SetActive(true);
        tooltipText.text = text;
    }

    public void Hide()
    {
        tooltip.SetActive(false);
    }
}