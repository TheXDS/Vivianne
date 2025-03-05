using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Bnk;

[StructLayout(LayoutKind.Sequential)]
internal struct BnkV2Header
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic; // = "BNKl"

    public short Version;

    public short Samples;

    public int PoolOffset;
}
