using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Attributes;

/// <summary>
/// On models that have properties loaded from a static offset table, indicates
/// the index to load the offset for the property property from.
/// </summary>
/// <param name="offset">Index on the offset table for the property.</param>
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
public class OffsetTableIndexAttribute(int offset) : Attribute, IValueAttribute<int>
{
    /// <summary>
    /// Gets the index for the property on the offset table.
    /// </summary>
    public int Value { get; } = offset;
}
