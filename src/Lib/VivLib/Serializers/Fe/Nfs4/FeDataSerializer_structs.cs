using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs4;

public partial class FeDataSerializer
{
    private const byte Nfs4PursuitFlag_Mask = 0xF0;
    private const byte IsBonus_Mask = 0x01;
    private const byte Upgradable_Mask = 0x40;
    private const byte Roof_Mask = 0x03;
    private const byte IsDlc_Mask = 0x04;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct CompareTableItem
    {
        public byte DefaultValue;
        public byte Upgrade1;
        public byte Upgrade2;
        public byte Upgrade3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FeDataHeader
    {
        public static FeDataHeader Empty = new()
        {
            Magic = 0x04,
            Padding_0x1 = new byte[0x111],
            CarId = new byte[4],
            Padding_0x116 = new byte[0x208],
            Padding_0x320 = new byte[0x5a],
            Unk_0x386 = 0xb5,
            Unk_0x387 = 0x01,
            Unk_0x3ae = new byte[0x10],
            CompareCount = 0x0a
        };

        public byte Magic; // = 0x04
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x111)]
        public byte[] Padding_0x1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CarId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x208)]
        public byte[] Padding_0x116;
        public ushort SerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x5a)]
        public byte[] Padding_0x320;
        public byte PoliceFlag_IsBonus;
        public byte Upgradable_RoofFlags_IsDlc;
        public byte Unk_0x37c;
        public byte Unk_0x37d;
        public byte Unk_0x37e;
        public byte Unk_0x37f;
        public byte Unk_0x380;
        public byte Unk_0x381;
        public CarClass CarClass;
        public byte Unk_0x383;
        public byte Unk_0x384;
        public byte Unk_0x385;
        public byte Unk_0x386; // 0xb5 or 0x80
        public byte Unk_0x387; // 0x01 or 0x00
        public byte CompareCount; // x2, this seems to indicate the number of ushort values that form the compare tables (actual table values are 1 byte each), always = 0x0a
        public CompareTableItem Acceleration;
        public CompareTableItem TopSpeed;
        public CompareTableItem Handling;
        public CompareTableItem Braking;
        public CompareTableItem Overall;
        public byte Unk_0x39d;
        public int BasePrice;
        public int Upgrade1Price;
        public int Upgrade2Price;
        public int Upgrade3Price;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0d)]
        public byte[] Unk_0x3ae;
        public EngineLocation EngineLocation;
        public ushort Unk_0x3bc;
        public ushort StringEntries;
    }
}
