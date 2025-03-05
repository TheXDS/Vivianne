using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Bnk;

internal class PtHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic = "PT\0\0"u8.ToArray();
    public byte Channels = 1;
    public bool Compression = false;
    public int SampleRate = 22050;
    public int NumSamples;
    public int LoopOffset = 1;
    public int LoopLength;
    public int DataOffset;
    public int BytesPerSample = 2;
}