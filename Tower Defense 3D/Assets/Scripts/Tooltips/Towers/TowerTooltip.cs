using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Tooltip", menuName = "Tooltips/Tower Tooltip")]
public class TowerTooltip : TooltipBase, ITooltipHeader, ITooltipPrice
{
    [SerializeField] string header;

    public string Header => header;
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