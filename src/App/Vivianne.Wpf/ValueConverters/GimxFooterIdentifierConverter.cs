using System.Globalization;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns a description of the footer data that may be present after the
/// pixel data in a GIMX texture.
/// </summary>
public class GimxFooterIdentifierConverter : IOneWayValueConverter<Gimx, string>
{
    /// <inheritdoc/>
    public string Convert(Gimx value, object? parameter, CultureInfo? culture)
    {
        return value.Footer.Length switch
        {
            0 => "No footer data present",
            104 => "Car dashboard data",
            int l when value.Footer.All(p => p == 0) => $"{l} padding zeros",
            int l => $"Unknown ({l} bytes)"
        };
    }
}