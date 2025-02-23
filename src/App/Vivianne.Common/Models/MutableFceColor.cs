using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Mutable class equivalent to <see cref="HsbColor"/>.
/// </summary>
public class MutableFceColor : NotifyPropertyChanged
{
    private byte _Hue;
    private byte _Saturation;
    private byte _Brightness;
    private byte _Alpha;

    /// <inheritdoc/>
    protected override void OnInitialize(IPropertyBroadcastSetup broadcastSetup)
    {
        broadcastSetup.RegisterPropertyChangeTrigger(() => Preview, () => Hue, () => Saturation, () => Brightness, () => Alpha);
    }

    /// <summary>
    /// Gets or sets the color hue.
    /// </summary>
    public byte Hue
    {
        get => _Hue;
        set => Change(ref _Hue, value);
    }

    /// <summary>
    /// Gets or sets the color saturation.
    /// </summary>
    public byte Saturation
    {
        get => _Saturation;
        set => Change(ref _Saturation, value);
    }

    /// <summary>
    /// Gets or sets the color brightness.
    /// </summary>
    public byte Brightness
    {
        get => _Brightness;
        set => Change(ref _Brightness, value);
    }

    /// <summary>
    /// Gets or sets the _alpha level for this color.
    /// </summary>
    public byte Alpha
    {
        get => _Alpha;
        set => Change(ref _Alpha, value);
    }

    /// <summary>
    /// Converts this color into an <see cref="HsbColor"/> that allows for
    /// preview operations to be executed by notification of property change.
    /// </summary>
    public HsbColor Preview => ToColor();

    /// <summary>
    /// Converts this instance into an <see cref="HsbColor"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="HsbColor"/> equivalent to this instance.
    /// </returns>
    public HsbColor ToColor() => new(Hue, Saturation, Brightness, Alpha);

    /// <summary>
    /// Converts an <see cref="HsbColor"/> into a <see cref="MutableFceColor"/>
    /// instance.
    /// </summary>
    /// <param name="color">Color to be converted.</param>
    /// <returns>
    /// A new <see cref="MutableFceColor"/> instance equivalent to the original
    /// <see cref="HsbColor"/>.
    /// </returns>
    public static MutableFceColor From(HsbColor color) => new()
    {
        Hue = (byte)color.Hue,
        Saturation = (byte)color.Saturation,
        Brightness = (byte)color.Brightness,
        Alpha = (byte)color.Alpha
    };

    /// <summary>
    /// Implicitly converts a <see cref="HsbColor"/> into a
    /// <see cref="MutableFceColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator MutableFceColor(HsbColor color) => From(color);

    /// <summary>
    /// Implicitly converts a <see cref="MutableFceColor"/> into a
    /// <see cref="HsbColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator HsbColor(MutableFceColor color) => color.ToColor();
}