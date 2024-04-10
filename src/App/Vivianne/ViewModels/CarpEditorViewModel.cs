using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that alows the user to edit Carp data.
/// </summary>
public class CarpEditorViewModel : EditorViewModelBase<CarpEditorState>
{
    private readonly Action<string> _saveCallback;
    private readonly VivFile? _vivFileRef;

    public CarpEditorViewModel(CarpEditorState state, Action<string> saveCallback, VivFile? vivFileRef = null) : base(state)
    {
        _saveCallback = saveCallback;
        _vivFileRef = vivFileRef;

        EditIntCurveCommand = new SimpleCommand(OnEditIntCurve);
        EditDoubleCurveCommand = new SimpleCommand(OnEditDoubleCurve);
        CopyTransToAutoCommand = new SimpleCommand(OnCopyTransToAuto);
        CopyTransToManualCommand = new SimpleCommand(OnCopyTransToManual);
        CopyTiresToFrontCommand = new SimpleCommand(OnCopyTiresToFront);
        CopyTiresToRearCommand = new SimpleCommand(OnCopyTiresToRear);
        FedataSyncCommand = new SimpleCommand(OnFeDataSync, vivFileRef is not null);
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

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        _saveCallback.Invoke(State.ToString());
        return Task.CompletedTask;
    }

    private Task OnEditDoubleCurve(object? parameter)
    {
        return parameter switch
        {
            CollectionDescriptor d when GetCollection<double>(d.Collection) is { } c => RunCurveEditor(c, d),
            ICollection<double> c => RunCurveEditor(c, new() { Minimum = 0, Maximum = 100, Step = 10, BarWidth = 40 }),
            _ => Task.CompletedTask
        };
    }

    private async Task OnEditIntCurve(object? parameter)
    {
        ICollection<int>? collection = null;
        ICollection<double>? doubleCollection = null;
        switch (parameter)
        {
            case CollectionDescriptor d when GetCollection<int>(d.Collection) is { } c:
                collection = c;
                doubleCollection = await RunCurveEditor(c.Select(p => (double)p).ToList(), d);
                break;
            case ICollection<int> c:
                collection = c;
                doubleCollection = await RunCurveEditor(c.Select(p => (double)p).ToList());
                break;
        }
        if (collection is not null && doubleCollection is not null)
        {
            collection.Clear();
            collection.AddRange(doubleCollection.Select(p => (int)p));
        }
    }

    private async Task OnFeDataSync()
    {

    }

    private void OnCopyTransToManual()
    {
        State.NumberOfGearsManual = State.NumberOfGearsAuto;
        State.FinalGearManual = State.FinalGearAuto;
        State.VelocityToRpmManual.Clear();
        State.VelocityToRpmManual.AddRange(State.VelocityToRpmAuto);
        State.GearRatioManual.Clear();
        State.GearRatioManual.AddRange(State.GearRatioAuto);
        State.GearEfficiencyManual.Clear();
        State.GearEfficiencyManual.AddRange(State.GearEfficiencyAuto);
    }

    private void OnCopyTransToAuto()
    {
        State.NumberOfGearsAuto = State.NumberOfGearsManual;
        State.FinalGearAuto = State.FinalGearManual;
        State.VelocityToRpmAuto.Clear();
        State.VelocityToRpmAuto.AddRange(State.VelocityToRpmManual);
        State.GearRatioAuto.Clear();
        State.GearRatioAuto.AddRange(State.GearRatioManual);
        State.GearEfficiencyAuto.Clear();
        State.GearEfficiencyAuto.AddRange(State.GearEfficiencyManual);
    }

    private void OnCopyTiresToFront()
    {
        State.TireWidthFront = State.TireWidthRear;
        State.TireSidewallFront = State.TireSidewallRear;
        State.TireRimFront = State.TireRimRear;
    }

    private void OnCopyTiresToRear()
    {
        State.TireWidthRear = State.TireWidthFront;
        State.TireSidewallRear = State.TireSidewallFront;
        State.TireRimRear = State.TireRimFront;
    }

    private void Vm_StateSaved(object? sender, EventArgs e)
    {
        State.UnsavedChanges = true;
    }

    private async Task<ICollection<double>> RunCurveEditor(ICollection<double> c, CollectionDescriptor? d = null)
    {
        Range<double> rng;
        d ??= new() { Minimum = 0, Maximum = 100, Step = 10, BarWidth = 40 };
        if (KeyboardProxy.IsShiftKeyDown)
        {
            var result = await DialogService!.GetInputRange("Edit curve", "Select a value range to edit this curve", d.Minimum, d.Maximum);
            if (!result) return [];
            rng = new(result.Result.Min, result.Result.Max);
        }
        else
        {
            rng = new(d.Minimum, d.Maximum);
        }
        var vm = new CurveEditorDialogViewModel(new(c) { Minimum = rng.Minimum, Maximum = rng.Maximum, Step = d.Step, BarWidth = d.BarWidth }) { Message = d.Message };
        vm.StateSaved += Vm_StateSaved;
        await DialogService!.CustomDialog(vm);
        vm.StateSaved -= Vm_StateSaved;
        return vm.State.TargetCollection;
    }

    private ICollection<T>? GetCollection<T>(string name) where T : unmanaged
    {
        return typeof(CarpEditorState).GetProperty(name)?.GetValue(State) as ICollection<T>;
    }
}