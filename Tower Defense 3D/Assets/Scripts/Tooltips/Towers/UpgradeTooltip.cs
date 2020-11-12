using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Tower Tooltip", menuName = "Tooltips/Upgrade Tower Tooltip")]
public class UpgradeTooltip : DynamicTooltip, ITooltipHeader
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
            if (towerBase.TowerData.MaxLevel == towerBase.Level)
            {
                builder.Append("Tower has reached max level!");
            }
            else
            {
                builder.Append(towerBase.TowerData.GetLevelData(towerBase.Level).Damage).Append(" --> ")
                        .Append(towerBase.TowerData.GetLevelData(towerBase.Level + 1).Damage).Append(TooltipIcons.GetFormattedIconText(TooltipIcons.SwordIcon)).AppendLine();
                builder.Append(towerBase.TowerData.GetLevelData(towerBase.Level + 1).Price).Append(TooltipIcons.GetFormattedIconText(TooltipIcons.GoldIcon)).AppendLine();
            }
        }
        else
        {
            builder.Append("Missing tower data.").AppendLine();
        }

        return builder.ToString();
    }
}