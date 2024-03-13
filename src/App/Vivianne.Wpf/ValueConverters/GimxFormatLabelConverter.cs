using System.Globalization;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns a description for the pixel format used in a GIMX file based on its
/// magic header.
/// </summary>
public class GimxFormatLabelConverter : IOneWayValueConverter<GimxFormat, string>
{
    /// <inheritdoc/>
    public string Convert(GimxFormat value, object? parameter, CultureInfo? culture)
    {
        return (byte)value switch
        {
            0x00 => "No image loaded",
            0x2a => "32-bit 256 color palette",
            0x7B => "8-bit color (256 colors)",
            0x78 => "16-bit color (BGR565)",
            0x7D => "24-bit color with 8-bit alpha channel (RGBA32)",
            _ => $"Unknown (MAGIC 0x{value:X}"
        };
    }
}
