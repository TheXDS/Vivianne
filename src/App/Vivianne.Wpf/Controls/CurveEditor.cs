using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using TheXDS.MCART.Types.Base;
using static TheXDS.Ganymede.Helpers.DependencyObjectHelpers;


namespace TheXDS.Vivianne.Controls;

public partial class EnumValProvider : MarkupExtension
{
    public Type EnumType { get; set; }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(EnumType);
    }
}



[ContentProperty(nameof(Curve))]
public class CurveEditor : Control
{
    public static DependencyProperty CurveProperty;

    public static DependencyProperty AllowAddAndRemoveProperty;

    static CurveEditor()
    {
        AllowAddAndRemoveProperty = NewDp<bool, CurveEditor>(nameof(AllowAddAndRemove));
        CurveProperty = NewDp2Way<ObservableCollection<DoubleValue>, CurveEditor>(nameof(Curve), []);
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CurveEditor), new FrameworkPropertyMetadata(typeof(CurveEditor)));
    }

    public bool AllowAddAndRemove
    {
        get => (bool)GetValue(AllowAddAndRemoveProperty);
        set => SetValue(AllowAddAndRemoveProperty, value);
    }

    public ObservableCollection<DoubleValue> Curve
    {
        get => (ObservableCollection<DoubleValue>)GetValue(CurveProperty);
        set => SetValue(CurveProperty, value);
    }
}

[ContentProperty(nameof(Value))]
public class DoubleValue : NotifyPropertyChanged
{
    private double _Value;

    public double Value
    {
        get => _Value;
        set => Change(ref _Value, value);
    }

    public static implicit operator DoubleValue(double value) => new() { Value = value };
    public static implicit operator double(DoubleValue value) => value.Value;

}
