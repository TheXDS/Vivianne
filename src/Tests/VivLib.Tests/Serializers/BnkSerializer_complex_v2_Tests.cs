#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class BnkSerializer_complex_v2_Tests() : BnkSerializerTests("test_complex_v2.bnk", GetDefaultFile())
{
    private static BnkFile GetDefaultFile() => new()
    {
        FileVersion = 2,
        Streams =
        {
            new BnkStream()
            {
                Properties =
                {
                    { 0x06, 0x64 },
                    { 0x0a, 0x0e },
                    { 0x0e, 0x6e },
                    { 0x13, 0x64 },
                },
                Channels = 1,
                Compression = CompressionMethod.None,
                SampleRate = 22050,
                BytesPerSample = 2,
                SampleData = [
                    0x00, 0x00, 0xff, 0x3f, 0xff, 0x7f,
                    0xff, 0x3f, 0x00, 0x00, 0x01, 0xc0,
                    0x00, 0x80, 0x01, 0xc0, 0x00, 0x00
                ],
                PostAudioStreamData = [.. Enumerable.Range(0,10).Select(p => (byte)p)],
                AltStream = new()
                {
                    IsAltStream = true,
                    Properties =
                    {
                        { 0x06, 0x63 },
                        { 0x07, 0x3f },
                        { 0x0a, 0x0e },
                        { 0x0e, 0x6e },
                        { 0x10, 0x30 },
                        { 0x13, 0x64 },
                    },
                    Channels = 1,
                    Compression = CompressionMethod.None,
                    SampleRate = 22050,
                    BytesPerSample = 2,
                    SampleData = [
                        0x00, 0x00, 0x01, 0xc0, 0x00, 0x80,
                        0x01, 0xc0, 0x00, 0x00, 0xff, 0x3f,
                        0xff, 0x7f, 0xff, 0x3f, 0x00, 0x00,
                    ],
                }
            },
            new BnkStream()
            {
                Channels = 2,
                Compression = CompressionMethod.None,
                SampleRate = 22050,
                BytesPerSample = 2,
                SampleData = [
                    0x00, 0x00, 0x00, 0x00,
                    0xff, 0x3f, 0x01, 0xc0,
                    0xff, 0x7f, 0x00, 0x80,
                    0xff, 0x3f, 0x01, 0xc0,
                    0x00, 0x00, 0x00, 0x00,
                    0x01, 0xc0, 0xff, 0x3f,
                    0x00, 0x80, 0xff, 0x7f,
                    0x01, 0xc0, 0xff, 0x3f,
                    0x00, 0x00, 0x00, 0x00
                ],
                PostAudioStreamData = [.. Enumerable.Range(0, 10).Select(p => (byte)p)],
                AltStream = new()
                {
                    IsAltStream = true,
                    Channels = 2,
                    Compression = CompressionMethod.None,
                    SampleRate = 22050,
                    BytesPerSample = 2,
                    SampleData = [
                        0x00, 0x00, 0x00, 0x00,
                        0x01, 0xc0, 0xff, 0x3f,
                        0x00, 0x80, 0xff, 0x7f,
                        0x01, 0xc0, 0xff, 0x3f,
                        0x00, 0x00, 0x00, 0x00,
                        0xff, 0x3f, 0x01, 0xc0,
                        0xff, 0x7f, 0x00, 0x80,
                        0xff, 0x3f, 0x01, 0xc0,
                        0x00, 0x00, 0x00, 0x00,
                    ],
                }
            },
            new BnkStream()
            {
                Channels = 1,
                Compression = CompressionMethod.None,
                SampleRate = 22050,
                BytesPerSample = 2,
                SampleData = [
                    0x00, 0x00, 0xff, 0x3f, 0xff, 0x7f,
                    0xff, 0x3f, 0x00, 0x00, 0x01, 0xc0,
                    0x00, 0x80, 0x01, 0xc0, 0x00, 0x00
                ],
                PostAudioStreamData = [.. Enumerable.Range(0,10).Select(p => (byte)p)],
            },
            new BnkStream()
            {
                Channels = 2,
                Compression = CompressionMethod.None,
                SampleRate = 22050,
                BytesPerSample = 2,
                SampleData = [
                    0x00, 0x00, 0x00, 0x00,
                    0xff, 0x3f, 0x01, 0xc0,
                    0xff, 0x7f, 0x00, 0x80,
                    0xff, 0x3f, 0x01, 0xc0,
                    0x00, 0x00, 0x00, 0x00,
                    0x01, 0xc0, 0xff, 0x3f,
                    0x00, 0x80, 0xff, 0x7f,
                    0x01, 0xc0, 0xff, 0x3f,
                    0x00, 0x00, 0x00, 0x00
                ],
                PostAudioStreamData = [.. Enumerable.Range(0, 10).Select(p => (byte)p)],
            }
        }
    };
}
