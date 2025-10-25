using System.Globalization;
using System.Text;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts raw bytes into a string representation.
/// </summary>
public class ByteToTextConverter : IOneWayValueConverter<byte[], string>
{
    /// <inheritdoc/>
    public string Convert(byte[] value, object? parameter, CultureInfo? culture)
    {
        return (parameter as Encoding ?? Encoding.Latin1).GetString(value);
    }
}