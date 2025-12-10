using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Nfs2;
using TheXDS.Vivianne.ViewModels.Base;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Ganymede.Models;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Models.Carp;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.CarpEditorViewModel;
using Carp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Carp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using Carp3Ser = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using Carp4Ser = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;

namespace TheXDS.Vivianne.ViewModels.Carp.Nfs2;

/// <summary>
/// Implements a ViewModel for NFS2 Carp files.
/// </summary>
public class CarpEditorViewModel : StatefulFileEditorViewModelBase<CarpEditorState, CarPerf>
{
    /// <summary>
    /// Gets a reference to the command used to edit a curve of
    /// <see cref="int"/> values.
    /// </summary>
    public ICommand EditByteCurveCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to edit a curve of
    /// <see cref="double"/> values.
    /// </summary>
    public ICommand EditDoubleCurveCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to upgrade the current Carp2 data to Carp3/4.
    /// </summary>
    public ICommand UpgradeToCarp34Command { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CarpEditorViewModel"/> class.
    /// </summary>
    public CarpEditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        EditByteCurveCommand = cb.BuildSimple(OnEditByteCurve);
        EditDoubleCurveCommand = cb.BuildSimple(OnEditDoubleCurve);
        UpgradeToCarp34Command = cb.BuildSimple(OnUpgradeToCarp34);
    }

    private async Task OnUpgradeToCarp34()
    {
        if (DialogService is not null && await DialogService.SelectOption(CommonDialogTemplates.Input, new MCART.Types.NamedObject<int>("Carp3", 0), new MCART.Types.NamedObject<int>("Carp4", 1)) is { Success: true, Result: int result })
        {
            switch (result)
            {
                case 0:
                    await SaveConvertedCarp<Carp3Ser, Carp3>(UpgradeToCarp3);
                    break;
                case 1:
                    await SaveConvertedCarp<Carp4Ser, Carp4>(UpgradeToCarp4);
                    break;
            }
        }
    }

    private async Task SaveConvertedCarp<TSerializer, TCarp>(Func<CarPerf, TCarp> convertCallback)
        where TCarp : class, ICarPerf
        where TSerializer : ISerializer<TCarp>, new()
    {
        var newCarp = convertCallback.Invoke(State.File);
        
        if (await BackingStore!.Store.GetNewFileName("carp.txt") is { Success: true, Result: string carpSavePath })
        {
            await BackingStore.Store.WriteAsync(carpSavePath, await new TSerializer().SerializeAsync(newCarp));
        }
    }

    private Task OnEditDoubleCurve(object? parameter)
    {
        return parameter switch
        {
            ICollection<double> c => RunCurveEditor(c, false),
            _ => Task.CompletedTask
        };
    }

    private async Task OnEditByteCurve(object? parameter)
    {
        ICollection<byte>? collection = null;
        ICollection<double>? doubleCollection = null;
        switch (parameter)
        {
            case ICollection<byte> c:
                collection = c;
                doubleCollection = await RunCurveEditor([.. c.Select(p => (double)p)]);
                break;
        }
        if (collection is not null && doubleCollection is not null)
        {
            collection.Clear();
            collection.AddRange(doubleCollection.Select(p => (byte)p));
        }
    }

    private void Vm_StateSaved(object? sender, EventArgs e)
    {
        State.UnsavedChanges = true;
    }

    private async Task<ICollection<double>> RunCurveEditor(ICollection<double> c, bool allowCollectionGrow = true)
    {
        var vm = new CurveEditorDialogViewModel(new(c), allowCollectionGrow) { Message = St.EditCurve };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.Show(vm);
        vm.StateSaved -= Vm_StateSaved;
        return vm.State.TargetCollection;
    }

    private static Carp3 UpgradeToCarp3(CarPerf carp2)
    {
        var carp3 = UpgradeToCarp<Carp3>(carp2);
        carp3.CarClass = Models.Fe.Nfs3.CarClass.A;
        return carp3;
    }

    private static Carp4 UpgradeToCarp4(CarPerf carp2)
    {
        var carp4 = UpgradeToCarp<Carp4>(carp2);
        carp4.CarClass = Models.Fe.Nfs4.CarClass.A;
        return carp4;
    }

    private static TCarp UpgradeToCarp<TCarp>(CarPerf carp2)
        where TCarp : class, ICarPerf, new()
    {
        var newCarp = new TCarp
        {
            SerialNumber = (ushort)Random.Shared.Next(64, 65535),
            Mass = carp2.Mass,
            NumberOfGearsManual = carp2.NumberOfGears,
            NumberOfGearsAuto = carp2.NumberOfGears,
            GearShiftDelay = carp2.GearShiftDelay,
            FinalGearManual = 1,
            FinalGearAuto = 1,
            EngineMinRpm = 800,
            EngineMaxRpm = carp2.EngineMaxRpm,
            MaxVelocity = carp2.MaxVelocity,
            TopSpeed = carp2.MaxVelocity,
            FrontDriveRatio = carp2.FrontDriveRatio,
            Abs = true,
            MaxBrakeDecel = carp2.MaxBrakeDecel,
            FrontBrakeBias = carp2.FrontBrakeBias,
            WheelBase = carp2.WheelBase,
            FrontGripBias = carp2.FrontGripBias,
            PowerSteering = true,
            MinimumSteerAccel = 18.250000,
            TurnInRamp = carp2.TurnInRamp,
            TurnOutRamp = carp2.TurnOutRamp,
            LateralAccGripMult = carp2.LateralAccGripMult,
            AeroDownMult = carp2.AeroDownMult,
            GasOffFactor = carp2.GasOffFactor,
            GTransferFactor = carp2.GTransferFactor,
            TurnCircleRadius = 6.300000,
            TireWidthFront = 245,
            TireSidewallFront = 35,
            TireRimFront = 18,
            TireWidthRear = 335,
            TireSidewallRear = 35,
            TireRimRear = 18,
            TireWear = 0.000000,
            SlideMult = carp2.SlideMult,
            SpinVelocityCap = carp2.SpinVelocityCap,
            SlideVelocityCap = carp2.SlideVelocityCap,
            SlideAssistanceFactor = carp2.SlideAssistanceFactor,
            PushFactor = carp2.PushFactor,
            LowTurnFactor = carp2.LowTurnFactor,
            HighTurnFactor = carp2.HighTurnFactor,
            PitchRollFactor = 0.920000,
            RoadBumpFactor = 0.910000,
            SpoilerFunctionType = 8,
            SpoilerActivationSpeed = 0.000000,
            GradualTurnCutoff = 110,
            MediumTurnCutoff = 160,
            SharpTurnCutoff = 180,
            MediumTurnSpdMod = 0.970000,
            SharpTurnSpdMod = 0.950000,
            ExtremeTurnSpdMod = 0.930000,
            SubdivideLevel = 3,
            CameraArm = 0.250000,
            BodyDamage = 0.000000,
            EngineDamage = 0.000000,
            SuspensionDamage = 0.000000,
            EngineTuning = 1.000000,
            BrakeBalance = 0.000000,
            SteeringSpeed = 1.000000,
            GearRatFactor = 1.000000,
            SuspensionStiffness = 1.114755,
            AeroFactor = 1.000000,
            TireFactor = 1.000000,
            AiCurve0 = { 4.266667, 6.349520, 7.737358, 8.536504, 8.955383, 9.161016, 9.224277, 9.157882, 9.052475, 9.028872, 9.089978, 9.139469, 9.096681, 9.015980 },
            AiCurve1 = { 9.009326, 9.113148, 9.310430, 9.504269, 9.512443, 9.131333, 8.316323, 7.362495, 6.644360, 6.281654, 6.185064, 6.216635, 6.278254, 6.316365 },
            AiCurve2 = { 6.292465, 6.171501, 5.896525, 5.472954, 5.072002, 4.847321, 4.760395, 4.711891, 4.667427, 4.630373, 4.604499, 4.588714, 4.548230, 4.393620 },
            AiCurve3 = { 4.094394, 3.763382, 3.514253, 3.351206, 3.247369, 3.205148, 3.212960, 3.218600, 3.184813, 3.129700, 3.083073, 3.045386, 3.004708, 2.939694 },
            AiCurve4 = { 2.808251, 2.604247, 2.396461, 2.251089, 2.168125, 2.114827, 2.067707, 2.019555, 1.972794, 1.922291, 1.852626, 1.778252, 1.746975, 1.770979 },
            AiCurve5 = { 1.809858, 1.827267, 1.815285, 1.777023, 1.717956, 1.646383, 1.569493, 1.491768, 1.415381, 1.339153, 1.260314, 1.177396, 1.090600, 1.000228 },
            AiCurve6 = { 0.905302, 0.803205, 0.690751, 0.566915, 0.434530, 0.299065, 0.140351, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000 },
            AiCurve7 = { 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000 }
        };
        newCarp.VelocityToRpmManual.AddRange(carp2.VelocityToRpm.Take(carp2.NumberOfGears));
        newCarp.VelocityToRpmAuto.AddRange(newCarp.VelocityToRpmManual);
        newCarp.GearRatioManual.AddRange(CalculateGearRatios(carp2.VelocityToRpm.Take(carp2.NumberOfGears).ToArray()[^2], carp2.VelocityToRpm.Take(carp2.NumberOfGears)));
        newCarp.GearRatioAuto.AddRange(newCarp.GearRatioManual);
        newCarp.TorqueCurve.AddRange(carp2.TorqueCurve);
        newCarp.GasIncreaseCurve.AddRange(carp2.GasIncreaseCurve.Select(p => (double)p));
        newCarp.GasDecreaseCurve.AddRange(carp2.GasDecreaseCurve.Select(p => (double)p));
        newCarp.BrakeIncreaseCurve.AddRange(carp2.BrakeIncreaseCurve.Select(p => (double)p));
        newCarp.BrakeDecreaseCurve.AddRange(carp2.BrakeDecreaseCurve.Select(p => (double)p));
        newCarp.ShiftBlip.AddRange(((double[])[0, 0, 125, 115, 90, 80, 75, 55]).Take(carp2.NumberOfGears));
        newCarp.BrakeBlip.AddRange(((double[])[0, 0, 115, 100, 85, 75, 60, 50]).Take(carp2.NumberOfGears));
        newCarp.GearEfficiencyManual.AddRange(((double[])[0.800000, 0.000000, 0.770000, 0.790000, 0.820000, 0.840000, 0.850000, 1.150000]).Take(carp2.NumberOfGears));
        newCarp.GearEfficiencyAuto.AddRange(((double[])[0.800000, 0.000000, 0.710000, 0.710000, 0.740000, 0.740000, 0.800000, 1.150000]).Take(carp2.NumberOfGears));
        return newCarp;
    }

    private static IEnumerable<double> CalculateGearRatios(double referenceVelocity, IEnumerable<double> velocities)
    {
        foreach (var j in velocities)
        {
            if (j == 0) yield return 0;
            else yield return double.Round(double.Abs(j / referenceVelocity), 2);
        }
    }
}
