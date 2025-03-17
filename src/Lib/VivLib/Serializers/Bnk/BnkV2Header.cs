using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Bnk;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct BnkV2Header
{
    public static BnkV2Header Empty() => new()
    {
        Magic = "BNKl"u8.ToArray(),
        Version = 2,
    };

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;

    public short Version;

    public short Samples;

    public int PoolOffset;
}
