using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a block of car information (FeData)
/// </summary>
public class FeData
{
    /// <summary>
    /// Enumerates all the known fedata file extensions.
    /// </summary>
    public static readonly string[] KnownExtensions = [".bri", ".eng", ".fre", ".ger", ".ita", ".spa", ".swe"];

    [StructLayout(LayoutKind.Sequential)]
    private struct FeDataHeader
    {
        public static FeDataHeader Empty = new()
        {
            Unk1 = 0x0009,
            Unk2 = 0x0003,
            Unk5 = 0x0080,
            Unk13 = 0x05,
            Unk14 = 0x28
        };

        public ushort Unk1;// = 0x0009
        public ushort IsBonus;
        public ushort AvailableToAi;
        public ushort CarClass;
        public ushort Unk2;// = 0x0003
        public ushort Unk3;// = 0x0000

        public ushort IsPolice;
        public ushort Seat;
        public ushort Unk4; // = 0x0000
        public ushort Unk5; // = 0x????
        public ushort SerialNumber;
        public ushort Unk6; // = 0x0000;
        public ushort Unk7; // = 0x0000;
        public ushort Unk8; // = 0x0000;

        public ushort Unk9; // = 0x0000;
        public ushort Unk10; // = 0x0000;
        public ushort Unk11; // = 0x0000;
        public ushort Unk12; // = 0x0000;
        public byte Accel;
        public byte TopSpeed;
        public byte Handling;
        public byte Braking;
        public byte Unk13; // = 0x05;
        public byte Unk14; // = 0x28;
        public byte Unk15; // = 0x00;
    }

    public string CarId { get; set; }
    public ushort SerialNumber { get; set; }
    public Nfs3CarClass VehicleClass { get; set; }
    public DriverSeatPosition Seat { get; set; }
    public bool IsPolice { get; set; }
    public bool IsBonus { get; set; }
    public bool AvailableToAi { get; set; }
    public byte CarAccel { get; set; }
    public byte CarTopSpeed { get; set; }
    public byte CarHandling { get; set; }
    public byte CarBraking { get; set; }

    [OffsetTableIndex(0)]
    public string Manufacturer { get; set; }
    [OffsetTableIndex(1)]
    public string Model { get; set; }
    [OffsetTableIndex(2)]
    public string CarName { get; set; }
    [OffsetTableIndex(3)]
    public string Price { get; set; }
    [OffsetTableIndex(4)]
    public string Status { get; set; }
    [OffsetTableIndex(5)]
    public string Weight { get; set; }
    [OffsetTableIndex(6)]
    public string WeightDistribution { get; set; }
    [OffsetTableIndex(7)]
    public string Length { get; set; }
    [OffsetTableIndex(8)]
    public string Width { get; set; }
    [OffsetTableIndex(9)]
    public string Height { get; set; }
    [OffsetTableIndex(10)]
    public string Engine { get; set; }
    [OffsetTableIndex(11)]
    public string Displacement { get; set; }
    [OffsetTableIndex(12)]
    public string Hp { get; set; }
    [OffsetTableIndex(13)]
    public string Torque { get; set; }
    [OffsetTableIndex(14)]
    public string MaxEngineSpeed { get; set; }
    [OffsetTableIndex(15)]
    public string Brakes { get; set; }
    [OffsetTableIndex(16)]
    public string Tires { get; set; }
    [OffsetTableIndex(17)]
    public string TopSpeed { get; set; }
    [OffsetTableIndex(18)]
    public string Accel0To60 { get; set; }
    [OffsetTableIndex(19)]
    public string Accel0To100 { get; set; }
    [OffsetTableIndex(20)]
    public string Transmission { get; set; }
    [OffsetTableIndex(21)]
    public string Gearbox { get; set; }
    [OffsetTableIndex(22)]
    public string History1 { get; set; }
    [OffsetTableIndex(23)]
    public string History2 { get; set; }
    [OffsetTableIndex(24)]
    public string History3 { get; set; }
    [OffsetTableIndex(25)]
    public string History4 { get; set; }
    [OffsetTableIndex(26)]
    public string History5 { get; set; }
    [OffsetTableIndex(27)]
    public string History6 { get; set; }
    [OffsetTableIndex(28)]
    public string History7 { get; set; }
    [OffsetTableIndex(29)]
    public string History8 { get; set; }
    [OffsetTableIndex(30)]
    public string Color1 { get; set; }
    [OffsetTableIndex(31)]
    public string Color2 { get; set; }
    [OffsetTableIndex(32)]
    public string Color3 { get; set; }
    [OffsetTableIndex(33)]
    public string Color4 { get; set; }
    [OffsetTableIndex(34)]
    public string Color5 { get; set; }
    [OffsetTableIndex(35)]
    public string Color6 { get; set; }
    [OffsetTableIndex(36)]
    public string Color7 { get; set; }
    [OffsetTableIndex(37)]
    public string Color8 { get; set; }
    [OffsetTableIndex(38)]
    public string Color9 { get; set; }
    [OffsetTableIndex(39)]
    public string Color10 { get; set; }

    public static FeData LoadFrom(byte[] contents)
    {
        using var ms = new MemoryStream(contents);
        using var reader = new BinaryReader(ms);
        string SeekAndRead(uint offset)
        {
            ms.Seek(offset, SeekOrigin.Begin);
            return reader.ReadNullTerminatedString(Encoding.Latin1);
        }
        var carId = Encoding.ASCII.GetString(reader.ReadBytes(4));
        var fedataHeader = reader.ReadStruct<FeDataHeader>();
        var data = new FeData
        {
            CarId = carId,
            IsBonus = fedataHeader.IsBonus == 1,
            AvailableToAi = fedataHeader.AvailableToAi == 1,
            VehicleClass = (Nfs3CarClass)fedataHeader.CarClass,
            IsPolice = fedataHeader.IsPolice == 1,
            Seat = (DriverSeatPosition)fedataHeader.Seat,
            SerialNumber = fedataHeader.SerialNumber,
            CarAccel = fedataHeader.Accel,
            CarTopSpeed = fedataHeader.TopSpeed,
            CarHandling = fedataHeader.Handling,
            CarBraking = fedataHeader.Braking,
        };
        uint[] offsets = new uint[40];
        for (int i = 0; i < 40; i++)
        {
            offsets[i] = reader.ReadUInt32();
        }
        foreach (var j in typeof(FeData).GetProperties())
        {
            if (j.GetAttribute<OffsetTableIndexAttribute>() is { Value: int offset })
            {
                j.SetValue(data, SeekAndRead(offsets[offset]));
            }
        }
        return data;
    }

    public byte[] Serialize()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Encoding.ASCII.GetBytes(CarId));
        writer.WriteStruct(FeDataHeader.Empty with
        {
            IsBonus = IsBonus ? (ushort)1 : (ushort)0,
            AvailableToAi = AvailableToAi ? (ushort)1 : (ushort)0,
            CarClass = (ushort)VehicleClass,
            IsPolice = IsPolice ? (ushort)1 : (ushort)0,
            Seat = (ushort)Seat,
            SerialNumber = SerialNumber,
            Accel = CarAccel,
            TopSpeed = CarTopSpeed,
            Handling = CarHandling,
            Braking = CarBraking,
        });
        uint lastOffset = 0xcf;
        using (var ms2 = new MemoryStream())
        {
            using (var writer2 = new BinaryWriter(ms2))
            {
                foreach (var j in typeof(FeData).GetProperties())
                {
                    if (j.GetAttribute<OffsetTableIndexAttribute>() is { Value: int offset })
                    {
                        string value = j.GetValue(this)?.ToString() ?? string.Empty;
                        writer.Write(lastOffset);
                        writer2.WriteNullTerminatedString(value);
                        lastOffset += (uint)value.Length + 1;
                    }
                }
            }
            writer.Write(ms2.ToArray());
        }
        return ms.ToArray();
    }
}
