using System.Diagnostics;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Serializers.Audio;

internal static class PtHeaderSerializerHelper
{
    public static PtHeader ReadPtHeader(BinaryReader br)
    {
        var result = new PtHeader();
        while (true)
        {
            var field = (PtHeaderField)br.ReadByte();

            switch (field)
            {
                case PtHeaderField.AlternateStream:
                    result.AltStream = ReadPtHeader(br);
                    goto case PtHeaderField.EndOfHeader;
                case PtHeaderField.Unk_0xfc:
#if DEBUG
                    Debug.Assert(false, "The 0xFC PT header command is not yet implemented. Please verify that we're not overriding data. If you continue, the program will signal the end of the PT header and ignore whatever data follows after 0xFC.");
                    if (Debugger.IsAttached || Debugger.Launch()) Debugger.Break();
                    goto case PtHeaderField.EndOfHeader;
#endif
                case PtHeaderField.EndOfHeader: goto EndParsing;
                case PtHeaderField.AudioHeader:
                    ReadAudioHeader(result, br);
                    break;
                default:
                    var length = br.ReadByte();
                    result[field] = new PtHeaderValue
                    {
                        Length = length,
                        Value = ReadBytes(br, length)
                    };
                    break;
            }
        }
    EndParsing:
        return result;
    }

    public static void WritePtHeader(BinaryWriter bw, PtHeader header)
    {
        WritePtHeaderValues(bw, header);
        bw.Write((byte)PtHeaderField.AudioHeader);
        WriteAudioHeaderValues(bw, header);
        if (header.AltStream is not null)
        {
            bw.Write((byte)PtHeaderField.AlternateStream);
            WritePtHeader(bw, header.AltStream);
        }
    }

    private static void ReadAudioHeader(PtHeader result, BinaryReader br)
    {
        while (true)
        {
            var field = (PtAudioHeaderField)br.ReadByte();
            var length = br.ReadByte();
            var value = ReadBytes(br, length);
            result[field] = new PtHeaderValue
            {
                Length = length,
                Value = value
            };
            /* For some inexplicable reason, end of audio header declares 4
             * bytes of data, and its value is always zero. Hence, the check
             * to get out of the parser loop lives here instead of up there.
             */
            if (field == PtAudioHeaderField.EndOfHeader) break;
        }
    }

    private static void WritePtHeaderValues(BinaryWriter bw, PtHeader header)
    {
        foreach (var j in header.Values)
        {
            bw.Write((byte)j.Key);
            Write(bw, j.Value);
        }
    }

    private static void WriteAudioHeaderValues(BinaryWriter bw, PtHeader header)
    {
        foreach (var j in header.AudioValues)
        {
            if (j.Key == PtAudioHeaderField.EndOfHeader || j.Value.Value != PtHeader.Default[j.Key].Value)
            {
                bw.Write((byte)j.Key);
                Write(bw, j.Value);
            }
        }
    }

    private static int ReadBytes(BinaryReader br, byte count)
    {
        int result = 0;
        for (int i = 0; i < count; i++)
        {
            byte byteValue = br.ReadByte();
            result <<= 8;
            result += byteValue;
        }
        return result;
    }

    private static void Write(BinaryWriter bw, PtHeaderValue value)
    {
        bw.Write(value.Length);
        bw.Write(BitConverter.GetBytes(value.Value).Reverse().Skip(4 - value.Length).Take(value.Length).ToArray());
    }
}