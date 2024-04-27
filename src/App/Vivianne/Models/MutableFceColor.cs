using TheXDS.MCART.Types.Base;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Mutable class equivalent to <see cref="FceColor"/>.
/// </summary>
public class MutableFceColor : NotifyPropertyChanged
{
    private byte _Hue;
    private byte _Saturation;
    private byte _Brightness;
    private byte _Alpha;

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableFceColor"/> class.
    /// </summary>
    public MutableFceColor()
    {
        RegisterPropertyChangeTrigger(nameof(Preview), nameof(Hue), nameof(Saturation), nameof(Brightness), nameof(Alpha));
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
    /// Gets or sets the alpha level for this color.
    /// </summary>
    public byte Alpha
    {
        get => _Alpha;
        set => Change(ref _Alpha, value);
    }

    /// <summary>
    /// Converts this color into an <see cref="FceColor"/> that allows for
    /// preview operations to be executed by notification of property change.
    /// </summary>
    public FceColor Preview => ToColor();

    /// <summary>
    /// Converts this instance into an <see cref="FceColor"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="FceColor"/> equivalent to this instance.
    /// </returns>
    public FceColor ToColor() => new(Hue, Saturation, Brightness, Alpha);

    /// <summary>
    /// Converts an <see cref="FceColor"/> into a <see cref="MutableFceColor"/>
    /// instance.
    /// </summary>
    /// <param name="color">Color to be converted.</param>
    /// <returns>
    /// A new <see cref="MutableFceColor"/> instance equivalent to the original
    /// <see cref="FceColor"/>.
    /// </returns>
    public static MutableFceColor From(FceColor color) => new()
    {
        Hue = (byte)color.Hue,
        Saturation = (byte)color.Saturation,
        Brightness = (byte)color.Brightness,
        Alpha = (byte)color.Alpha
    };

    /// <summary>
    /// Implicitly converts a <see cref="FceColor"/> into a
    /// <see cref="MutableFceColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator MutableFceColor(FceColor color) => From(color);

    /// <summary>
    /// Implicitly converts a <see cref="MutableFceColor"/> into a
    /// <see cref="FceColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator FceColor(MutableFceColor color) => color.ToColor();
}