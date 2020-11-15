using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sellable Tooltip", menuName = "Tooltips/Sellable Tooltip")]
public class SellableTooltip : TooltipBase, ITooltipHeader, ITooltipPrice
{
    [SerializeField] string header;

    public string Header => header;

    // Duplicity with BuyableTooltip....
    public int Price { get; set; }

    public override string GetTooltipText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(TooltipFont.GetFormattedHeader(Header)).AppendLine();
        builder.Append(TooltipFont.GetFormattedDescription(Description)).AppendLine();
        builder.Append(Price).Append(TooltipIcons.GetFormattedIconText(TooltipIcons.GoldIcon)).AppendLine();

        return builder.ToString();
    }
}