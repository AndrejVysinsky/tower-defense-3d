using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TooltipIcons
{
    public static string GoldIcon = "Gold";
    public static string SwordIcon = "Sword";

    public static string GetFormattedIconText(string icon)
    {
        return $"<sprite name=\"{icon}\">";
    }
}
