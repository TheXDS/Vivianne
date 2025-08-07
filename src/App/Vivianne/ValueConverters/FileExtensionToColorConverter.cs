using System.Globalization;
using System.IO;
using System.Windows.Media;
using TheXDS.MCART.ValueConverters.Base;
using Wpf.Ui.Controls;

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
            ".md" or ".nfo" => Brushes.SkyBlue,
            ".txt" or ".dat" or ".qda" => Brushes.MediumSlateBlue,
            ".bnk" or ".asf" or ".mus" => Brushes.Coral,
            ".fce" or ".geo" => Brushes.ForestGreen,
            ".tga" or ".fsh" or ".qfs" => Brushes.CadetBlue,
            ".bri" or ".eng" or ".fre" or ".ger" or ".ita" or ".spa" or ".swe" => Brushes.MediumVioletRed,
            _ => Brushes.White
        };
    }
}