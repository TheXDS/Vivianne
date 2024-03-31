using System.Globalization;
using System.Windows.Data;

namespace TheXDS.Vivianne.ValueConverters;

public class CoordsToPointConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is int x && values[1] is int y)
        {
            return new System.Windows.Point(x, y);
        }
        return default(System.Windows.Point);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value is System.Windows.Point p)
        {
            return [(int)p.X, (int)p.Y];
        }
        return [0, 0];
    }
}
