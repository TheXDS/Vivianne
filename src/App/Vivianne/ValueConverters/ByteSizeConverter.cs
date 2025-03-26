using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts an <see cref="int"/> value into a string that represents a
/// human-readable file size in bytes.
/// </summary>
public class ByteSizeConverter : IOneWayValueConverter<int, string>
{
    /// <inheritdoc/>
    public string Convert(int value, object? parameter, CultureInfo? culture)
    {
        return Common.ByteUnits(value);
    }
}
