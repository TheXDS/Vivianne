using System.Globalization;
using System.IO;
using TheXDS.MCART.ValueConverters.Base;
using Wpf.Ui.Controls;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a converter that returns an icon to be displayed on each file in
/// the VIV directory.
/// </summary>
public class FileExtensionToIconConverter : IOneWayValueConverter<string, SymbolRegular>
{
    /// <inheritdoc/>
    public SymbolRegular Convert(string value, object? parameter, CultureInfo? culture)
    {
        return Path.GetExtension(value.ToLowerInvariant()) switch
        {
            ".md" or ".nfo" => SymbolRegular.Info24,
            ".txt" or ".dat" or ".qda" => SymbolRegular.Gauge24,
            ".bnk" => SymbolRegular.Speaker224,
            ".asf" or ".mus" => SymbolRegular.MusicNote120,
            ".fce" or ".geo" => SymbolRegular.VehicleCar24,
            ".tga" or ".fsh" or ".qfs" => SymbolRegular.Image24,
            ".bri" or ".eng" or ".fre" or ".ger" or ".ita" or ".spa" or ".swe" => SymbolRegular.Text32,
            _ => SymbolRegular.Document24
        };
    }
}
