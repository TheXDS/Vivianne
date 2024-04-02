using System.Windows.Markup;


namespace TheXDS.Vivianne.Controls;

public partial class EnumValProvider : MarkupExtension
{
    public Type EnumType { get; set; }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(EnumType);
    }
}
