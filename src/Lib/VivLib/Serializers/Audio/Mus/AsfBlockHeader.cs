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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MapFileHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;
    public byte Unknown1;
    public byte FirstSection;
    public byte NumberOfSections;
    public byte RecordSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Unknown2;
    public byte NumRecords;
}

public struct MapFileSection
{
    public byte Index;
    public byte NumRecords;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Id;
    public ushort Size;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public MapSectionRecord[] Records;
}

public struct MapSectionRecord
{
    public byte Unk_0x0;
    public byte Magic;
    public byte NextSection;
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
            Compression = (CompressionMethod)PtHeader.AudioValues[PtAudioHeaderField.Compression].Value,
            Properties = new Dictionary<byte, PtHeaderValue>(),
        };
        file.AudioBlocks.AddRange(AudioBlocks);
        return file;
    }


    public int BlockCount { get; set; }

    public PtHeader PtHeader { get; set; } = null!;

    public List<byte[]> AudioBlocks { get; } = [];

}
