using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct MapFileSection
{
    public byte Index;
    public byte NumRecords;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public MapSectionRecord[] Records;
}
