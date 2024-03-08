using System;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that allows the user to see and edit FeData car
/// information files.
/// </summary>
public class FeDataPreviewViewModel : ViewModel
{
    private static readonly ISerializer<FeData> serializer = new FeDataSerializer();
    private readonly Action<byte[]> saveCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeDataPreviewViewModel"/>
    /// class.
    /// </summary>
    /// <param name="data">FeData to manipulate.</param>
    /// <param name="saveCallback">
    /// Callback to invoke when saving the FeData contents.
    /// </param>
    public FeDataPreviewViewModel(byte[] data, Action<byte[]> saveCallback)
    {
        this.saveCallback = saveCallback;
        Data = serializer.Deserialize(data);
        SaveCommand = new SimpleCommand(OnSave);
    }

    /// <summary>
    /// Gets the <see cref="FeData"/> instance to view/edit.
    /// </summary>
    public FeData Data { get; }

    /// <summary>
    /// Gets a reference to the command used to save the changes made to the FeData File.
    /// </summary>
    public ICommand SaveCommand { get; } 

    private void OnSave()
    {
        saveCallback?.Invoke(serializer.Serialize(Data));
    }
}