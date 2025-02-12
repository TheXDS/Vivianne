using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a serializer that can read and write <see cref="FeData3"/>
/// entities.
/// </summary>
public class FeData4Serializer : ISerializer<FeData4>
{
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
        public byte Magic; // = 0x04
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x111)]
        public byte[] Padding_0x1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string CarId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x208)]
        public byte[] Padding_0x116;
        public ushort SerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x5a)]
        public byte[] Padding_0x320;
        public byte PoliceFlag_IsBonus;
        public byte Upgradable_Convertible;
        public ushort Unk_0x37c;
        public ushort Unk_0x37e;
        public ushort Unk_0x380;
        public Nfs4CarClass CarClass;
        public byte Unk_0x383;
        public ushort Unk_0x384;
        public ushort Unk_0x386;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Unk_0x3ae;
        public ushort StringEntries;
    }

    private const byte Nfs4PursuitFlag_Mask = 0x1c;
    private const byte IsBonus_Mask = 0x01;
    private const byte Upgradable_Mask = 0x40;
    private const byte Convertible_Mask = 0x01;

    /// <inheritdoc/>
    public FeData4 Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        string SeekAndRead(uint offset)
        {
            if (offset > stream.Length)
            {
                Debug.Print(string.Format(St.FeDataSerializer_StringOutOfBounds, offset));
                return string.Empty;
            }
            stream.Seek(offset, SeekOrigin.Begin);
            return reader.ReadNullTerminatedString(Encoding.Latin1);
        }
        var fedataHeader = reader.MarshalReadStruct<FeDataHeader>();
        var data = new FeData4
        {
            CarId = fedataHeader.CarId,
            SerialNumber = fedataHeader.SerialNumber,
            PoliceFlag = (Nfs4PursuitFlag)(fedataHeader.PoliceFlag_IsBonus & Nfs4PursuitFlag_Mask),
            VehicleClass = fedataHeader.CarClass,
            IsBonus = (fedataHeader.PoliceFlag_IsBonus & IsBonus_Mask) != 0,
            Upgradable = (fedataHeader.Upgradable_Convertible & Upgradable_Mask) != 0,
            Convertible = (fedataHeader.Upgradable_Convertible & Convertible_Mask) != 0,
            StringEntries = fedataHeader.StringEntries,
            DefaultCompare = new Nfs4CompareTable()
            {
                Acceleration = fedataHeader.Acceleration.DefaultValue,
                TopSpeed = fedataHeader.TopSpeed.DefaultValue,
                Handling = fedataHeader.Handling.DefaultValue,
                Braking = fedataHeader.Braking.DefaultValue,
                Overall = fedataHeader.Overall.DefaultValue,
                Price = fedataHeader.BasePrice,
            },
            CompareUpg1 = new Nfs4CompareTable()
            {
                Acceleration = fedataHeader.Acceleration.Upgrade1,
                TopSpeed = fedataHeader.TopSpeed.Upgrade1,
                Handling = fedataHeader.Handling.Upgrade1,
                Braking = fedataHeader.Braking.Upgrade1,
                Overall = fedataHeader.Overall.Upgrade1,
                Price = fedataHeader.Upgrade1Price,
            },
            CompareUpg2 = new Nfs4CompareTable()
            {
                Acceleration = fedataHeader.Acceleration.Upgrade2,
                TopSpeed = fedataHeader.TopSpeed.Upgrade2,
                Handling = fedataHeader.Handling.Upgrade2,
                Braking = fedataHeader.Braking.Upgrade2,
                Overall = fedataHeader.Overall.Upgrade2,
                Price = fedataHeader.Upgrade2Price,
            },
            CompareUpg3 = new Nfs4CompareTable()
            {
                Acceleration = fedataHeader.Acceleration.Upgrade3,
                TopSpeed = fedataHeader.TopSpeed.Upgrade3,
                Handling = fedataHeader.Handling.Upgrade3,
                Braking = fedataHeader.Braking.Upgrade3,
                Overall = fedataHeader.Overall.Upgrade3,
                Price = fedataHeader.Upgrade3Price,
            }
        };
        uint[] offsets = new uint[fedataHeader.StringEntries];
        for (int i = 0; i < fedataHeader.StringEntries; i++)
        {
            offsets[i] = reader.ReadUInt32();
        }
        foreach (var j in typeof(FeData4).GetProperties())
        {
            if (j.GetAttribute<OffsetTableIndexAttribute>() is { Value: int offset })
            {
                j.SetValue(data, SeekAndRead(offsets[offset]));
            }
        }
        return data;
    }

    /// <inheritdoc/>
    public void SerializeTo(FeData4 entity, Stream stream)
    {
        //using var writer = new BinaryWriter(stream);
        //writer.Write(Encoding.ASCII.GetBytes(entity.CarId));
        //writer.WriteStruct(FeDataHeader.Empty with
        //{
        //    IsBonus = entity.IsBonus ? (ushort)1 : (ushort)0,
        //    AvailableToAi = entity.AvailableToAi ? (ushort)1 : (ushort)0,
        //    CarClass = (ushort)entity.VehicleClass,
        //    IsPolice = entity.IsPolice ? (ushort)1 : (ushort)0,
        //    Seat = (ushort)entity.Seat,
        //    IsDlcCar = entity.IsDlcCar,
        //    SerialNumber = entity.SerialNumber,
        //    Accel = entity.CarAccel,
        //    TopSpeed = entity.CarTopSpeed,
        //    Handling = entity.CarHandling,
        //    Braking = entity.CarBraking,
        //    StringEntries = entity.StringEntries,

        //    // TODO: Investigate what these values are:
        //    Unk_0x0c = entity.Unk_0x0c,
        //    Unk_0x14 = entity.Unk_0x14,
        //    Unk_0x16 = entity.Unk_0x16,
        //    Unk_0x1a = entity.Unk_0x1a,
        //    Unk_0x1c = entity.Unk_0x1c,
        //    Unk_0x1e = entity.Unk_0x1e,
        //    Unk_0x20 = entity.Unk_0x20,
        //    Unk_0x22 = entity.Unk_0x22,
        //    Unk_0x24 = entity.Unk_0x24,
        //    Unk_0x26 = entity.Unk_0x26,
        //    Unk_0x2c = entity.Unk_0x2c,
        //});
        //uint lastOffset = (uint)(0x2f + (entity.StringEntries * 4));
        //using var ms2 = new MemoryStream();
        //using (var writer2 = new BinaryWriter(ms2))
        //{
        //    foreach (var j in typeof(FeData3).GetProperties())
        //    {
        //        if (j.GetAttribute<OffsetTableIndexAttribute>() is not { Value: int offset }) continue;
        //        string value = j.GetValue(entity)?.ToString() ?? string.Empty;
        //        writer.Write(lastOffset);
        //        writer2.WriteNullTerminatedString(value, Encoding.Latin1);
        //        lastOffset += (uint)value.Length + 1;
        //    }
        //}
        //writer.Write(ms2.ToArray());
    }
}
