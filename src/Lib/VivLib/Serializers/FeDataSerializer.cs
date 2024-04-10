using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a serializer that can read and write <see cref="FeData"/>
/// entities.
/// </summary>
public class FeDataSerializer : ISerializer<FeData>
{
    [StructLayout(LayoutKind.Sequential)]
    private struct FeDataHeader
    {
        public static FeDataHeader Empty = new()
        {
            FlagCount = 0x0009,
            StringEntries = 0x0028
        };

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

    /// <inheritdoc/>
    public FeData Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        string SeekAndRead(uint offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
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
            IsDlcCar = fedataHeader.IsDlcCar,
            SerialNumber = fedataHeader.SerialNumber,
            CarAccel = fedataHeader.Accel,
            CarTopSpeed = fedataHeader.TopSpeed,
            CarHandling = fedataHeader.Handling,
            CarBraking = fedataHeader.Braking,

            // TODO: Investigate what these values are:
            Unk_0x0c = fedataHeader.Unk_0x0c,
            Unk_0x14 = fedataHeader.Unk_0x14,
            Unk_0x16 = fedataHeader.Unk_0x16,
            Unk_0x1a = fedataHeader.Unk_0x1a,
            Unk_0x1c = fedataHeader.Unk_0x1c,
            Unk_0x1e = fedataHeader.Unk_0x1e,
            Unk_0x20 = fedataHeader.Unk_0x20,
            Unk_0x22 = fedataHeader.Unk_0x22,
            Unk_0x24 = fedataHeader.Unk_0x24,
            Unk_0x26 = fedataHeader.Unk_0x26,
            Unk_0x2c = fedataHeader.Unk_0x2c,
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

    /// <inheritdoc/>
    public void SerializeTo(FeData entity, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(Encoding.ASCII.GetBytes(entity.CarId));
        writer.WriteStruct(FeDataHeader.Empty with
        {
            IsBonus = entity.IsBonus ? (ushort)1 : (ushort)0,
            AvailableToAi = entity.AvailableToAi ? (ushort)1 : (ushort)0,
            CarClass = (ushort)entity.VehicleClass,
            IsPolice = entity.IsPolice ? (ushort)1 : (ushort)0,
            Seat = (ushort)entity.Seat,
            IsDlcCar = entity.IsDlcCar,
            SerialNumber = entity.SerialNumber,
            Accel = entity.CarAccel,
            TopSpeed = entity.CarTopSpeed,
            Handling = entity.CarHandling,
            Braking = entity.CarBraking,

            // TODO: Investigate what these values are:
            Unk_0x0c = entity.Unk_0x0c,
            Unk_0x14 = entity.Unk_0x14,
            Unk_0x16 = entity.Unk_0x16,
            Unk_0x1a = entity.Unk_0x1a,
            Unk_0x1c = entity.Unk_0x1c,
            Unk_0x1e = entity.Unk_0x1e,
            Unk_0x20 = entity.Unk_0x20,
            Unk_0x22 = entity.Unk_0x22,
            Unk_0x24 = entity.Unk_0x24,
            Unk_0x26 = entity.Unk_0x26,
            Unk_0x2c = entity.Unk_0x2c,
        });
        uint lastOffset = 0xcf;
        using var ms2 = new MemoryStream();
        using (var writer2 = new BinaryWriter(ms2))
        {
            foreach (var j in typeof(FeData).GetProperties())
            {
                if (j.GetAttribute<OffsetTableIndexAttribute>() is not { Value: int offset }) continue;                
                string value = j.GetValue(entity)?.ToString() ?? string.Empty;
                writer.Write(lastOffset);
                writer2.WriteNullTerminatedString(value, Encoding.Latin1);
                lastOffset += (uint)value.Length + 1;
            }
        }
        writer.Write(ms2.ToArray());
    }
}
