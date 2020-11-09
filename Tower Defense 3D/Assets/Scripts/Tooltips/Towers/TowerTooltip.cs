using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Tooltip", menuName = "Tooltips/Tower Tooltip")]
public class TowerTooltip : DynamicTooltip, ITooltipHeader
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
            builder.Append(towerBase.TowerData.GetLevelData(1).Damage).AppendLine();
            builder.Append(towerBase.TowerData.GetLevelData(1).Price).AppendLine();
        }
        else
        {
            builder.Append("Missing tower data.").AppendLine();
        }

        return builder.ToString();
    }
}