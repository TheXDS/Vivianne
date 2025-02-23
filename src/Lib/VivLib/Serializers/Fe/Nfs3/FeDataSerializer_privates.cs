using System.Diagnostics;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs3;

public partial class FeDataSerializer
{
    private static FeData CreateFeData(FeDataHeader feDataHeader) => new()
    {
        CarId = Encoding.Latin1.GetString(feDataHeader.CarId),
        IsBonus = feDataHeader.IsBonus != 0,
        AvailableToAi = feDataHeader.AvailableToAi != 0,
        VehicleClass = (CarClass)feDataHeader.CarClass,
        IsPolice = feDataHeader.IsPolice != 0,
        Seat = (DriverSeatPosition)feDataHeader.Seat,
        IsDlcCar = feDataHeader.IsDlcCar,
        SerialNumber = feDataHeader.SerialNumber,
        CarAccel = feDataHeader.Accel,
        CarTopSpeed = feDataHeader.TopSpeed,
        CarHandling = feDataHeader.Handling,
        CarBraking = feDataHeader.Braking,
        StringEntries = feDataHeader.StringEntries,

        // TODO: Investigate what these values are:
        Unk_0x0c = feDataHeader.Unk_0x0c,
        Unk_0x14 = feDataHeader.Unk_0x14,
        Unk_0x16 = feDataHeader.Unk_0x16,
        Unk_0x1a = feDataHeader.Unk_0x1a,
        Unk_0x1c = feDataHeader.Unk_0x1c,
        Unk_0x1e = feDataHeader.Unk_0x1e,
        Unk_0x20 = feDataHeader.Unk_0x20,
        Unk_0x22 = feDataHeader.Unk_0x22,
        Unk_0x24 = feDataHeader.Unk_0x24,
        Unk_0x26 = feDataHeader.Unk_0x26,
        Unk_0x2c = feDataHeader.Unk_0x2c,
    };

    private static FeDataHeader CreateHeader(FeData feData) => FeDataHeader.Empty with
    {
        CarId = Encoding.Latin1.GetBytes(feData.CarId),
        IsBonus = feData.IsBonus ? (ushort)1 : (ushort)0,
        AvailableToAi = feData.AvailableToAi ? (ushort)1 : (ushort)0,
        CarClass = (ushort)feData.VehicleClass,
        IsPolice = feData.IsPolice ? (ushort)1 : (ushort)0,
        Seat = (ushort)feData.Seat,
        IsDlcCar = feData.IsDlcCar,
        SerialNumber = feData.SerialNumber,
        Accel = feData.CarAccel,
        TopSpeed = feData.CarTopSpeed,
        Handling = feData.CarHandling,
        Braking = feData.CarBraking,
        StringEntries = feData.StringEntries,

        // TODO: Investigate what these values are:
        Unk_0x0c = feData.Unk_0x0c,
        Unk_0x14 = feData.Unk_0x14,
        Unk_0x16 = feData.Unk_0x16,
        Unk_0x1a = feData.Unk_0x1a,
        Unk_0x1c = feData.Unk_0x1c,
        Unk_0x1e = feData.Unk_0x1e,
        Unk_0x20 = feData.Unk_0x20,
        Unk_0x22 = feData.Unk_0x22,
        Unk_0x24 = feData.Unk_0x24,
        Unk_0x26 = feData.Unk_0x26,
        Unk_0x2c = feData.Unk_0x2c,
    };

    private static string SeekAndRead(BinaryReader reader, uint offset)
    {
        if (offset > reader.BaseStream.Length)
        {
            Debug.Print(string.Format(St.FeDataSerializer_StringOutOfBounds, offset));
            return string.Empty;
        }
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        return reader.ReadNullTerminatedString(Encoding.Latin1);
    }

    private static uint[] GetOffsetTable(BinaryReader reader, ushort entries)
    {
        uint[] offsets = new uint[entries];
        for (int i = 0; i < entries; i++)
        {
            offsets[i] = reader.ReadUInt32();
        }
        return offsets;
    }

    private static void ReadStrings(BinaryReader reader, FeData data, uint[] offsets)
    {
        foreach (var j in typeof(FeData).GetProperties())
        {
            if (j.GetAttribute<OffsetTableIndexAttribute>() is { Value: int offset })
            {
                j.SetValue(data, SeekAndRead(reader, offsets[offset]));
            }
        }
    }

    private static byte[] WriteStrings(BinaryWriter offsetsWriter, FeData feData)
    {
        uint lastOffset = (uint)(0x2f + feData.StringEntries * 4);
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms))
        {
            foreach (var j in typeof(FeData).GetProperties())
            {
                if (j.GetAttribute<OffsetTableIndexAttribute>() is not { Value: int offset }) continue;
                string value = j.GetValue(feData)?.ToString() ?? string.Empty;
                offsetsWriter.Write(lastOffset);
                bw.WriteNullTerminatedString(value, Encoding.Latin1);
                lastOffset += (uint)value.Length + 1;
            }

        }
        return ms.ToArray();
    }
}
