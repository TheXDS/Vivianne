namespace TheXDS.Vivianne.Models.Audio.Base;

/// <summary>
/// Represents a PT header value found on audio files.
/// </summary>
/// <param name="Length">
/// Length of the value in bytes. Valid ranges are 1-4.
/// </param>
/// <param name="Value">Actual ingeter value.</param>
public record struct PtHeaderValue(byte Length, int Value)
{
    /// <summary>
    /// Implicitly converts a value of type <see cref="PtHeaderValue"/> to a
    /// value of type <see cref="int"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator int(PtHeaderValue value) => value.Value;

    /// <summary>
    /// Implicitly converts a value of type <see cref="bool"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(bool value)
    {
        return new(1, value ? 1 : 0);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="byte"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(byte value)
    {
        return new(1, value);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="sbyte"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(sbyte value)
    {
        return new(1, value);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="short"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(short value)
    {
        return new(CalculatePackSize((uint)value), value);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="ushort"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(ushort value)
    {
        return new(CalculatePackSize(value), value);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="int"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(int value)
    {
        return new(CalculatePackSize((uint)value), value);
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="uint"/> to a value of
    /// type <see cref="PtHeaderValue"/>.
    /// </summary>
    /// <param name="value">Value to be converted.</param>
    public static implicit operator PtHeaderValue(uint value)
    {
        return new(CalculatePackSize(value), (int)value);
    }

    private static byte CalculatePackSize(uint value) => value switch
    {
        > ushort.MaxValue => 4,
        > byte.MaxValue => 2,
        _ => 1,
    };
}
