using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sell Tower Tooltip", menuName = "Tooltips/Sell Tower Tooltip")]
public class SellTooltip : DynamicTooltip, ITooltipHeader
{
    [SerializeField] string header;

    public string Header => header;

    public override string GetTooltipText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(TooltipFont.GetFormattedHeader(Header)).AppendLine();
        builder.Append(TooltipFont.GetFormattedDescription(Description)).AppendLine();

        if (ObjectData.TryGetComponent(out TowerBase towerBase))
        {
            builder.Append(towerBase.TowerData.GetSellValue(towerBase.Level)).AppendLine();
        }
        else
        {
            builder.Append("Missing tower data.").AppendLine();
        }

        return builder.ToString();
    }
}