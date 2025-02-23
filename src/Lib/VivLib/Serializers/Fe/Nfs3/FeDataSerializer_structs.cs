using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs3;

public partial class FeDataSerializer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FeDataHeader
    {
        public static FeDataHeader Empty = new()
        {
            Unk_0x0c = 0x0003,
            FlagCount = 0x0009,
            Unk_0x2c = 0x05,
            StringEntries = 0x0028
        };
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CarId;
        public ushort FlagCount;
        public ushort IsBonus;
        public ushort AvailableToAi;
        public ushort CarClass;
        public ushort Unk_0x0c;// = 0x0003
        public ushort IsDlcCar;
        public ushort IsPolice;
        public ushort Seat;
        public ushort Unk_0x14; // = 0x0000
        public ushort Unk_0x16; // = 0x????
        public ushort SerialNumber;
        public ushort Unk_0x1a; // = 0x0000;
        public ushort Unk_0x1c; // = 0x0000;
        public ushort Unk_0x1e; // = 0x0000;
        public ushort Unk_0x20; // = 0x0000;
        public ushort Unk_0x22; // = 0x0000;
        public ushort Unk_0x24; // = 0x0000;
        public ushort Unk_0x26; // = 0x0000;
        public byte Accel;
        public byte TopSpeed;
        public byte Handling;
        public byte Braking;
        public byte Unk_0x2c; // = 0x05;
        public ushort StringEntries;
    }
}
