using System.Globalization;
using System.Numerics;
using Wpf.Ui.Controls;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Base class for formatters that display numbers as hex values.
/// </summary>
public abstract class HexNumberFormatter<T> : INumberFormatter, INumberParser
    where T : unmanaged, INumber<T>
{
    /// <summary>
    /// Gets a byte array from the given value.
    /// </summary>
    /// <param name="value">Value represented by the byte array.</param>
    /// <returns>
    /// A byte array that represents the raw value of the number.
    /// </returns>
    protected abstract byte[] GetBytes(T value);

    /// <summary>
    /// Gets a value of type <typeparamref name="T"/> from the given double,
    /// int or uint value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A value of type <typeparamref name="T"/> that is equivalent to to the
    /// input value.
    /// </returns>
    protected abstract T From(double? value);

    /// <summary>
    /// Gets a value of type <typeparamref name="T"/> from the given double,
    /// int or uint value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A value of type <typeparamref name="T"/> that is equivalent to to the
    /// input value.
    /// </returns>
    protected abstract T From(int? value);

    /// <summary>
    /// Gets a value of type <typeparamref name="T"/> from the given double,
    /// int or uint value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A value of type <typeparamref name="T"/> that is equivalent to to the
    /// input value.
    /// </returns>
    protected abstract T From(uint? value);

    /// <summary>
    /// Converts the value to a <see cref="double"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A <see cref="double"/> value that is equivalent to to the input value.
    /// </returns>
    protected abstract double? ToDouble(T? value);

    /// <summary>
    /// Converts the value to a <see cref="int"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A <see cref="int"/> value that is equivalent to to the input value.
    /// </returns>
    protected abstract int? ToInt(T? value);

    /// <summary>
    /// Converts the value to a <see cref="uint"/>.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>
    /// A <see cref="uint"/> value that is equivalent to to the input value.
    /// </returns>
    protected abstract uint? ToUInt(T? value);

    private string FormatHex(T value)
    {
        return $"0x{string.Join(string.Empty, GetBytes(value).Reverse().Select(p => p.ToString("X2")))}";
    }

    string INumberFormatter.FormatDouble(double? value) => FormatHex(From(value));

    string INumberFormatter.FormatInt(int? value) => FormatHex(From(value));

    string INumberFormatter.FormatUInt(uint? value) => FormatHex(From(value));

    double? INumberParser.ParseDouble(string? value)
    {
        return T.TryParse(value, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result) ? ToDouble(result) : null;
    }

    int? INumberParser.ParseInt(string? value)
    {
        return T.TryParse(value, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result) ? ToInt(result) : null;
    }

    uint? INumberParser.ParseUInt(string? value)
    {
        return T.TryParse(value, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result) ? ToUInt(result) : null;
    }
}
