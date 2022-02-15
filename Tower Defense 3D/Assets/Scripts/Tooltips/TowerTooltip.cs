﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerTooltip : MonoBehaviour
{
    public static TowerTooltip Instance { get; private set; }

    [SerializeField] GameObject tooltip;

    [Header("Tower elements")]
    [SerializeField] TMP_Text towerHeaderText;
    [SerializeField] TMP_Text towerCostText;
    [SerializeField] TMP_Text towerDamageText;
    [SerializeField] TMP_Text towerAttackSpeedText;
    [SerializeField] TMP_Text towerRangeText;
    [SerializeField] TMP_Text towerDescriptionText;

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
        towerCostText.text = towerData.Price.ToString();
        towerDamageText.text = towerData.Damage.ToString();
        towerAttackSpeedText.text = towerData.AttackDelay.ToString();
        towerRangeText.text = towerData.Radius.ToString();
        towerDescriptionText.text = towerData.Description;
    }

    public void Hide()
    {
        if (tooltip == null)
            return;

        tooltip.SetActive(false);
    }
}