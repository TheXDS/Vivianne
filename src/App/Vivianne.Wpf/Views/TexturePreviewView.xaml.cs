using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.Vivianne.Helpers;

namespace TheXDS.Vivianne.Views
{
    /// <summary>
    /// View that allows the user to preview a TGA texture.
    /// </summary>
    public partial class TexturePreviewView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TexturePreviewView"/>
        /// class.
        /// </summary>
        public TexturePreviewView()
        {
            InitializeComponent();
            _ = new ScrollHookHelper(brdContent, scvContent, sldZoom);

        }
    }
}
