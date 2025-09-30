using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Nfs2;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.ViewModels.CarpEditorViewModel;

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
    /// Initializes a new instance of the <see cref="CarpEditorViewModel"/> class.
    /// </summary>
    public CarpEditorViewModel()
    {
        var cb = CommandBuilder.For(this);
        EditByteCurveCommand = cb.BuildSimple(OnEditByteCurve);
        EditDoubleCurveCommand = cb.BuildSimple(OnEditDoubleCurve);
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
}
