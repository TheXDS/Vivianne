using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.ViewModels.Fe;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents the state of the <see cref="FeData3EditorViewModel"/>.
/// </summary>
public class FeData3EditorState : FileStateBase<FeData>, IFeData
{
    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public FceColor?[]? PreviewFceColorTable { get; set; }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x0c</c>.
    /// </summary>
    public ushort Unk_0x0c {get => File.Unk_0x0c; set => Change(p => p.Unk_0x0c, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x14</c>.
    /// </summary>
    public ushort Unk_0x14 {get => File.Unk_0x14; set => Change(p => p.Unk_0x14, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x16</c>.
    /// </summary>
    public ushort Unk_0x16 {get => File.Unk_0x16; set => Change(p => p.Unk_0x16, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1a</c>.
    /// </summary>
    public ushort Unk_0x1a {get => File.Unk_0x1a; set => Change(p => p.Unk_0x1a, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1c</c>.
    /// </summary>
    public ushort Unk_0x1c {get => File.Unk_0x1c; set => Change(p => p.Unk_0x1c, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x1e</c>.
    /// </summary>
    public ushort Unk_0x1e {get => File.Unk_0x1e; set => Change(p => p.Unk_0x1e, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x20</c>.
    /// </summary>
    public ushort Unk_0x20 {get => File.Unk_0x20; set => Change(p => p.Unk_0x20, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x22</c>.
    /// </summary>
    public ushort Unk_0x22 {get => File.Unk_0x22; set => Change(p => p.Unk_0x22, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x24</c>.
    /// </summary>
    public ushort Unk_0x24 {get => File.Unk_0x24; set => Change(p => p.Unk_0x24, value); }

    /// <summary>
    /// Gets or sets the unknown <c>WORD</c> value at offset <c>0x26</c>.
    /// </summary>
    public ushort Unk_0x26 {get => File.Unk_0x26; set => Change(p => p.Unk_0x26, value); }

    /// <summary>
    /// Gets or sets the unknown <c>BYTE</c> value at offset <c>0x2c</c>.
    /// </summary>
    public byte Unk_0x2c {get => File.Unk_0x2c; set => Change(p => p.Unk_0x2c, value); }

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public CarClass VehicleClass { get => File.VehicleClass; set => Change(p => p.VehicleClass, value); }

    /// <summary>
    /// Gets or sets the driver seat position for this vehicle.
    /// </summary>
    public DriverSeatPosition Seat { get => File.Seat; set => Change(p => p.Seat, value); }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public bool IsPolice { get => File.IsPolice; set => Change(p => p.IsPolice, value); }

    /// <summary>
    /// Gets or sets a value that indicates if the car is part of a DLC (downloadable content) package.
    /// </summary>
    public ushort IsDlc { get => File.IsDlc; set => Change(p => p.IsDlc, value); }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is available to AI drivers.
    /// </summary>
    public bool AvailableToAi { get => File.AvailableToAi; set => Change(p => p.AvailableToAi, value); }

    /// <summary>
    /// Gets or sets the acceleration compare value for this vehicle.
    /// </summary>
    public byte CarAccel { get => File.CarAccel; set => Change(p => p.CarAccel, value); }

    /// <summary>
    /// Gets or sets the top speed compare value for this vehicle.
    /// </summary>
    public byte CarTopSpeed { get => File.CarTopSpeed; set => Change(p => p.CarTopSpeed, value); }

    /// <summary>
    /// Gets or sets the car handling compare value for this vehicle.
    /// </summary>
    public byte CarHandling { get => File.CarHandling; set => Change(p => p.CarHandling, value); }

    /// <summary>
    /// Gets or sets the braking compare value for this vehicle.
    /// </summary>
    public byte CarBraking { get => File.CarBraking; set => Change(p => p.CarBraking, value); }

    /// <inheritdoc/>
    public string TopSpeed { get => File.TopSpeed; set => Change(p => p.TopSpeed, value); }

    /// <inheritdoc/>
    public string Accel0To60 { get => File.Accel0To60; set => Change(p => p.Accel0To60, value); }

    /// <inheritdoc/>
    public string Accel0To100 { get => File.Accel0To100; set => Change(p => p.Accel0To100, value); }

    /// <inheritdoc/>
    public string Transmission { get => File.Transmission; set => Change(p => p.Transmission, value); }

    /// <inheritdoc/>
    public string Gearbox { get => File.Gearbox; set => Change(p => p.Gearbox, value); }

    /// <inheritdoc/>
    public string History1 { get => File.History1; set => Change(p => p.History1, value); }

    /// <inheritdoc/>
    public string History2 { get => File.History2; set => Change(p => p.History2, value); }

    /// <inheritdoc/>
    public string History3 { get => File.History3; set => Change(p => p.History3, value); }

    /// <inheritdoc/>
    public string History4 { get => File.History4; set => Change(p => p.History4, value); }

    /// <inheritdoc/>
    public string History5 { get => File.History5; set => Change(p => p.History5, value); }

    /// <inheritdoc/>
    public string History6 { get => File.History6; set => Change(p => p.History6, value); }

    /// <inheritdoc/>
    public string History7 { get => File.History7; set => Change(p => p.History7, value); }

    /// <inheritdoc/>
    public string History8 { get => File.History8; set => Change(p => p.History8, value); }

    /// <inheritdoc/>
    public string Color1 { get => File.Color1; set => Change(p => p.Color1, value); }

    /// <inheritdoc/>
    public string Color2 { get => File.Color2; set => Change(p => p.Color2, value); }

    /// <inheritdoc/>
    public string Color3 { get => File.Color3; set => Change(p => p.Color3, value); }

    /// <inheritdoc/>
    public string Color4 { get => File.Color4; set => Change(p => p.Color4, value); }

    /// <inheritdoc/>
    public string Color5 { get => File.Color5; set => Change(p => p.Color5, value); }

    /// <inheritdoc/>
    public string Color6 { get => File.Color6; set => Change(p => p.Color6, value); }

    /// <inheritdoc/>
    public string Color7 { get => File.Color7; set => Change(p => p.Color7, value); }

    /// <inheritdoc/>
    public string Color8 { get => File.Color8; set => Change(p => p.Color8, value); }

    /// <inheritdoc/>
    public string Color9 { get => File.Color9; set => Change(p => p.Color9, value); }

    /// <inheritdoc/>
    public string Color10 { get => File.Color10; set => Change(p => p.Color10, value); }

    /// <inheritdoc/>
    public string CarId { get => File.CarId; set => Change(p => p.CarId, value); }

    /// <inheritdoc/>
    public ushort SerialNumber { get => File.SerialNumber; set => Change(p => p.SerialNumber, value); }

    /// <inheritdoc/>
    public bool IsBonus { get => File.IsBonus; set => Change(p => p.IsBonus, value); }

    /// <inheritdoc/>
    public ushort StringEntries { get => File.StringEntries; set => Change(p => p.StringEntries, value); }

    /// <inheritdoc/>
    public string Manufacturer { get => File.Manufacturer; set => Change(p => p.Manufacturer, value); }

    /// <inheritdoc/>
    public string Model { get => File.Model; set => Change(p => p.Model, value); }

    /// <inheritdoc/>
    public string CarName { get => File.CarName; set => Change(p => p.CarName, value); }

    /// <inheritdoc/>
    public string Price { get => File.Price; set => Change(p => p.Price, value); }

    /// <inheritdoc/>
    public string Status { get => File.Status; set => Change(p => p.Status, value); }

    /// <inheritdoc/>
    public string Weight { get => File.Weight; set => Change(p => p.Weight, value); }

    /// <inheritdoc/>
    public string WeightDistribution { get => File.WeightDistribution; set => Change(p => p.WeightDistribution, value); }

    /// <inheritdoc/>
    public string Length { get => File.Length; set => Change(p => p.Length, value); }

    /// <inheritdoc/>
    public string Width { get => File.Width; set => Change(p => p.Width, value); }

    /// <inheritdoc/>
    public string Height { get => File.Height; set => Change(p => p.Height, value); }

    /// <inheritdoc/>
    public string Engine { get => File.Engine; set => Change(p => p.Engine, value); }

    /// <inheritdoc/>
    public string Displacement { get => File.Displacement; set => Change(p => p.Displacement, value); }

    /// <inheritdoc/>
    public string Hp { get => File.Hp; set => Change(p => p.Hp, value); }

    /// <inheritdoc/>
    public string Torque { get => File.Torque; set => Change(p => p.Torque, value); }

    /// <inheritdoc/>
    public string MaxEngineSpeed { get => File.MaxEngineSpeed; set => Change(p => p.MaxEngineSpeed, value); }

    /// <inheritdoc/>
    public string Brakes { get => File.Brakes; set => Change(p => p.Brakes, value); }

    /// <inheritdoc/>
    public string Tires { get => File.Tires; set => Change(p => p.Tires, value); }
}
