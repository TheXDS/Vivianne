using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;
using St = TheXDS.Vivianne.Resources.Strings.Info.Bnk.BnkStreamInfoExtractor;

namespace TheXDS.Vivianne.Info.Bnk;

/// <summary>
/// Implements an information extractor for <see cref="BnkStream"/> entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BnkStreamInfoExtractor"/>
/// class.
/// </remarks>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
public class BnkStreamInfoExtractor(bool humanSize) : IEntityInfoExtractor<BnkStream>
{
    /// <inheritdoc/>
    public string[] GetInfo(BnkStream value)
    {
        return [.. new string?[]{
            string.Format(St.BnkNfo_Duration, TimeSpan.FromSeconds((double)value.SampleData.Length / (value.SampleRate * value.BytesPerSample))),
            string.Format(St.BnkNfo_Samples, value.SampleData.Length / value.BytesPerSample),
            string.Format(St.BnkNfo_Channels, value.Channels),
            string.Format(St.BnkNfo_Format, value.BytesPerSample * 8, value.Compression ? "?" : "PCM"),
            string.Format(St.BnkNfo_SampleRate, value.SampleRate),
            string.Format(St.BnkNfo_Size, value.SampleData.Length.GetSize(humanSize)),
            string.Format("Data after audio stream: {0}", value.PostAudioStreamData.Length.GetSize(humanSize)),
            value.AltStream is null ? null : St.BnkNfo_AltStream
        }.NotNull()];
    }
}

public class AsfFileInfoExtractor(bool humanSize) : IEntityInfoExtractor<AsfFile>
{
    public string[] GetInfo(AsfFile value)
    {
        var totalSamples = value.AudioBlocks.Sum(p => p.Length);

        return [.. new string?[]{
            string.Format(St.BnkNfo_Duration, TimeSpan.FromSeconds((double)totalSamples / (value.SampleRate * value.BytesPerSample))),
            string.Format(St.BnkNfo_Samples, (double)totalSamples / value.BytesPerSample),
            string.Format(St.BnkNfo_Channels, value.Channels),
            string.Format(St.BnkNfo_Format, value.BytesPerSample * 8, value.Compression ? "EAADPCM" : "PCM"),
            string.Format(St.BnkNfo_SampleRate, value.SampleRate),
            string.Format(St.BnkNfo_Size, totalSamples.GetSize(humanSize)),
        }.NotNull()];
    }
}