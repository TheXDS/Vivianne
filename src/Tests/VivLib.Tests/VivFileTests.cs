#pragma warning disable CS1591


namespace TheXDS.Vivianne.Tests;

//public class VivFileTests
//{
//    private static MemoryStream CreateTestViv()
//    {
//        return new([
//           0x42, 0x49, 0x47, 0x46, // "BIGF"
//           0x00, 0x00, 0x00, 0x27, // File length
//           0x00, 0x00, 0x00, 0x01, // Entries
//           0x00, 0x00, 0x00, 0x1D, // Data pool offset
//           // Entries:
//           0x00, 0x00, 0x00, 0x1D, // Entry offset
//           0x00, 0x00, 0x00, 0x0A, // Blob length
//           0x54, 0x45, 0x53, 0x54, // Null-terminated name ("TEST")
//           0x00,
//           0x30, 0x31, 0x32, 0x33, // Blob contents ("0123456789")
//           0x34, 0x35, 0x36, 0x37,
//           0x38, 0x39
//        ]);
//    }

//    [Test]
//    public void Class_can_read_viv()
//    {
//        using var ms = CreateTestViv();
//        var viv = VivFile.ReadFrom(ms);
        
//        Assert.Multiple(() =>
//        {
//            Assert.That(viv.Directory, Has.Count.EqualTo(1));
//            Assert.That(viv.Directory.ContainsKey("TEST"));
//            Assert.That(viv.Directory["TEST"], Is.EquivalentTo("0123456789"u8.ToArray()));
//        });
//    }

//    [Test]
//    public void Class_can_update_viv_contents()
//    {
//        using var ms = CreateTestViv();
//        var viv = VivFile.ReadFrom(ms);
//        viv.Directory["TEST"] = "987654321"u8.ToArray();

//        Assert.Multiple(() =>
//        {
//            Assert.That(viv.Directory, Has.Count.EqualTo(1));
//            Assert.That(viv.Directory.ContainsKey("TEST"));
//            Assert.That(viv.Directory["TEST"], Is.EquivalentTo("987654321"u8.ToArray()));
//        });
//    }

//    [Test]
//    public void Writing_viv_persists_changes()
//    {
//        using var ms = CreateTestViv();
//        var viv = VivFile.ReadFrom(ms);
//        viv.Directory["TEST"] = "987654321"u8.ToArray();

//        using var ms2 = new MemoryStream();
//        viv.WriteTo(ms2);

//        Assert.That(ms.ToArray(), Is.Not.EquivalentTo(ms2.ToArray()));
//    }

//    [Test]
//    public void Viv_can_be_written_to_stream()
//    {
//        using var ms = CreateTestViv();
//        var viv = VivFile.ReadFrom(ms);

//        using var ms2 = new MemoryStream();
//        viv.WriteTo(ms2);

//        Assert.That(ms.ToArray(), Is.EquivalentTo(ms2.ToArray()));
//    }

//    [Test]
//    public void Read_invalid_header_throws_exception()
//    {
//        using var ms = new MemoryStream(new byte[80]);
//        Assert.That(()=> _ = VivFile.ReadFrom(ms), Throws.InstanceOf<InvalidDataException>());
//    }
    
//    [Test]
//    public void Class_can_write_viv()
//    {
//        using var ms = new MemoryStream();
//        var viv = new VivFile();
//        viv.Directory.Add("TEST", "0123456789"u8.ToArray());
//        viv.WriteTo(ms);
        
//        Assert.That(ms.ToArray(), Is.EquivalentTo(CreateTestViv().ToArray()));
//    }
//}