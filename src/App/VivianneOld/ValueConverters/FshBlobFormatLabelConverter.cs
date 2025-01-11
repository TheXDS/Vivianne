using System.Globalization;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns a description for the pixel format used in a FSH blob based on its
/// magic header.
/// </summary>
public class FshBlobFormatLabelConverter : IOneWayValueConverter<FshBlobFormat, string>
{
    /// <inheritdoc/>
    public string Convert(FshBlobFormat value, object? parameter, CultureInfo? culture)
    {
        return value switch
        {
            0x00 => "No image loaded",
            _ when Mappings.FshBlobToLabel.TryGetValue(value, out var label) => label,
            _ => $"Unknown (0x{(byte)value:X}"
        };
    }
}
