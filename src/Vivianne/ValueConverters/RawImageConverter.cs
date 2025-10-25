using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Loads a raw image file into a <see cref="BitmapSource"/>, determining the
/// codec to use automatically.
/// </summary>
public class RawImageConverter : RawImageConverterBase, IMultiValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is byte[] rawBytes && values[1] is bool alpha)
        {
            return GetBitmap(rawBytes, null, alpha)!;
        }
        return null;
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
