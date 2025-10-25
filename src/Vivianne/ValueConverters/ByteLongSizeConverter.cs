using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts an <see cref="long"/> value into a string that represents a
/// human-readable file size in bytes.
/// </summary>
public class ByteLongSizeConverter : IOneWayValueConverter<long, string>
{
    /// <inheritdoc/>
    public string Convert(long value, object? parameter, CultureInfo? culture)
    {
        return Common.ByteUnits(value);
    }
}