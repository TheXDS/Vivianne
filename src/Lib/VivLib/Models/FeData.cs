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
}
