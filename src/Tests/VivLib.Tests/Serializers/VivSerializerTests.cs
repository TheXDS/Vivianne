#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class VivSerializerTests() : SerializerTestsBase<VivSerializer, VivFile>("test.viv", GetDefaultFile())
{
    private static VivFile GetDefaultFile() => new()
    {
        Directory = { { "test.txt", "TEST"u8.ToArray() } }
    };

    protected override void TestParsedFile(VivFile expected, VivFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ContainsKey("test.txt"));
            Assert.That(actual["test.txt"], Is.EquivalentTo("TEST"u8.ToArray()));
        }
    }
}
