using System.Globalization;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using St = TheXDS.Vivianne.Resources.Strings.ValueConverters.FshBlobFormatLabelConverter;

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
            0x00 => St.NoImageLoaded,
            _ when Mappings.FshBlobToLabel.TryGetValue(value, out var label) => label,
            _ => string.Format(St.Unknown, (byte)value)
        };
    }
}
