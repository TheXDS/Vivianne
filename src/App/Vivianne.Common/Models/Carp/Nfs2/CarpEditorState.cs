using System.Collections.Generic;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Carp.Nfs2;

/// <summary>
/// Implements a <see cref="FileStateBase{T}"/> for NFS2 Carp files.
/// </summary>
public class CarpEditorState : FileStateBase<CarPerf>
{
    private ObservableListWrap<byte>? _brakeDecreaseCurve;
    private ObservableListWrap<byte>? _brakeIncreaseCurve;
    private ObservableListWrap<byte>? _gasDecreaseCurve;
    private ObservableListWrap<byte>? _gasIncreaseCurve;
    private ObservableListWrap<double>? _gearEfficiency;
    private ObservableListWrap<double>? _torqueCurve;
    private ObservableListWrap<double>? _velocityToRpm;

    /// <inheritdoc/>
    public double AeroDownMult
    {
        get => File.AeroDownMult;
        set => Change(p => p.AeroDownMult, value);
    }

    /// <inheritdoc/>
    public IList<byte> BrakeDecreaseCurve => _brakeDecreaseCurve ??= GetObservable(File.BrakeDecreaseCurve);

    /// <inheritdoc/>
    public IList<byte> BrakeIncreaseCurve => _brakeIncreaseCurve ??= GetObservable(File.BrakeIncreaseCurve);


    /// <inheritdoc/>
    public int EngineMaxRpm
    {
        get => File.EngineMaxRpm;
        set => Change(p => p.EngineMaxRpm, value);
    }

    /// <inheritdoc/>
    public double FrontBrakeBias
    {
        get => File.FrontBrakeBias;
        set => Change(p => p.FrontBrakeBias, value);
    }

    /// <inheritdoc/>
    public double FrontDriveRatio
    {
        get => File.FrontDriveRatio;
        set => Change(p => p.FrontDriveRatio, value);
    }

    /// <inheritdoc/>
    public double FrontGripBias
    {
        get => File.FrontGripBias;
        set => Change(p => p.FrontGripBias, value);
    }

    /// <inheritdoc/>
    public IList<byte> GasDecreaseCurve => _gasDecreaseCurve ??= GetObservable(File.GasDecreaseCurve);

    /// <inheritdoc/>
    public IList<byte> GasIncreaseCurve => _gasIncreaseCurve ??= GetObservable(File.GasIncreaseCurve);

    /// <inheritdoc/>
    public double GasOffFactor
    {
        get => File.GasOffFactor;
        set => Change(p => p.GasOffFactor, value);
    }

    /// <inheritdoc/>
    public IList<double> GearEfficiency => _gearEfficiency ??= GetObservable(File.GearEfficiency);

    /// <inheritdoc/>
    public int GearShiftDelay
    {
        get => File.GearShiftDelay;
        set => Change(p => p.GearShiftDelay, value);
    }

    /// <inheritdoc/>
    public double GTransferFactor
    {
        get => File.GTransferFactor;
        set => Change(p => p.GTransferFactor, value);
    }

    /// <inheritdoc/>
    public double HighTurnFactor
    {
        get => File.HighTurnFactor;
        set => Change(p => p.HighTurnFactor, value);
    }

    /// <inheritdoc/>
    public double LateralAccGripMult
    {
        get => File.LateralAccGripMult;
        set => Change(p => p.LateralAccGripMult, value);
    }

    /// <inheritdoc/>
    public double LowTurnFactor
    {
        get => File.LowTurnFactor;
        set => Change(p => p.LowTurnFactor, value);
    }

    /// <inheritdoc/>
    public double Mass
    {
        get => File.Mass;
        set => Change(p => p.Mass, value);
    }

    /// <inheritdoc/>
    public double MaxBrakeDecel
    {
        get => File.MaxBrakeDecel;
        set => Change(p => p.MaxBrakeDecel, value);
    }

    /// <inheritdoc/>
    public double MaxVelocity
    {
        get => File.MaxVelocity;
        set => Change(p => p.MaxVelocity, value);
    }

    /// <inheritdoc/>
    public int NumberOfGears
    {
        get => File.NumberOfGears;
        set => Change(p => p.NumberOfGears, value);
    }

    /// <inheritdoc/>
    public double PushFactor
    {
        get => File.PushFactor;
        set => Change(p => p.PushFactor, value);
    }

    /// <inheritdoc/>
    public double SlideAssistanceFactor
    {
        get => File.SlideAssistanceFactor;
        set => Change(p => p.SlideAssistanceFactor, value);
    }

    /// <inheritdoc/>
    public double SlideMult
    {
        get => File.SlideMult;
        set => Change(p => p.SlideMult, value);
    }

    /// <inheritdoc/>
    public double SlideVelocityCap
    {
        get => File.SlideVelocityCap;
        set => Change(p => p.SlideVelocityCap, value);
    }

    /// <inheritdoc/>
    public double SpinVelocityCap
    {
        get => File.SpinVelocityCap;
        set => Change(p => p.SpinVelocityCap, value);
    }

    /// <inheritdoc/>
    public IList<double> TorqueCurve => _torqueCurve ??= GetObservable(File.TorqueCurve);

    /// <inheritdoc/>
    public double TurnInRamp
    {
        get => File.TurnInRamp;
        set => Change(p => p.TurnInRamp, value);
    }

    /// <inheritdoc/>
    public double TurnOutRamp
    {
        get => File.TurnOutRamp;
        set => Change(p => p.TurnOutRamp, value);
    }

    /// <inheritdoc/>
    public IList<double> VelocityToRpm => _velocityToRpm ??= GetObservable(File.VelocityToRpm);

    /// <inheritdoc/>
    public double WheelBase
    {
        get => File.WheelBase;
        set => Change(p => p.WheelBase, value);
    }
}
