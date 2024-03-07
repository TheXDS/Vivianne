using TheXDS.MCART.Attributes;

namespace TheXDS.Vivianne.Attributes
{
    public class OffsetTableIndexAttribute(int offset) : Attribute, IValueAttribute<int>
    {
        public int Value { get; } = offset;
    }
}
