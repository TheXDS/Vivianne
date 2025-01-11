using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.ValueConverters.Base;

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
