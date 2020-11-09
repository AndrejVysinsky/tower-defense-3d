using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TooltipFont
{
    public static int HeaderSize { get; } = 32;
    public static int DescriptionSize { get; } = 27;

    public static string GetFormattedHeader(string header)
    {
        return $"<size={HeaderSize}><b>{header}</b></size>";
    }

    public static string GetFormattedDescription(string description)
    {
        return $"<size={DescriptionSize}>{description}</size>";
    }
}
