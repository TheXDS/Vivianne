using System.Globalization;
using System.Windows;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Returns a visibility value when the source is equal to the specified value.
/// </summary>
public class ValueToVisibilityConverter : IOneWayValueConverter<object?, Visibility>
{
    /// <inheritdoc/>
    public Visibility Convert(object? value, object? parameter, CultureInfo? culture)
    {
        return (value?.Equals(parameter) ?? parameter is null) ? Visibility.Visible : Visibility.Collapsed;
    }
}