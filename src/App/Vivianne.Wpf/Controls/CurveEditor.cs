using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using TheXDS.MCART.Types.Base;
using static TheXDS.Ganymede.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

[ContentProperty(nameof(Curve))]
public class CurveEditor : Control
{
    public static DependencyPropertyKey CurvePropertyKey;
    public static DependencyProperty CurveProperty;
    public static DependencyProperty CollectionProperty;
    public static DependencyProperty MinimumProperty;
    public static DependencyProperty MaximumProperty;
    public static DependencyProperty StepProperty;
    public static DependencyProperty BarWidthProperty;

    static CurveEditor()
    {
        MinimumProperty = NewDp<double, CurveEditor>(nameof(Minimum), 0.0);
        MaximumProperty = NewDp<double, CurveEditor>(nameof(Maximum), 100.0);
        StepProperty = NewDp<double, CurveEditor>(nameof(Step), 10.0);
        (CurvePropertyKey, CurveProperty) = NewDpRo<ICollection<DoubleValue>, CurveEditor>(nameof(Curve), null!);
        CollectionProperty = NewDp2Way<ICollection<double>, CurveEditor>(nameof(Collection), null!,OnCollectionChanged);
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CurveEditor), new FrameworkPropertyMetadata(typeof(CurveEditor)));
        BarWidthProperty = NewDp<double, CurveEditor>(nameof(BarWidth), 35);
    }

    private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var value = e.NewValue as ICollection<double>;
        d.SetValue(CurvePropertyKey, new List<DoubleValue>(value.Select(p => new DoubleValue() { Value = p })));
    }

    public ICollection<DoubleValue> Curve
    {
        get => (ICollection<DoubleValue>)GetValue(CurveProperty);
    }

    public ICollection<double> Collection
    {
        get => (ICollection<double>)GetValue(CollectionProperty);
        set => SetValue(CollectionProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public double BarWidth
    {
        get => (double)GetValue(BarWidthProperty);
        set => SetValue(BarWidthProperty, value);
    }
}

public class DoubleValue : NotifyPropertyChanged
{
    private double _Value;

    public double Value
    {
        get => _Value;
        set => Change(ref _Value, value);
    }
}
