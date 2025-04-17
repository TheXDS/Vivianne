using System.Text;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs4;

public partial class FeDataSerializer
{
    private static FeData CreateFeData(FeDataHeader fedataHeader) => new()
    {
        CarId = Encoding.Latin1.GetString(fedataHeader.CarId),
        SerialNumber = fedataHeader.SerialNumber,
        PoliceFlag = (PursuitFlag)(fedataHeader.PoliceFlag_IsBonus & Nfs4PursuitFlag_Mask),
        VehicleClass = fedataHeader.CarClass,
        IsBonus = (fedataHeader.PoliceFlag_IsBonus & IsBonus_Mask) != 0,
        Upgradable = (fedataHeader.Upgradable_RoofFlags_IsDlc & Upgradable_Mask) == 0,
        IsDlc = (fedataHeader.Upgradable_RoofFlags_IsDlc & IsDlc_Mask) != 0,
        Roof = (RoofFlag)(fedataHeader.Upgradable_RoofFlags_IsDlc & Roof_Mask),
        StringEntries = fedataHeader.StringEntries,
        DefaultCompare = new CompareTable()
        {
            Acceleration = fedataHeader.Acceleration.DefaultValue,
            TopSpeed = fedataHeader.TopSpeed.DefaultValue,
            Handling = fedataHeader.Handling.DefaultValue,
            Braking = fedataHeader.Braking.DefaultValue,
            Overall = fedataHeader.Overall.DefaultValue,
            Price = fedataHeader.BasePrice,
        },
        CompareUpg1 = new CompareTable()
        {
            Acceleration = fedataHeader.Acceleration.Upgrade1,
            TopSpeed = fedataHeader.TopSpeed.Upgrade1,
            Handling = fedataHeader.Handling.Upgrade1,
            Braking = fedataHeader.Braking.Upgrade1,
            Overall = fedataHeader.Overall.Upgrade1,
            Price = fedataHeader.Upgrade1Price,
        },
        CompareUpg2 = new CompareTable()
        {
            Acceleration = fedataHeader.Acceleration.Upgrade2,
            TopSpeed = fedataHeader.TopSpeed.Upgrade2,
            Handling = fedataHeader.Handling.Upgrade2,
            Braking = fedataHeader.Braking.Upgrade2,
            Overall = fedataHeader.Overall.Upgrade2,
            Price = fedataHeader.Upgrade2Price,
        },
        CompareUpg3 = new CompareTable()
        {
            Acceleration = fedataHeader.Acceleration.Upgrade3,
            TopSpeed = fedataHeader.TopSpeed.Upgrade3,
            Handling = fedataHeader.Handling.Upgrade3,
            Braking = fedataHeader.Braking.Upgrade3,
            Overall = fedataHeader.Overall.Upgrade3,
            Price = fedataHeader.Upgrade3Price,
        },
        EngineLocation = fedataHeader.EngineLocation,
    };

    private static FeDataHeader CreateHeader(FeData feData) => FeDataHeader.Empty with
    {
        CarId = Encoding.Latin1.GetBytes(feData.CarId),
        SerialNumber = feData.SerialNumber,
        PoliceFlag_IsBonus = (byte)(((byte)feData.PoliceFlag) | (byte)(feData.IsBonus ? 0x1 : 0x0)),
        CarClass = feData.VehicleClass,
        Upgradable_RoofFlags_IsDlc = (byte)((feData.Upgradable ? 0x0 : Upgradable_Mask) | (byte)(feData.Roof) | (feData.IsDlc ? IsDlc_Mask : 0x0)),
        StringEntries = feData.StringEntries,
        CompareCount = 0x0a,
        Acceleration = new CompareTableItem()
        {
            DefaultValue = feData.DefaultCompare.Acceleration,
            Upgrade1 = feData.CompareUpg1.Acceleration,
            Upgrade2 = feData.CompareUpg2.Acceleration,
            Upgrade3 = feData.CompareUpg3.Acceleration,
        },
        TopSpeed = new CompareTableItem()
        {
            DefaultValue = feData.DefaultCompare.TopSpeed,
            Upgrade1 = feData.CompareUpg1.TopSpeed,
            Upgrade2 = feData.CompareUpg2.TopSpeed,
            Upgrade3 = feData.CompareUpg3.TopSpeed,
        },
        Handling = new CompareTableItem()
        {
            DefaultValue = feData.DefaultCompare.Handling,
            Upgrade1 = feData.CompareUpg1.Handling,
            Upgrade2 = feData.CompareUpg2.Handling,
            Upgrade3 = feData.CompareUpg3.Handling,
        },
        Braking = new CompareTableItem()
        {
            DefaultValue = feData.DefaultCompare.Braking,
            Upgrade1 = feData.CompareUpg1.Braking,
            Upgrade2 = feData.CompareUpg2.Braking,
            Upgrade3 = feData.CompareUpg3.Braking,
        },
        Overall = new CompareTableItem()
        {
            DefaultValue = feData.DefaultCompare.Overall,
            Upgrade1 = feData.CompareUpg1.Overall,
            Upgrade2 = feData.CompareUpg2.Overall,
            Upgrade3 = feData.CompareUpg3.Overall,
        },
        BasePrice = feData.DefaultCompare.Price,
        Upgrade1Price = feData.CompareUpg1.Price,
        Upgrade2Price = feData.CompareUpg2.Price,
        Upgrade3Price = feData.CompareUpg3.Price,
        EngineLocation = feData.EngineLocation,
    };
}
