#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Serializers.Audio.Bnk;

namespace TheXDS.Vivianne.Serializers;

public abstract class BnkSerializerTests(string streamName, BnkFile referenceFile) : SerializerTestsBase<BnkSerializer, BnkFile>(streamName, referenceFile)
{
    protected override void TestParsedFile(BnkFile expected, BnkFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.FileVersion, Is.EqualTo(expected.FileVersion));
            Assert.That(actual.Streams, Has.Count.EqualTo(expected.Streams.Count));
        }
        foreach ((BnkStream? expectedAudio, BnkStream? actualAudio) in expected.Streams.Zip(actual.Streams))
        {
            if (expectedAudio is not null && actualAudio is not null)
            { 
                VerifyBnkStream(expectedAudio, actualAudio);
            }
            else
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(expectedAudio, Is.Null);
                    Assert.That(actualAudio, Is.Null);
                }
            }
        }
    }

    private static void VerifyBnkStream(BnkStream expected, BnkStream actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Channels, Is.EqualTo(expected.Channels));
            Assert.That(actual.Compression, Is.EqualTo(expected.Compression));
            Assert.That(actual.SampleRate, Is.EqualTo(expected.SampleRate));
            Assert.That(actual.BytesPerSample, Is.EqualTo(expected.BytesPerSample));
            Assert.That(actual.SampleData, Is.EquivalentTo(expected.SampleData));
            Assert.That(actual.PostAudioStreamData, Is.EquivalentTo(expected.PostAudioStreamData));
            Assert.That(actual.IsAltStream, Is.EqualTo(expected.IsAltStream));
            Assert.That(actual.CalculatedDuration, Is.EqualTo(expected.CalculatedDuration));
            Assert.That(actual.TotalSamples, Is.EqualTo(expected.TotalSamples));
        }
        if (expected.AltStream is not null)
        {
            Assert.That(actual.AltStream, Is.Not.Null);
            VerifyBnkStream(expected.AltStream, actual.AltStream);
        }
    }
}
