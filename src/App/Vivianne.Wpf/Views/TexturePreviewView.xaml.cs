using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TheXDS.Vivianne.Views
{
    /// <summary>
    /// View that allows the user to preview a TGA texture.
    /// </summary>
    public partial class TexturePreviewView : UserControl
    {
        private Point _scrollMousePoint;
        private double _hOff = 1;
        private double _vOff = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturePreviewView"/>
        /// class.
        /// </summary>
        public TexturePreviewView()
        {
            InitializeComponent();
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
    }
}
