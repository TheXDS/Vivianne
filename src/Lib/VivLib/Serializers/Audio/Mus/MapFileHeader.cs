using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct MapFileHeader
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
