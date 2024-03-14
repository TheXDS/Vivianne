using System.Windows.Controls;
using TheXDS.Vivianne.Helpers;

namespace TheXDS.Vivianne.Views;

/// <summary>
/// View that allows the user to interacti with a FSH textures file.
/// </summary>
public partial class FshEditorView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FshEditorView"/> class.
    /// </summary>
    public FshEditorView()
    {
        InitializeComponent();
        _ = new ScrollHookHelper(brdContent, scvContent, sldZoom);
    }
}
