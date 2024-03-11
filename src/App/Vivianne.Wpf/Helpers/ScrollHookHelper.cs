using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TheXDS.MCART.Math;

namespace TheXDS.Vivianne.Helpers;

internal class ScrollHookHelper
{
    private const double MouseWheelZoom = 0.125;
    private readonly Border brdContent;
    private readonly ScrollViewer scvContent;
    private readonly RangeBase rngZoom;
    private Point _scrollMousePoint;
    private double _hOff = 1;
    private double _vOff = 1;

    public ScrollHookHelper(Border brdContent, ScrollViewer scvContent, RangeBase rngZoom)
    {
        this.brdContent = brdContent ?? throw new ArgumentNullException(nameof(brdContent));
        this.scvContent = scvContent ?? throw new ArgumentNullException(nameof(scvContent));
        this.rngZoom = rngZoom ?? throw new ArgumentNullException(nameof(rngZoom));
        brdContent.MouseLeftButtonDown += Sv_MouseLeftButtonDown;
        brdContent.PreviewMouseMove += Sv_PreviewMouseMove;
        brdContent.PreviewMouseLeftButtonUp += Sv_PreviewMouseLeftButtonUp;
        brdContent.PreviewMouseWheel += BrdContent_PreviewMouseWheel;
    }

    private void BrdContent_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        rngZoom.Value = (rngZoom.Value + (e.Delta > 0 ? MouseWheelZoom : -MouseWheelZoom)).Clamp(1.0, 10.0);
        rngZoom.GetBindingExpression(RangeBase.ValueProperty).UpdateSource();
        e.Handled = true;
    }

    private void Sv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        brdContent.CaptureMouse();
        _scrollMousePoint = e.GetPosition(scvContent);
        _hOff = scvContent.HorizontalOffset;
        _vOff = scvContent.VerticalOffset;
    }

    private void Sv_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (brdContent.IsMouseCaptured)
        {
            var newXOffset = _hOff + (_scrollMousePoint.X - e.GetPosition(scvContent).X);
            var newYOffset = _vOff + (_scrollMousePoint.Y - e.GetPosition(scvContent).Y);
            scvContent.ScrollToHorizontalOffset(newXOffset);
            scvContent.ScrollToVerticalOffset(newYOffset);
        }
    }

    private void Sv_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        brdContent.ReleaseMouseCapture();
    }

    ~ScrollHookHelper()
    {
        brdContent.MouseLeftButtonDown -= Sv_MouseLeftButtonDown;
        brdContent.PreviewMouseMove -= Sv_PreviewMouseMove;
        brdContent.PreviewMouseLeftButtonUp -= Sv_PreviewMouseLeftButtonUp;
        brdContent.PreviewMouseWheel -= BrdContent_PreviewMouseWheel;
    }
}
