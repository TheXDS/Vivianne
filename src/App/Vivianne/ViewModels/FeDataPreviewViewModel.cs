using System;
using System.IO;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
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
    private readonly VivFile? viv;
    private readonly string? fedataName;
    private bool _LinkEdits;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeDataPreviewViewModel"/>
    /// class.
    /// </summary>
    /// <param name="data">FeData to manipulate.</param>
    /// <param name="saveCallback">
    /// Callback to invoke when saving the FeData contents.
    /// </param>
    /// <param name="viv">
    /// Reference to the VIV file when attempting to sync up changes on other
    /// files.
    /// </param>
    public FeDataPreviewViewModel(byte[] data, Action<byte[]> saveCallback, VivFile? viv = null, string? fedataName = null)
    {
        this.saveCallback = saveCallback;
        this.viv = viv;
        this.fedataName = fedataName;
        Data = serializer.Deserialize(data);
        SaveCommand = new SimpleCommand(OnSave);
    }

    /// <summary>
    /// Gets the <see cref="FeData"/> instance to view/edit.
    /// </summary>
    public FeData Data { get; }

    /// <summary>
    /// Gets a reference to the command used to save the changes made to the
    /// FeData File.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Gets or sets a value that indicates that changes made on the FeData
    /// editor should be synced up on other FeData files and the Carp.txt file.
    /// </summary>
    public bool LinkEdits
    {
        get => _LinkEdits;
        set => Change(ref _LinkEdits, value);
    }

    private void OnSave()
    {

        saveCallback?.Invoke(serializer.Serialize(Data));
    }

    private void OnSyncChanges()
    {
        if (viv is null) return;
        ISerializer<FeData> s = new FeDataSerializer();
        foreach (var j in FeData.KnownExtensions.ExceptFor(Path.GetExtension(fedataName)))
        {
            if (viv.TryGetValue($"fedata{j}", out var content))
            {
                var f = s.Deserialize(content);
                f.CarName = Data.CarName;
                f.CarId = Data.CarId;
                f.SerialNumber = Data.SerialNumber;
                f.VehicleClass = Data.VehicleClass;
                f.Seat = Data.Seat;
                f.IsPolice = Data.IsPolice;
                f.IsBonus = Data.IsBonus;
                f.AvailableToAi = Data.AvailableToAi;
                f.IsDlcCar = Data.IsDlcCar;
                f.CarAccel = Data.CarAccel;
                f.CarTopSpeed = Data.CarTopSpeed;
                f.CarHandling = Data.CarHandling;
                f.CarBraking = Data.CarBraking;
                f.Manufacturer = Data.Manufacturer;
                f.Model = Data.Model;                
                // Skip status and price, as localizations are different.
                f.Weight = Data.Weight;
                f.WeightDistribution = Data.WeightDistribution;
                f.Width = Data.Width;
                f.Height = Data.Height;
                f.Length = Data.Length;                
                // Skip perf data, as localizations are different and those can be inferred from carp.

            }
        }
    }
}