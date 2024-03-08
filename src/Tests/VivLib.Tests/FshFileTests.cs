#pragma warning disable CS1591

using TheXDS.Vivianne.Containers;

namespace TheXDS.Vivianne.Tests;

public class FshFileTests
{
    private static MemoryStream CreateTestFsh()
    {
        return new([
           0x53, 0x48, 0x50, 0x49, // "SHPI"
           0x31, 0x00, 0x00, 0x00, // File length
           0x01, 0x00, 0x00, 0x00, // Entries
           0x47, 0x49, 0x4D, 0x58, // "GIMX"
           0x54, 0x45, 0x53, 0x54, // 4-byte name ("TEST")
           0x18, 0x00, 0x00, 0x00, // Entry offset
           ..CreateTestFshItem()]);
    }

    private static byte[] CreateTestFshItem()
    {
        return [
            0x78,               // Magic
            0x00, 0x00, 0x00,   // 24-bit blob size
            0x03, 0x00,         // 3px width
            0x03, 0x00,         // 3px height
            0x02, 0x00,         // x rotation
            0x02, 0x00,         // y rotation
            0x40, 0x00,         // x position
            0x40, 0x00,         // y position
            0x80,0x80,0x80,     // \
            0x80,0x80,0x80,     //  > Pixel data
            0x80,0x80,0x80      // /
        ];
    }

    [Test]
    public void Class_can_read_fsh()
    {
        using var ms = CreateTestFsh();
        var fsh = FshFile.ReadFrom(ms);

        Assert.Multiple(() =>
        {
            Assert.That(fsh.Images, Has.Count.EqualTo(1));
            Assert.That(fsh.Images.ContainsKey("TEST"));
            var img = fsh.Images["TEST"];
            Assert.That(img.Width, Is.EqualTo(3));
            Assert.That(img.Height, Is.EqualTo(3));
            Assert.That(img.XRotation, Is.EqualTo(2));
            Assert.That(img.YRotation, Is.EqualTo(2));
            Assert.That(img.XPosition, Is.EqualTo(0x40));
            Assert.That(img.YPosition, Is.EqualTo(0x40));
            Assert.That(img.PixelData, Is.EquivalentTo(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }));
        });
    }
}
