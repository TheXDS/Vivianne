using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Fce.Common;
using H3 = TheXDS.Vivianne.Models.Fce.Nfs3.HsbColor;
using H4 = TheXDS.Vivianne.Models.Fce.Nfs4.HsbColor;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Mutable class equivalent to <see cref="IHsbColor"/>.
/// </summary>
public class MutableFceColor : NotifyPropertyChanged, IHsbColor
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
    /// Converts this color into an <see cref="IHsbColor"/> that allows for
    /// preview operations to be executed by notification of property change.
    /// </summary>
    public IHsbColor Preview => this;

    /// <summary>
    /// Converts this instance into an <see cref="IHsbColor"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="H3"/> equivalent to this instance.
    /// </returns>
    public H3 ToColor3() => new(Hue, Saturation, Brightness, Alpha);

    /// <summary>
    /// Converts this instance into an <see cref="IHsbColor"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="H4"/> equivalent to this instance.
    /// </returns>
    public H4 ToColor4() => new(Hue, Saturation, Brightness, Alpha);

    /// <summary>
    /// Converts this instance into an <see cref="IHsbColor"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="IHsbColor"/> equivalent to this instance.
    /// </returns>
    public IHsbColor ToColor() => new H4(Hue, Saturation, Brightness, Alpha);

    /// <summary>
    /// Converts an <see cref="IHsbColor"/> into a <see cref="MutableFceColor"/>
    /// instance.
    /// </summary>
    /// <param name="color">Color to be converted.</param>
    /// <returns>
    /// A new <see cref="MutableFceColor"/> instance equivalent to the original
    /// <see cref="IHsbColor"/>.
    /// </returns>
    public static MutableFceColor From(IHsbColor color) => new()
    {
        Hue = color.Hue,
        Saturation = color.Saturation,
        Brightness = color.Brightness,
        Alpha = color.Alpha
    };

    /// <inheritdoc/>
    public (int R, int G, int B, int A) ToRgba()
    {
        var (R, G, B) = ColorConversion.ToRgb(Hue, Saturation, Brightness);
        return (R, G, B, Alpha);
    }

    /// <summary>
    /// Implicitly converts a <see cref="H3"/> into a
    /// <see cref="MutableFceColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator MutableFceColor(H3 color) => From(color);

    /// <summary>
    /// Implicitly converts a <see cref="H4"/> into a
    /// <see cref="MutableFceColor"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator MutableFceColor(H4 color) => From(color);

    /// <summary>
    /// Implicitly converts a <see cref="MutableFceColor"/> into a
    /// <see cref="H4"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator H3(MutableFceColor color) => color.ToColor3();

    /// <summary>
    /// Implicitly converts a <see cref="MutableFceColor"/> into a
    /// <see cref="H4"/>.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    public static implicit operator H4(MutableFceColor color) => color.ToColor4();
}