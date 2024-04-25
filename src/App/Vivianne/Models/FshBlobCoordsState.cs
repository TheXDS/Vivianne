using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents the state of the <see cref="FshBlobCoordsEditorViewModel"/>.
/// </summary>
/// <param name="blob">Blob to be edited.</param>
public class FshBlobCoordsState(FshBlob blob) : EditorViewModelStateBase
{
    private int _XRotation = blob.XRotation;
    private int _YRotation = blob.YRotation;
    private int _XPosition = blob.XPosition;
    private int _YPosition = blob.YPosition;

    /// <summary>
    /// Gets a reference to the Blob being edited.
    /// </summary>
    public FshBlob Blob { get; } = blob;

    /// <summary>
    /// Gets or sets the X coord used for rotation operations.
    /// </summary>
    public int XRotation
    {
        get => _XRotation;
        set => Change(ref _XRotation, value);
    }

    /// <summary>
    /// Gets or sets the Y coord used for rotation operations.
    /// </summary>
    public int YRotation
    {
        get => _YRotation;
        set => Change(ref _YRotation, value);
    }

    /// <summary>
    /// Gets or sets the X coord used for traslation operations.
    /// </summary>
    public int XPosition
    {
        get => _XPosition;
        set => Change(ref _XPosition, value);
    }

    /// <summary>
    /// Gets or sets the Y coord used for traslation operations.
    /// </summary>
    public int YPosition
    {
        get => _YPosition;
        set => Change(ref _YPosition, value);
    }
}

public class FceColorTableEditorState(FceFile fce) : EditorViewModelStateBase
{
    public FceFile Fce { get; } = fce;

    public ICollection<FceColorItem> Colors { get; } = CreateFromFce(fce);

    private static ICollection<FceColorItem> CreateFromFce(FceFile fce)
    {
        var primary = fce.Header.PrimaryColorTable
            .Take(fce.Header.PrimaryColors)
            .Select(MutableFceColor.From);
        var secondary = fce.Header.SecondaryColorTable
            .Take(fce.Header.SecondaryColors)
            .Select(MutableFceColor.From);
        var joint = primary
            .Zip(secondary)
            .Select(p => new FceColorItem()
            { 
                PrimaryColor = p.First,
                SecondaryColor = p.Second
            });
        return new ObservableCollection<FceColorItem>(joint);
    }
}

public class FceColorItem : NotifyPropertyChanged
{
    private MutableFceColor _PrimaryColor;
    private MutableFceColor _SecondaryColor;

    public MutableFceColor PrimaryColor
    {
        get => _PrimaryColor;
        set => Change(ref _PrimaryColor, value);
    }

    public MutableFceColor SecondaryColor
    {
        get => _SecondaryColor;
        set => Change(ref _SecondaryColor, value);
    }
}

public class MutableFceColor : NotifyPropertyChanged
{
    private byte _Hue;
    private byte _Saturation;
    private byte _Brightness;
    private byte _Alpha;

    public MutableFceColor()
    {
        RegisterPropertyChangeTrigger(nameof(Preview), nameof(Hue), nameof(Saturation), nameof(Brightness), nameof(Alpha));
    }

    public byte Hue
    {
        get => _Hue;
        set => Change(ref _Hue, value);
    }

    public byte Saturation
    {
        get => _Saturation;
        set => Change(ref _Saturation, value);
    }

    public byte Brightness
    {
        get => _Brightness;
        set => Change(ref _Brightness, value);
    }

    public byte Alpha
    {
        get => _Alpha;
        set => Change(ref _Alpha, value);
    }

    public FceColor Preview => ToColor();

    public FceColor ToColor() => new(Hue, Saturation, Brightness, Alpha);

    public static MutableFceColor From(FceColor color) => new()
    {
        Hue = (byte)color.Hue,
        Saturation = (byte)color.Saturation,
        Brightness = (byte)color.Brightness,
        Alpha = (byte)color.Alpha
    };
}