using System.Windows.Markup;
using TheXDS.MCART.Resources;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Markup extension that allows an <see cref="Enum"/> type to be specified on
/// XAML.
/// </summary>
public partial class EnumValProvider : MarkupExtension
{
    private Type? enumType;

    /// <summary>
    /// Type of <see cref="Enum"/> to use.
    /// </summary>
    public Type? EnumType
    {
        get => enumType;
        set
        {
            if (value is null || value.IsEnum)
            {
                enumType = value;
            }
            else
            {
                throw Errors.EnumExpected(nameof(value), value);
            }
        }
    }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return enumType is not null ? Enum.GetValues(enumType) : Array.Empty<Enum>();
    }
}
