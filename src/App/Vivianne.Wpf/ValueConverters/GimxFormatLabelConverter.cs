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
        return value switch
        {
            0x00 => "No image loaded",
            GimxFormat.Palette => "32-bit 256 color palette",
            GimxFormat.Indexed8 => "8-bit color (256 colors) with palette",
            GimxFormat.Rgb565 => "16-bit color (RGB565), no alpha",
            GimxFormat.Argb32 => "24-bit color with 8-bit alpha channel (ARGB32)",
            _ => $"Unknown (0x{(byte)value:X}"
        };
    }
}
