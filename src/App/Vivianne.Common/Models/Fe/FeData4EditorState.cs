using System.Runtime.CompilerServices;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents the current state of the FeData4 editor ViewModel.
/// </summary>
public class FeData4EditorState : FileStateBase<FeData>, IFeData
{
    private MutableCompareTable? defaultCompare;
    private MutableCompareTable? _CompareUpg1;
    private MutableCompareTable? _CompareUpg2;
    private MutableCompareTable? _CompareUpg3;

    /// <summary>
    /// Gets a table of the colors defined in the FCE file.
    /// </summary>
    public FceColor?[]? PreviewFceColorTable { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a police car.
    /// </summary>
    public PursuitFlag PoliceFlag { get => File.PoliceFlag; set => Change(p => p.PoliceFlag, value); }

    /// <summary>
    /// Gets or sets the vehicle performance class onto which this vehicle belongs to.
    /// </summary>
    public CarClass VehicleClass { get => File.VehicleClass; set => Change(p => p.VehicleClass, value); }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle can be upgraded in
    /// carreer mode.
    /// </summary>
    public bool Upgradable { get => File.Upgradable; set => Change(p => p.Upgradable, value); }

    /// <summary>
    /// Gets or sets a value that indicates if this vehicle is a convertible.
    /// </summary>
    public RoofFlag Roof { get => File.Roof; set => Change(p => p.Roof, value); }

    /// <summary>
    /// Gets or sets a value that indicates the location of the engine in this
    /// car.
    /// </summary>
    public EngineLocation EngineLocation { get => File.EngineLocation; set => Change(p => p.EngineLocation, value); }

    /// <summary>
    /// Gets or sets a value that indicates if the car is part of a DLC (downloadable content) package.
    /// </summary>
    public bool IsDlc { get => File.IsDlc; set => Change(p => p.IsDlc, value); }

    /// <summary>
    /// Gets or sets the default compare table for this vehicle.
    /// </summary>
    public MutableCompareTable DefaultCompare
    {
        get => defaultCompare ??= CreateFrom(File.DefaultCompare);
        set
        {
            if (Change(ref defaultCompare, value))
            {
                File.DefaultCompare = value.ToCompare();
            }
        }
    }

    /// <summary>
    /// Gets or sets the first upgrade compare table for this vehicle.
    /// </summary>
    public MutableCompareTable CompareUpg1
    {
        get => _CompareUpg1 ??= CreateFrom(File.CompareUpg1);
        set
        {
            if (Change(ref _CompareUpg1, value))
            {
                File.CompareUpg1 = value.ToCompare();
            }
        }
    }

    /// <summary>
    /// Gets or sets the second upgrade compare table for this vehicle.
    /// </summary>
    public MutableCompareTable CompareUpg2
    {
        get => _CompareUpg2 ??= CreateFrom(File.CompareUpg2);
        set
        {
            if (Change(ref _CompareUpg2, value))
            {
                File.CompareUpg2 = value.ToCompare();
            }
        }
    }

    /// <summary>
    /// Gets or sets the third upgrade compare table for this vehicle.
    /// </summary>
    public MutableCompareTable CompareUpg3
    {
        get => _CompareUpg3 ??= CreateFrom(File.CompareUpg3);
        set
        {
            if (Change(ref _CompareUpg3, value))
            {
                File.CompareUpg3 = value.ToCompare();
            }
        }
    }

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

    /// <inheritdoc/>
    public string DynamicStability { get => File.DynamicStability; set => Change(p => p.DynamicStability, value); }

    private MutableCompareTable CreateFrom(CompareTable compare, [CallerMemberName] string propertyName = null!)
    {
        var result = MutableCompareTable.From(compare);
        result.Subscribe((i, p, n) => UnsavedChanges = true);
        return result;
    }
}
