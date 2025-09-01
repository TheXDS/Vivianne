#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Serializers.Fsh;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class FshSerializerTests() : SerializerTestsBase<FshSerializer, FshFile>("test.fsh", GetDefaultFile())
{
    private static FshFile GetDefaultFile()
    {
        return new FshFile()
        {
            Entries = { {"TEST", new FshBlob()
            {
                Magic = FshBlobFormat.Indexed8,
                Width = 3,
                Height = 3,
                XRotation = 2,
                YRotation = 2,
                XPosition = 0x40,
                YPosition = 0x40,
                PixelData = [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80],
                Footer = []
            }}}
        };
    }

    protected override void TestParsedFile(FshFile expected, FshFile actual)
    {
        Assert.That(actual.Entries, Has.Count.EqualTo(expected.Entries.Count));
        Assert.That(actual.Entries.ContainsKey("TEST"));
        var expectedBlob = expected.Entries["TEST"];
        var actualBlob = actual.Entries["TEST"];
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actualBlob.Width, Is.EqualTo(expectedBlob.Width));
            Assert.That(actualBlob.Height, Is.EqualTo(expectedBlob.Height));
            Assert.That(actualBlob.XRotation, Is.EqualTo(expectedBlob.XRotation));
            Assert.That(actualBlob.YRotation, Is.EqualTo(expectedBlob.YRotation));
            Assert.That(actualBlob.XPosition, Is.EqualTo(expectedBlob.XPosition));
            Assert.That(actualBlob.YPosition, Is.EqualTo(expectedBlob.YPosition));
            Assert.That(actualBlob.PixelData, Is.EquivalentTo(expectedBlob.PixelData));
        }
    }
}