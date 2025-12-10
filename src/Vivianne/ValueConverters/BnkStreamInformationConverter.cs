using System.Globalization;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Info.Bnk;
using TheXDS.Vivianne.Models.Audio.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.BnkEditorViewModel;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Extracts and displays information on the <see cref="BnkStream"/>.
/// </summary>
public class BnkStreamInformationConverter : IOneWayValueConverter<BnkStream?, string>
{
    /// <inheritdoc/>
    public string Convert(BnkStream? value, object? parameter, CultureInfo? culture)
    {
        return value is null ? St.NoAudioStreamLoaded : string.Join(Environment.NewLine, new BnkStreamInfoExtractor(true).GetInfo(value));
    }
}
