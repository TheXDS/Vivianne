using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct AsfBlockHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;
    public int BlockSize;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct EaAdpcmStereoChunkHeader
{
    public int OutSize;
    public EaAdpcmInitialState LeftChannel;
    public EaAdpcmInitialState RightChannel;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct EaAdpcmMonoChunkHeader
{
    public int OutSize;
    public EaAdpcmInitialState MonoChannel;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct EaAdpcmInitialState
{
    public short CurrentSample;
    public short PreviousSample;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SplitChunkHeader
{
    public int OutSize;
    public int LeftChannelOffset;
    public int RightChannelOffset;
}











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
            Compression = PtHeader.AudioValues[PtAudioHeaderField.Compression].Value != 0,
            Properties = new Dictionary<byte, PtHeaderValue>(),
        };
        file.AudioBlocks.AddRange(AudioBlocks);
        return file;
    }


    public int BlockCount { get; set; }

    public PtHeader PtHeader { get; set; }

    public List<byte[]> AudioBlocks { get; } = [];

}
