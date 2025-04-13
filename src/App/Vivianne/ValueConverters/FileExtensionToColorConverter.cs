using System.Globalization;
using System.IO;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a converter that returns a color to be displayed for a specific
/// file type in the VIV directory.
/// </summary>
public class FileExtensionToColorConverter : IOneWayValueConverter<string, Brush>
{
    /// <inheritdoc/>
    public Brush Convert(string value, object? parameter, CultureInfo? culture)
    {
        return Path.GetExtension(value.ToLowerInvariant()) switch
        {
            ".txt" => Brushes.MediumSlateBlue,
            ".bnk" => Brushes.Coral,
            ".fce" => Brushes.ForestGreen,
            ".tga" or ".fsh" or ".qfs" => Brushes.CadetBlue,
            ".bri" or ".eng" or ".fre" or ".ger" or ".ita" or ".spa" or ".swe" => Brushes.MediumVioletRed,
            _ => Brushes.White
        };
    }
}