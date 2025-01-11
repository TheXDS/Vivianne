using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns a description of the footer data that may be present after the
/// pixel data in a FSH blob.
/// </summary>
public class FshBlobFooterIdentifierConverter : IOneWayValueConverter<FshBlob?, string>
{
    /// <inheritdoc/>
    public string Convert(FshBlob? value, object? parameter, CultureInfo? culture)
    {
        foreach (var j in Mappings.FshBlobFooterIdentifier)
        {
            if (j.Value.Invoke(value?.Footer!))
            {
                return Mappings.FshBlobFooterToLabel.TryGetValue(j.Key, out var label)
                    ? label
                    : $"Other ({j.Key})";
            }
        }
        return $"Unknown ({((long)(value?.Footer?.Length ?? 0)).ByteUnits()})";
    }
}