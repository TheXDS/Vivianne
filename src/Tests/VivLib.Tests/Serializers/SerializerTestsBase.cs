#pragma warning disable CS1591
namespace TheXDS.Vivianne.Serializers;

public abstract class SerializerTestsBase<TSerializer, TFile>(string streamName, TFile referenceFile) where TSerializer : ISerializer<TFile>, new()
{
    private readonly string streamName = streamName;
    private readonly TFile referenceFile = referenceFile;
    private TSerializer serializer;
    private Stream testStream;
    private byte[] testFileContents;

    [SetUp]
    public void Setup()
    {
        serializer = new TSerializer();
        testStream = typeof(SerializerTestsBase<,>).Assembly!.GetManifestResourceStream(@$"TheXDS.Vivianne.Resources.Files.{streamName}")!;
        using var ms = new MemoryStream();
        testStream.CopyTo(ms);
        testStream.Seek(0, SeekOrigin.Begin);
        testFileContents = ms.ToArray();
    }

    [TearDown]
    public void TearDown()
    {
        testStream.Dispose();
    }

    [Test]
    public void Serializer_can_parse_file_from_stream()
    {
        var file = serializer.Deserialize(testStream);
        Assert.That(file, Is.InstanceOf<TFile>());
    }

    [Test]
    public async Task Serializer_can_parse_file_asyncronously_from_stream()
    {
        var file = await serializer.DeserializeAsync(testStream);
        Assert.That(file, Is.InstanceOf<TFile>());
    }

    [Test]
    public void Serializer_can_parse_file_from_byte_array()
    {
        var file = serializer.Deserialize(testFileContents);
        Assert.That(file, Is.InstanceOf<TFile>());
    }

    [Test]
    public async Task Serializer_can_parse_file_asyncronously_from_byte_array()
    {
        var file = await serializer.DeserializeAsync(testFileContents);
        Assert.That(file, Is.InstanceOf<TFile>());
    }

    [Test]
    public void Serializer_can_write_new_file_to_stream()
    {
        using var ms = new MemoryStream();
        serializer.SerializeTo(referenceFile, ms);
        Assert.That(ms.ToArray(), Is.EquivalentTo(testFileContents));
    }

    [Test]
    public async Task Serializer_can_write_new_file_to_stream_asyncronously()
    {
        using var ms = new MemoryStream();
        await serializer.SerializeToAsync(referenceFile, ms);
        Assert.That(ms.ToArray(), Is.EquivalentTo(testFileContents));
    }

    [Test]
    public void Serializer_can_write_new_file_to_byte_array()
    {
        var result = serializer.Serialize(referenceFile);
        Assert.That(result, Is.EquivalentTo(testFileContents));
    }

    [Test]
    public async Task Serializer_can_write_new_file_to_byte_array_asyncronously()
    {
        var result = await serializer.SerializeAsync(referenceFile);
        Assert.That(result, Is.EquivalentTo(testFileContents));
    }

    protected abstract void TestParsedFile(TFile expected, TFile actual);
}
