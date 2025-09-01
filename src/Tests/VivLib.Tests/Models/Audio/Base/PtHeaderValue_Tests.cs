#pragma warning disable CS1591

namespace TheXDS.Vivianne.Models.Audio.Base;

[TestFixture]
public class PtHeaderValue_Tests
{
    [TestCase(1, 1)]
    [TestCase(255, 1)]
    [TestCase(256, 2)]
    [TestCase(65535, 2)]
    [TestCase(65536, 4)]
    [TestCase(int.MaxValue, 4)]
    public void Implicit_conversion_gets_right_pack_length(int value, byte expectedLength)
    {
        PtHeaderValue x = value;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(x.Value, Is.EqualTo(value));
            Assert.That(x.Length, Is.EqualTo(expectedLength));
        }
    }
}
