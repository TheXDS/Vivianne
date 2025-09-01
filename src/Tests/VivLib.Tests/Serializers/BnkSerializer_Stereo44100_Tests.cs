#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class BnkSerializer_Stereo44100_Tests() : BnkSerializerTests("teststereo44100.bnk", GetDefaultFile())
{
    private static BnkFile GetDefaultFile() => new()
    {
        FileVersion = 2,
        Streams =
        {
            new BnkStream()
            {
                Channels = 2,
                Compression = CompressionMethod.None,
                SampleRate = 44100,
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
                ]
            }
        }
    };
}