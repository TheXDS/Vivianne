using System.Globalization;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Bnk;
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
        return value is null ? TheXDS.Vivianne.Resources.Strings.ViewModels.BnkEditorViewModel.NoAudioStreamLoaded : string.Join(Environment.NewLine, [
            string.Format(St.BnkNfo_Duration, TimeSpan.FromSeconds((double)value.SampleData.Length / (value.SampleRate * value.BytesPerSample))),
            string.Format(St.BnkNfo_Samples, value.SampleData.Length / value.BytesPerSample),
            string.Format(St.BnkNfo_Channels, value.Channels),
            string.Format(St.BnkNfo_Format, value.BytesPerSample * 8, value.Compression ? "?" : "PCM"),
            string.Format(St.BnkNfo_SampleRate, value.SampleRate),
            string.Format(St.BnkNfo_Size, value.SampleData.LongLength.ByteUnits()),
            value.AltStream is null ? null : St.BnkNfo_AltStream]);
    }
}
