using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.Vivianne.Helpers;

namespace TheXDS.Vivianne.Views.Geo;

/// <summary>
/// Interaction logic for GeoEditorView.xaml
/// </summary>
public partial class GeoEditorView : UserControl
{
    Point from;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoEditorView"/> class.
    /// </summary>
    public GeoEditorView()
    {
        InitializeComponent();
        PreviewMouseMove += Window_PreviewMouseMove;
    }

    private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        var till = e.GetPosition(sender as IInputElement);
        double dx = (till.X - from.X) * -2;
        double dy = (till.Y - from.Y) * -3;
        from = till;

        var distance = (dx * dx) + (dy * dy);
        if (distance <= 0d) return;
        if (e.MouseDevice.LeftButton is MouseButtonState.Pressed)
        {
            var angle = (distance / ptcMain.FieldOfView) % 45d;
            ptcMain.Rotate(new(dx, dy, 0d), angle);
        }
    }
}
