namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Formats <see cref="byte"/> numbers as hex values.
/// </summary>
public class HexByteNumberFormatter : HexNumberFormatter<byte>
{
    /// <inheritdoc/>
    protected override byte From(double? value) => (byte)(value ?? 0);

    /// <inheritdoc/>
    protected override byte From(int? value) => (byte)(value ?? 0);

    /// <inheritdoc/>
    protected override byte From(uint? value) => (byte)(value ?? 0);

    /// <inheritdoc/>
    protected override byte[] GetBytes(byte value) => [value];

    /// <inheritdoc/>
    protected override double? ToDouble(byte? value) => value;

    /// <inheritdoc/>
    protected override int? ToInt(byte? value) => value;

    /// <inheritdoc/>
    protected override uint? ToUInt(byte? value) => value;
}