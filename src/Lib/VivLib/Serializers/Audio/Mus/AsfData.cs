using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

internal class AsfData
{
    public AsfFile ToFile()
    {
        if (PtHeader is null)
        {
            throw new InvalidOperationException("PT Header is not set. (Missing SCHl ASF block?)");
        }
        if (AudioBlocks.Count != BlockCount)
        {
            throw new InvalidOperationException($"Expected {BlockCount} audio blocks, but found {AudioBlocks.Count}.");
        }

        var file = new AsfFile
        {
            Channels = (byte)PtHeader.AudioValues[PtAudioHeaderField.Channels].Value,
            SampleRate = (ushort)PtHeader.AudioValues[PtAudioHeaderField.SampleRate].Value,
            BytesPerSample = (byte)PtHeader.AudioValues[PtAudioHeaderField.BytesPerSample].Value,
            Compression = (CompressionMethod)PtHeader.AudioValues[PtAudioHeaderField.Compression].Value,
            LoopStart = PtHeader.AudioValues.GetValueOrDefault(PtAudioHeaderField.LoopOffset, default).Value,
            LoopEnd = PtHeader.AudioValues.GetValueOrDefault(PtAudioHeaderField.LoopEnd, default).Value,
            Properties = new Dictionary<byte, PtHeaderValue>(ToProps(PtHeader.Values)),
            LoopOffset = LoopOffset,
            ByteAlignment = ByteAlignment,
        };
        file.AudioBlocks.AddRange(AudioBlocks);
        return file;
    }

    public int BlockCount { get; set; }

    public PtHeader PtHeader { get; set; } = null!;

    public List<byte[]> AudioBlocks { get; } = [];

    public int? LoopOffset { get; set; } = null;

    public byte? ByteAlignment { get; set; } = null;

    private static IEnumerable<KeyValuePair<byte, PtHeaderValue>> ToProps<T>(IEnumerable<KeyValuePair<T, PtHeaderValue>> props) where T : Enum
    {
        return props.Select(p => new KeyValuePair<byte, PtHeaderValue>((byte)(p.Key.ToUnderlyingType()), p.Value));
    }
}
