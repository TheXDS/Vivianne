using System.Windows.Controls;
using TheXDS.Vivianne.Controls.Base;
using Wpf.Ui.Controls;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements an <see cref="ItemsControl"/> that displays an array of
/// <see cref="NumberBox"/> controls that allow the user to directly edit a
/// collection of <see cref="double"/> values.
/// </summary>
public class DoubleCollectionEditor : CollectionEditorBase<double>;