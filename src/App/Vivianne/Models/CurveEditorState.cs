using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

public class CurveEditorState : EditorViewModelStateBase
{
    private double _Minimum = 0;
    private double _Maximum = 100;
    private double _Step = 10;
    private double _BarWidth = 60;

    public CurveEditorState(ICollection<double> targetCollection)
    {
        TargetCollection = targetCollection;
        Collection = TargetCollection.Copy().ToList();
    }

    public ICollection<double> Collection { get; }

    public ICollection<double> TargetCollection { get; }

    public double Minimum
    {
        get => _Minimum;
        set => Change(ref _Minimum, value);
    }

    public double Maximum
    {
        get => _Maximum;
        set => Change(ref _Maximum, value);
    }

    public double Step
    {
        get => _Step;
        set => Change(ref _Step, value);
    }


    public double BarWidth
    {
        get => _BarWidth;
        set => Change(ref _BarWidth, value);
    }
}
