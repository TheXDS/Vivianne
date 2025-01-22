using System.Globalization;
using System.Windows.Controls;
using TheXDS.Ganymede.Types;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

public class ButtonInteractionToMenuItemConverter : IOneWayValueConverter<IEnumerable<ButtonInteraction>, IEnumerable<MenuItem>>
{
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
