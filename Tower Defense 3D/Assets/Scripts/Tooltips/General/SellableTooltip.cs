using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sellable Tooltip", menuName = "Tooltips/Sellable Tooltip")]
public class SellableTooltip : TooltipBase, ITooltipHeader, ITooltipPrice
{
    [SerializeField] string header;
    [SerializeField] int price;

    public string Header => header;

    /*
     * Price is set depending on situation:
     *  1. object has some upgrades -> Price is set through objects code
     *  2. buying object from menu -> Price is set in inspector because it does not have objects reference
     */
    public int Price
    {
        get
        {
            return price;
        }
        set
        {
            price = value;
        }
    }

    public override string GetTooltipText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(TooltipFont.GetFormattedHeader(Header)).AppendLine();
        builder.Append(TooltipFont.GetFormattedDescription(Description)).AppendLine();
        builder.Append(Price).Append(TooltipIcons.GetFormattedIconText(TooltipIcons.GoldIcon)).AppendLine();

        return builder.ToString();
    }
}