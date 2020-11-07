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

        builder.Append(Header).AppendLine();
        builder.Append(Description).AppendLine();

        if (ObjectData.TryGetComponent(out TowerBase towerBase))
        {
            if (towerBase.TowerData.MaxLevel == towerBase.Level)
            {
                builder.Append("Tower has reached max level!");
            }
            else
            {
                builder.Append(towerBase.TowerData.GetLevelData(towerBase.Level).Damage).Append(" => ")
                        .Append(towerBase.TowerData.GetLevelData(towerBase.Level + 1).Damage).AppendLine();
                builder.Append(towerBase.TowerData.GetLevelData(towerBase.Level + 1).Price).AppendLine();
            }
        }
        else
        {
            builder.Append("Missing tower data.").AppendLine();
        }

        return builder.ToString();
    }
}