using System.Text;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;

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
        IsDlc = feDataHeader.IsDlcCar,
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
        IsDlcCar = feData.IsDlc,
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
}
