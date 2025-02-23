#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Serializers.Fe.Nfs3;

namespace TheXDS.Vivianne.Serializers.Nfs3;

[TestFixture]
public class FeDataSerializerTests() : SerializerTestsBase<FeDataSerializer, FeData>("Nfs3.test.eng", GetDefaultFile())
{
    private static FeData GetDefaultFile() => new()
    {
        CarId = "TEST",
        SerialNumber = 0xabcd,
        VehicleClass = CarClass.C,
        Unk_0x0c = 0x03,
        Seat = DriverSeatPosition.Right,
        Unk_0x16 = 128,
        IsPolice = true,
        IsBonus = true,
        AvailableToAi = true,
        CarAccel = 20,
        CarTopSpeed = 19,
        CarHandling = 18,
        CarBraking = 17,
        Unk_0x2c = 5,
        StringEntries = 40,
        Manufacturer = nameof(FeData.Manufacturer),
        Model = nameof(FeData.Model),
        CarName = nameof(FeData.CarName),
        Price = nameof(FeData.Price),
        Status = nameof(FeData.Status),
        Weight = nameof(FeData.Weight),
        WeightDistribution = nameof(FeData.WeightDistribution),
        Length = nameof(FeData.Length),
        Width = nameof(FeData.Width),
        Height = nameof(FeData.Height),
        Engine = nameof(FeData.Engine),
        Displacement = nameof(FeData.Displacement),
        Hp = nameof(FeData.Hp),
        Torque = nameof(FeData.Torque),
        MaxEngineSpeed = nameof(FeData.MaxEngineSpeed),
        Brakes = nameof(FeData.Brakes),
        Tires = nameof(FeData.Tires),
        TopSpeed = nameof(FeData.TopSpeed),
        Accel0To60 = nameof(FeData.Accel0To60),
        Accel0To100 = nameof(FeData.Accel0To100),
        Transmission = nameof(FeData.Transmission),
        Gearbox = nameof(FeData.Gearbox),
        History1 = nameof(FeData.History1),
        History2 = nameof(FeData.History2),
        History3 = nameof(FeData.History3),
        History4 = nameof(FeData.History4),
        History5 = nameof(FeData.History5),
        History6 = nameof(FeData.History6),
        History7 = nameof(FeData.History7),
        History8 = nameof(FeData.History8),
        Color1 = nameof(FeData.Color1),
        Color2 = nameof(FeData.Color2),
        Color3 = nameof(FeData.Color3),
        Color4 = nameof(FeData.Color4),
        Color5 = nameof(FeData.Color5),
        Color6 = nameof(FeData.Color6),
        Color7 = nameof(FeData.Color7),
        Color8 = nameof(FeData.Color8),
        Color9 = nameof(FeData.Color9),
        Color10 = nameof(FeData.Color10)
    };

    protected override void TestParsedFile(FeData expected, FeData actual)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.CarId, Is.EqualTo(expected.CarId));
            Assert.That(actual.SerialNumber, Is.EqualTo(expected.SerialNumber));
            Assert.That(actual.VehicleClass, Is.EqualTo(expected.VehicleClass));
            Assert.That(actual.Seat, Is.EqualTo(expected.Seat));
            Assert.That(actual.IsPolice, Is.EqualTo(expected.IsPolice));
            Assert.That(actual.IsBonus, Is.EqualTo(expected.IsBonus));
            Assert.That(actual.AvailableToAi, Is.EqualTo(expected.AvailableToAi));
            Assert.That(actual.CarAccel, Is.EqualTo(expected.CarAccel));
            Assert.That(actual.CarTopSpeed, Is.EqualTo(expected.CarTopSpeed));
            Assert.That(actual.CarHandling, Is.EqualTo(expected.CarHandling));
            Assert.That(actual.CarBraking, Is.EqualTo(expected.CarBraking));
            Assert.That(actual.Manufacturer, Is.EqualTo(expected.Manufacturer));
            Assert.That(actual.Model, Is.EqualTo(expected.Model));
            Assert.That(actual.CarName, Is.EqualTo(expected.CarName));
            Assert.That(actual.Price, Is.EqualTo(expected.Price));
            Assert.That(actual.Status, Is.EqualTo(expected.Status));
            Assert.That(actual.Weight, Is.EqualTo(expected.Weight));
            Assert.That(actual.WeightDistribution, Is.EqualTo(expected.WeightDistribution));
            Assert.That(actual.Length, Is.EqualTo(expected.Length));
            Assert.That(actual.Width, Is.EqualTo(expected.Width));
            Assert.That(actual.Height, Is.EqualTo(expected.Height));
            Assert.That(actual.Engine, Is.EqualTo(expected.Engine));
            Assert.That(actual.Displacement, Is.EqualTo(expected.Displacement));
            Assert.That(actual.Hp, Is.EqualTo(expected.Hp));
            Assert.That(actual.Torque, Is.EqualTo(expected.Torque));
            Assert.That(actual.MaxEngineSpeed, Is.EqualTo(expected.MaxEngineSpeed));
            Assert.That(actual.Brakes, Is.EqualTo(expected.Brakes));
            Assert.That(actual.Tires, Is.EqualTo(expected.Tires));
            Assert.That(actual.TopSpeed, Is.EqualTo(expected.TopSpeed));
            Assert.That(actual.Accel0To60, Is.EqualTo(expected.Accel0To60));
            Assert.That(actual.Accel0To100, Is.EqualTo(expected.Accel0To100));
            Assert.That(actual.Transmission, Is.EqualTo(expected.Transmission));
            Assert.That(actual.Gearbox, Is.EqualTo(expected.Gearbox));
            Assert.That(actual.History1, Is.EqualTo(expected.History1));
            Assert.That(actual.History2, Is.EqualTo(expected.History2));
            Assert.That(actual.History3, Is.EqualTo(expected.History3));
            Assert.That(actual.History4, Is.EqualTo(expected.History4));
            Assert.That(actual.History5, Is.EqualTo(expected.History5));
            Assert.That(actual.History6, Is.EqualTo(expected.History6));
            Assert.That(actual.History7, Is.EqualTo(expected.History7));
            Assert.That(actual.History8, Is.EqualTo(expected.History8));
            Assert.That(actual.Color1, Is.EqualTo(expected.Color1));
            Assert.That(actual.Color2, Is.EqualTo(expected.Color2));
            Assert.That(actual.Color3, Is.EqualTo(expected.Color3));
            Assert.That(actual.Color4, Is.EqualTo(expected.Color4));
            Assert.That(actual.Color5, Is.EqualTo(expected.Color5));
            Assert.That(actual.Color6, Is.EqualTo(expected.Color6));
            Assert.That(actual.Color7, Is.EqualTo(expected.Color7));
            Assert.That(actual.Color8, Is.EqualTo(expected.Color8));
            Assert.That(actual.Color9, Is.EqualTo(expected.Color9));
            Assert.That(actual.Color10, Is.EqualTo(expected.Color10));
        });
    }
}
