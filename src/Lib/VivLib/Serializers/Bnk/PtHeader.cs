using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.Serializers.Bnk;

internal class PtHeader
{
    public static readonly PtHeader Default = new();

    public readonly Dictionary<PtHeaderField, PtHeaderValue> Values = [];
    public readonly Dictionary<PtAudioHeaderField, PtHeaderValue> AudioValues = new()
    {
        { PtAudioHeaderField.Channels, new(1, 1) },
        { PtAudioHeaderField.Compression, new(1, 0) },
        { PtAudioHeaderField.SampleRate, new(2, 22050) },
        { PtAudioHeaderField.NumSamples, new(2, 0) },
        { PtAudioHeaderField.LoopOffset, new(2, 0) },
        { PtAudioHeaderField.LoopEnd, new(2, 0) },
        { PtAudioHeaderField.DataOffset, new(4, 0) },
        { PtAudioHeaderField.BytesPerSample, new(1, 2) },
        { PtAudioHeaderField.EndOfHeader, new(4, 0) },
    };

    public PtHeaderValue this[PtHeaderField field]
    {
        get => Values[field];
        set => Values[field] = value;
    }

    public PtHeaderValue this[PtAudioHeaderField field]
    {
        get => AudioValues[field];
        set => AudioValues[field] = value;
    }

    public PtHeader? AltStream { get; set; }
}
