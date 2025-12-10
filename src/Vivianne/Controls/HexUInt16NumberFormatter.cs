namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Formats <see cref="ushort"/> numbers as hex values.
/// </summary>
public class HexUInt16NumberFormatter : HexNumberFormatter<ushort>
{
    /// <inheritdoc/>
    protected override ushort From(double? value) => (ushort)(value ?? 0);

    /// <inheritdoc/>
    protected override ushort From(int? value) => (ushort)(value ?? 0);

    /// <inheritdoc/>
    protected override ushort From(uint? value) => (ushort)(value ?? 0);

    /// <inheritdoc/>
    protected override byte[] GetBytes(ushort value) => BitConverter.GetBytes(value);

    /// <inheritdoc/>
    protected override double? ToDouble(ushort? value) => value;

    /// <inheritdoc/>
    protected override int? ToInt(ushort? value) => value;

    /// <inheritdoc/>
    protected override uint? ToUInt(ushort? value) => value;
}
