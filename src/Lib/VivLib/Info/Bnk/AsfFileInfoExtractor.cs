using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Tools.Audio;
using St = TheXDS.Vivianne.Resources.Strings.Info.Bnk.BnkStreamInfoExtractor;

namespace TheXDS.Vivianne.Info.Bnk;

/// <summary>
/// Implements an information extractor for <see cref="AsfFile"/> entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AsfFileInfoExtractor"/>
/// class.
/// </remarks>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
public class AsfFileInfoExtractor(bool humanSize) : IEntityInfoExtractor<AsfFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(AsfFile value)
    {
        var totalBytes = value.AudioBlocks.Sum(p => p.Length);
        return [
            .. new string?[] {
                string.Format(St.BnkNfo_Duration, value.CalculatedDuration),
                string.Format(St.BnkNfo_Samples, value.TotalSamples),
                string.Format(St.BnkNfo_Channels, value.Channels),
                string.Format(St.BnkNfo_Format, value.BytesPerSample * 8, Mappings.AudioCodecDescriptions.GetValueOrDefault(value.Compression, "Unknown")),
                string.Format(St.BnkNfo_SampleRate, value.SampleRate),
                string.Format(St.BnkNfo_Size, totalBytes.GetSize(humanSize)),
                string.Format("PT Loop start: {0} ({1})", value.LoopStart, FromSample(value.LoopStart, value)),
                string.Format("PT Loop end: {0} ({1})", value.LoopEnd, FromSample(value.LoopEnd, value)),
                value.LoopOffset.HasValue ? string.Format("SCLl Loop offset: {0} ({1}) ", value.LoopOffset.Value, FromSample(value.LoopOffset.Value, value)) : null,
            }.NotNull(),
            .. value.Properties.Select(p => $"PTHeader {p.Key:X2}: {p.Value.Value} (0x{p.Value.Value:X8})")
        ];
    }

    private static TimeSpan FromSample(int sampleNumber, AudioStreamBase audioProps)
    {
        return TimeSpan.FromSeconds(sampleNumber / audioProps.Channels / audioProps.SampleRate);
    }
}

public class MusFileInfoExtractor(bool humanSize) : AsfFileInfoExtractor(humanSize), IEntityInfoExtractor<MusFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(MusFile value)
    {
        var jointSubStreams = AudioRender.JoinStreams(value.AsfSubStreams.Values);
        return GetInfo(jointSubStreams);
    }
}