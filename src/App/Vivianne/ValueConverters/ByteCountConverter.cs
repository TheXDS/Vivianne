using System.Globalization;
using System.Windows;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns the human-readable array size in bytes.
/// </summary>
public class ByteCountConverter : IOneWayValueConverter<byte[], string>
{
    /// <inheritdoc/>
    public string Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return Common.ByteUnits(value?.Length ?? 0);
    }
}
