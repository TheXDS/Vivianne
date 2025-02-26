using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Base;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.CarpEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Carp;

/// <summary>
/// Implements a ViewModel that alows the user to edit Carp data.
/// </summary>
public class CarpEditorViewModel<TState, TFile, TCarClass> : FileEditorViewModelBase<TState, TFile>
    where TState : CarpEditorState<TFile, TCarClass>, new()
    where TCarClass : unmanaged, Enum
    where TFile : CarPerf<TCarClass>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarpEditorViewModel{TState, TFile, TCarClass}"/> class.
    /// </summary>
    public CarpEditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        EditIntCurveCommand = cb.BuildSimple(OnEditIntCurve);
        EditDoubleCurveCommand = cb.BuildSimple(OnEditDoubleCurve);
        CopyTransToAutoCommand = cb.BuildSimple(OnCopyTransToAuto);
        CopyTransToManualCommand = cb.BuildSimple(OnCopyTransToManual);
        CopyTiresToFrontCommand = cb.BuildSimple(OnCopyTiresToFront);
        CopyTiresToRearCommand = cb.BuildSimple(OnCopyTiresToRear);
        FedataSyncCommand = cb.BuildSimple(OnFeDataSync);
        CarpWizardCommand = cb.BuildSimple(OnCarpWizard);
        PerformanceMetricsCommand = cb.BuildSimple(OnPerformanceMetrics);
    }

    /// <summary>
    /// Gets a reference to the command used to run a wizard to generate
    /// realistic Carp files.
    /// </summary>
    public ICommand CarpWizardCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to sync up Carp data with FeData
    /// files.
    /// </summary>
    public ICommand FedataSyncCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to edit a curve of
    /// <see cref="int"/> values.
    /// </summary>
    public ICommand EditIntCurveCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to edit a curve of
    /// <see cref="double"/> values.
    /// </summary>
    public ICommand EditDoubleCurveCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy gearbox data from manual
    /// to automatic.
    /// </summary>
    public ICommand CopyTransToAutoCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy gearbox data from
    /// automatic to manual.
    /// </summary>
    public ICommand CopyTransToManualCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy the tire data from the
    /// rear tires to the front tires.
    /// </summary>
    public ICommand CopyTiresToFrontCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to copy the tire data from the
    /// front tires to the rear tires.
    /// </summary>
    public ICommand CopyTiresToRearCommand { get; }

    /// <summary>
    /// Gets a reference to the command used to calculate and expose some
    /// performance metrics based on the current Carp data.
    /// </summary>
    public ICommand PerformanceMetricsCommand { get; }

    private Task OnEditDoubleCurve(object? parameter)
    {
        return parameter switch
        {
            ICollection<double> c => RunCurveEditor(c),
            _ => Task.CompletedTask
        };
    }

    private async Task OnEditIntCurve(object? parameter)
    {
        ICollection<int>? collection = null;
        ICollection<double>? doubleCollection = null;
        switch (parameter)
        {
            case ICollection<int> c:
                collection = c;
                doubleCollection = await RunCurveEditor([.. c.Select(p => (double)p)]);
                break;
        }
        if (collection is not null && doubleCollection is not null)
        {
            collection.Clear();
            collection.AddRange(doubleCollection.Select(p => (int)p));
        }
    }

    private void OnFeDataSync()
    {
        throw new NotImplementedException();
        //if (_vivFileRef is not null)
        //{
        //    UiThread.Invoke(() => FeData3SyncTool.Sync(State.ToCarp(), _vivFileRef.Directory));
        //    await DialogService!.Message(St.FeDataSync, St.OperationCompletedSuccessfully);
        //}
    }

    private void OnCarpWizard()
    {
        throw new NotImplementedException();
    }

    private void OnCopyTransToManual()
    {
        State.File.NumberOfGearsManual = State.File.NumberOfGearsAuto;
        State.File.FinalGearManual = State.File.FinalGearAuto;
        State.File.VelocityToRpmManual.Clear();
        State.File.VelocityToRpmManual.AddRange(State.File.VelocityToRpmAuto);
        State.File.GearRatioManual.Clear();
        State.File.GearRatioManual.AddRange(State.File.GearRatioAuto);
        State.File.GearEfficiencyManual.Clear();
        State.File.GearEfficiencyManual.AddRange(State.File.GearEfficiencyAuto);
    }

    private void OnCopyTransToAuto()
    {
        State.File.NumberOfGearsAuto = State.File.NumberOfGearsManual;
        State.File.FinalGearAuto = State.File.FinalGearManual;
        State.File.VelocityToRpmAuto.Clear();
        State.File.VelocityToRpmAuto.AddRange(State.File.VelocityToRpmManual);
        State.File.GearRatioAuto.Clear();
        State.File.GearRatioAuto.AddRange(State.File.GearRatioManual);
        State.File.GearEfficiencyAuto.Clear();
        State.File.GearEfficiencyAuto.AddRange(State.File.GearEfficiencyManual);
    }

    private void OnCopyTiresToFront()
    {
        State.File.TireWidthFront = State.File.TireWidthRear;
        State.File.TireSidewallFront = State.File.TireSidewallRear;
        State.File.TireRimFront = State.File.TireRimRear;
    }

    private void OnCopyTiresToRear()
    {
        State.File.TireWidthRear = State.File.TireWidthFront;
        State.File.TireSidewallRear = State.File.TireSidewallFront;
        State.File.TireRimRear = State.File.TireRimFront;
    }

    private void Vm_StateSaved(object? sender, EventArgs e)
    {
        State.UnsavedChanges = true;
    }

    private async Task<ICollection<double>> RunCurveEditor(ICollection<double> c)
    {
        var vm = new CurveEditorDialogViewModel(new(c)) { Message = St.EditCurve };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.Show(vm);
        vm.StateSaved -= Vm_StateSaved;
        return vm.State.TargetCollection;
    }

    private Task OnPerformanceMetrics()
    {
        var a = Mappings.GetTextProviderFromCulture(State.File);
        return DialogService!.Message(St.PerformanceMetrics, string.Format(St.PerformanceMetricsDetails,
            a.Weight,
            a.TopSpeed,
            a.Power,
            a.Torque,
            a.MaxRpm,
            a.Tires,
            a.Gearbox,
            a.Accel0To60,
            a.Accel0To100));
    }
}