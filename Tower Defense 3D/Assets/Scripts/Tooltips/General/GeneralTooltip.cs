using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New General Tooltip", menuName = "Tooltips/General Tooltip")]
public class GeneralTooltip : TooltipBase
{
    public override string GetTooltipText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(TooltipFont.GetFormattedDescription(Description));

        return builder.ToString();
    }
}