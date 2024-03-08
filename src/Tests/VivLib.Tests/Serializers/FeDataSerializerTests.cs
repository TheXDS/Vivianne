#pragma warning disable CS1591
#pragma warning disable CA1859

using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Serializers;

public class FeDataSerializerTests
{
    private static MemoryStream CreateTestFeDataStream()
    {
        return new([
            // Car attributes
            .."TEST"u8.ToArray(),   // Car ID
            0x09, 0x00,             // Flag count
            0x01, 0x00,             // Is Bonus
            0x01, 0x00,             // Available to AI
            0x02, 0x00,             // "C" Car Class
            0x03, 0x00, 0x00, 0x00, // ---- Unknown data ----
            0x01, 0x00,             // Is Police
            0x01, 0x00,             // "Right" seat location
            0x00, 0x00, 0x80, 0x00, // ---- Unknown data ----
            0xcd, 0xab,             // 0xABCD serial number
            0x00, 0x00, 0x00, 0x00, // \
            0x00, 0x00, 0x00, 0x00, //  \ ---- Unknown data ----
            0x00, 0x00, 0x00, 0x00, //  /
            0x00, 0x00,             // /
            0x14, 0x13, 0x12, 0x11, // "20, 19, 18, 17" Car compare table
            0x05, 0x28, 0x00,       // ---- Unknown data ----

            // String offset table
            0xCF, 0x00, 0x00, 0x00, // Manufacturer
            0xDC, 0x00, 0x00, 0x00, // Model
            0xE2, 0x00, 0x00, 0x00, // CarName
            0xEA, 0x00, 0x00, 0x00, // Price
            0xF0, 0x00, 0x00, 0x00, // Status
            0xF7, 0x00, 0x00, 0x00, // Weight
            0xFE, 0x00, 0x00, 0x00, // WeightDistribution
            0x11, 0x01, 0x00, 0x00, // Length
            0x18, 0x01, 0x00, 0x00, // Width
            0x1E, 0x01, 0x00, 0x00, // Height
            0x25, 0x01, 0x00, 0x00, // Engine
            0x2C, 0x01, 0x00, 0x00, // Displacement
            0x39, 0x01, 0x00, 0x00, // Hp
            0x3C, 0x01, 0x00, 0x00, // Torque
            0x43, 0x01, 0x00, 0x00, // MaxEngineSpeed
            0x52, 0x01, 0x00, 0x00, // Brakes
            0x59, 0x01, 0x00, 0x00, // Tires
            0x5F, 0x01, 0x00, 0x00, // TopSpeed
            0x68, 0x01, 0x00, 0x00, // Accel0To60
            0x73, 0x01, 0x00, 0x00, // Accel0To100
            0x7f, 0x01, 0x00, 0x00, // Transmission
            0x8c, 0x01, 0x00, 0x00, // Gearbox
            0x94, 0x01, 0x00, 0x00, // History1
            0x9d, 0x01, 0x00, 0x00, // History2
            0xA6, 0x01, 0x00, 0x00, // History3
            0xaf, 0x01, 0x00, 0x00, // History4
            0xb8, 0x01, 0x00, 0x00, // History5
            0xc1, 0x01, 0x00, 0x00, // History6
            0xca, 0x01, 0x00, 0x00, // History7
            0xd3, 0x01, 0x00, 0x00, // History8
            0xdc, 0x01, 0x00, 0x00, // Color1
            0xe3, 0x01, 0x00, 0x00, // Color2
            0xea, 0x01, 0x00, 0x00, // Color3
            0xf1, 0x01, 0x00, 0x00, // Color4
            0xf8, 0x01, 0x00, 0x00, // Color5
            0xff, 0x01, 0x00, 0x00, // Color6
            0x06, 0x02, 0x00, 0x00, // Color7
            0x0d, 0x02, 0x00, 0x00, // Color8
            0x14, 0x02, 0x00, 0x00, // Color9
            0x1b, 0x02, 0x00, 0x00, // Color 10
            .."Manufacturer"u8.ToArray(), 0x00,
            .."Model"u8.ToArray(), 0x00,
            .."CarName"u8.ToArray(), 0x00,
            .."Price"u8.ToArray(), 0x00,
            .."Status"u8.ToArray(), 0x00,
            .."Weight"u8.ToArray(), 0x00,
            .."WeightDistribution"u8.ToArray(), 0x00,
            .."Length"u8.ToArray(), 0x00,
            .."Width"u8.ToArray(), 0x00,
            .."Height"u8.ToArray(), 0x00,
            .."Engine"u8.ToArray(), 0x00,
            .."Displacement"u8.ToArray(), 0x00,
            .."Hp"u8.ToArray(), 0x00,
            .."Torque"u8.ToArray(), 0x00,
            .."MaxEngineSpeed"u8.ToArray(), 0x00,
            .."Brakes"u8.ToArray(), 0x00,
            .."Tires"u8.ToArray(), 0x00,
            .."TopSpeed"u8.ToArray(), 0x00,
            .."Accel0To60"u8.ToArray(), 0x00,
            .."Accel0To100"u8.ToArray(), 0x00,
            .."Transmission"u8.ToArray(), 0x00,
            .."Gearbox"u8.ToArray(), 0x00,
            .."History1"u8.ToArray(), 0x00,
            .."History2"u8.ToArray(), 0x00,
            .."History3"u8.ToArray(), 0x00,
            .."History4"u8.ToArray(), 0x00,
            .."History5"u8.ToArray(), 0x00,
            .."History6"u8.ToArray(), 0x00,
            .."History7"u8.ToArray(), 0x00,
            .."History8"u8.ToArray(), 0x00,
            .."Color1"u8.ToArray(), 0x00,
            .."Color2"u8.ToArray(), 0x00,
            .."Color3"u8.ToArray(), 0x00,
            .."Color4"u8.ToArray(), 0x00,
            .."Color5"u8.ToArray(), 0x00,
            .."Color6"u8.ToArray(), 0x00,
            .."Color7"u8.ToArray(), 0x00,
            .."Color8"u8.ToArray(), 0x00,
            .."Color9"u8.ToArray(), 0x00,
            .."Color10"u8.ToArray(), 0x00]);
    }

    private static FeData CreateTestFeData()
    {
        var feData = new FeData
        {
            CarId = "TEST",
            SerialNumber = 0xabcd,
            VehicleClass = Nfs3CarClass.C,
            Seat = DriverSeatPosition.Right,
            IsPolice = true,
            IsBonus = true,
            AvailableToAi = true,
            CarAccel = 20,
            CarTopSpeed = 19,
            CarHandling = 18,
            CarBraking = 17,
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
        return feData;
    }

    [Test]
    public void Serializer_can_read_FeData()
    {
        FeData expected = CreateTestFeData();
        using var ms = CreateTestFeDataStream();
        ISerializer<FeData> serializer = new FeDataSerializer();

        FeData feData = serializer.Deserialize(ms);

        Assert.That(feData, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(feData.CarId, Is.EqualTo(expected.CarId));
            Assert.That(feData.SerialNumber, Is.EqualTo(expected.SerialNumber));
            Assert.That(feData.VehicleClass, Is.EqualTo(expected.VehicleClass));
            Assert.That(feData.Seat, Is.EqualTo(expected.Seat));
            Assert.That(feData.IsPolice, Is.EqualTo(expected.IsPolice));
            Assert.That(feData.IsBonus, Is.EqualTo(expected.IsBonus));
            Assert.That(feData.AvailableToAi, Is.EqualTo(expected.AvailableToAi));
            Assert.That(feData.CarAccel, Is.EqualTo(expected.CarAccel));
            Assert.That(feData.CarTopSpeed, Is.EqualTo(expected.CarTopSpeed));
            Assert.That(feData.CarHandling, Is.EqualTo(expected.CarHandling));
            Assert.That(feData.CarBraking, Is.EqualTo(expected.CarBraking));
            Assert.That(feData.Manufacturer, Is.EqualTo(expected.Manufacturer));
            Assert.That(feData.Model, Is.EqualTo(expected.Model));
            Assert.That(feData.CarName, Is.EqualTo(expected.CarName));
            Assert.That(feData.Price, Is.EqualTo(expected.Price));
            Assert.That(feData.Status, Is.EqualTo(expected.Status));
            Assert.That(feData.Weight, Is.EqualTo(expected.Weight));
            Assert.That(feData.WeightDistribution, Is.EqualTo(expected.WeightDistribution));
            Assert.That(feData.Length, Is.EqualTo(expected.Length));
            Assert.That(feData.Width, Is.EqualTo(expected.Width));
            Assert.That(feData.Height, Is.EqualTo(expected.Height));
            Assert.That(feData.Engine, Is.EqualTo(expected.Engine));
            Assert.That(feData.Displacement, Is.EqualTo(expected.Displacement));
            Assert.That(feData.Hp, Is.EqualTo(expected.Hp));
            Assert.That(feData.Torque, Is.EqualTo(expected.Torque));
            Assert.That(feData.MaxEngineSpeed, Is.EqualTo(expected.MaxEngineSpeed));
            Assert.That(feData.Brakes, Is.EqualTo(expected.Brakes));
            Assert.That(feData.Tires, Is.EqualTo(expected.Tires));
            Assert.That(feData.TopSpeed, Is.EqualTo(expected.TopSpeed));
            Assert.That(feData.Accel0To60, Is.EqualTo(expected.Accel0To60));
            Assert.That(feData.Accel0To100, Is.EqualTo(expected.Accel0To100));
            Assert.That(feData.Transmission, Is.EqualTo(expected.Transmission));
            Assert.That(feData.Gearbox, Is.EqualTo(expected.Gearbox));
            Assert.That(feData.History1, Is.EqualTo(expected.History1));
            Assert.That(feData.History2, Is.EqualTo(expected.History2));
            Assert.That(feData.History3, Is.EqualTo(expected.History3));
            Assert.That(feData.History4, Is.EqualTo(expected.History4));
            Assert.That(feData.History5, Is.EqualTo(expected.History5));
            Assert.That(feData.History6, Is.EqualTo(expected.History6));
            Assert.That(feData.History7, Is.EqualTo(expected.History7));
            Assert.That(feData.History8, Is.EqualTo(expected.History8));
            Assert.That(feData.Color1, Is.EqualTo(expected.Color1));
            Assert.That(feData.Color2, Is.EqualTo(expected.Color2));
            Assert.That(feData.Color3, Is.EqualTo(expected.Color3));
            Assert.That(feData.Color4, Is.EqualTo(expected.Color4));
            Assert.That(feData.Color5, Is.EqualTo(expected.Color5));
            Assert.That(feData.Color6, Is.EqualTo(expected.Color6));
            Assert.That(feData.Color7, Is.EqualTo(expected.Color7));
            Assert.That(feData.Color8, Is.EqualTo(expected.Color8));
            Assert.That(feData.Color9, Is.EqualTo(expected.Color9));
            Assert.That(feData.Color10, Is.EqualTo(expected.Color10));
        });
    }

    [Test]
    public void Serializer_can_write_FeData()
    {
        var expected = CreateTestFeDataStream().ToArray();
        using var ms = new MemoryStream();
        ISerializer<FeData> s = new FeDataSerializer();

        s.SerializeTo(CreateTestFeData(), ms);

        Assert.That(ms.ToArray(), Is.EquivalentTo(expected));
    }
}
