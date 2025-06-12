using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct SplitChunkHeader
{
    public int OutSize;
    public int LeftChannelOffset;
    public int RightChannelOffset;
}
