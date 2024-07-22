using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Attributes;

/// <summary>
/// Indicates the endianness of a single field in a struct that is read using
/// the
/// <see cref="Extensions.BinaryReaderExtensions.MarshalReadStructExt{T}(BinaryReader)"/>
/// method.
/// </summary>
/// <param name="endianness">Value that indicates the field's endianness.</param>
[AttributeUsage(AttributeTargets.Field)]
public class EndiannessAttribute(Endianness endianness) : Attribute, IValueAttribute<Endianness>
{
    /// <inheritdoc/>
    public Endianness Value {get;} = endianness;
}