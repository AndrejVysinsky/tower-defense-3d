using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New General Tooltip With Header", menuName = "Tooltips/General Tooltip With Header")]
public class GeneralTooltipWithHeader : TooltipBase, ITooltipHeader
{
    [SerializeField] string header;

    public string Header => header;

    public override string GetTooltipText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(TooltipFont.GetFormattedHeader(Header)).AppendLine();
        builder.Append(TooltipFont.GetFormattedDescription(Description));

        return builder.ToString();
    }
}