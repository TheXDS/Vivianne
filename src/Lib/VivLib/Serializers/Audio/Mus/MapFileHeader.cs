using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct MapFileHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;
    public byte Unk_0x04;
    public byte FirstSection;
    public byte NumberOfSections;
    public byte RecordSize;
    [Endianness(Endianness.BigEndian)]
    public int NumRecords;
}
