namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Formats <see cref="int"/> numbers as hex values.
/// </summary>
public class HexInt32NumberFormatter : HexNumberFormatter<int>
{
    /// <inheritdoc/>
    protected override int From(double? value) => (int)(value ?? 0);

    /// <inheritdoc/>
    protected override int From(int? value) => (int)(value ?? 0);

    /// <inheritdoc/>
    protected override int From(uint? value) => (int)(value ?? 0);

    /// <inheritdoc/>
    protected override byte[] GetBytes(int value) => BitConverter.GetBytes(value);

    /// <inheritdoc/>
    protected override double? ToDouble(int? value) => value;

    /// <inheritdoc/>
    protected override int? ToInt(int? value) => value;

    /// <inheritdoc/>
    protected override uint? ToUInt(int? value) => (uint?)value;
}
