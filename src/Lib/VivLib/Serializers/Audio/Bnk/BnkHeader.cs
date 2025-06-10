using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Bnk;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct BnkHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;
    public short Version;
    public short Streams;
    public int PoolOffset;
}
