using System.Globalization;
using System.Windows.Controls;
using TheXDS.Ganymede.Types;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Converts a collection of <see cref="ButtonInteraction"/> objects into an
/// enumeration of <see cref="MenuItem"/> objects.
/// </summary>
public class ButtonInteractionToMenuItemConverter : IOneWayValueConverter<IEnumerable<ButtonInteraction>, IEnumerable<MenuItem>>
{
    /// <inheritdoc/>
    public IEnumerable<MenuItem> Convert(IEnumerable<ButtonInteraction> value, object? parameter, CultureInfo? culture)
    {
        foreach (var item in value)
        {
            yield return new MenuItem()
            {
                Command = item.Command,
                Header = item.Text,
            };
        }
    }
}
