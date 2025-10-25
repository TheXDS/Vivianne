using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a pair of integer values to a <see cref="Point"/>
/// structure.
/// </summary>
public class CoordsToPointConverter : IMultiValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is int x && values[1] is int y)
        {
            return new Point(x, y);
        }
        return default(Point);
    }

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value is Point p)
        {
            return [(int)p.X, (int)p.Y];
        }
        return [0, 0];
    }
}
