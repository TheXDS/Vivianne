using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a <see cref="BackgroundType"/> to a <see cref="Brush"/>.
/// </summary>
public class BackgroundToBrushConverter : IOneWayValueConverter<BackgroundType, Brush?>
{
    /// <inheritdoc/>
    public Brush? Convert(BackgroundType value, object? parameter, CultureInfo? culture)
    {
        return value switch
        {
            BackgroundType.Checkerboard => Application.Current.FindResource("CheckerboardBrush") as Brush,
            BackgroundType.Black => Brushes.Black,
            BackgroundType.White => Brushes.White,
            BackgroundType.Magenta => Brushes.Magenta,
            _ => null
        };
    }
}

public class CollectionToObservableWrapConverter : IValueConverter<ICollection<double>, ObservableCollection<double>>
{
    public ObservableCollection<double> Convert(ICollection<double> value, object? parameter, CultureInfo? culture)
    {
        return value is ObservableCollection<double> collection ? collection : new ObservableCollection<double>(value);
    }

    public ICollection<double> ConvertBack(ObservableCollection<double> value, object? parameter, CultureInfo culture)
    {
        return value;
    }
}