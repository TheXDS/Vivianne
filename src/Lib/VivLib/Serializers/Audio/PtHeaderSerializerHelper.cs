using System.Diagnostics;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;

namespace TheXDS.Vivianne.Serializers.Audio;

internal static class PtHeaderSerializerHelper
{
    private const int BlockAlignment = 4;

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
        bw.Write("PT\0\0"u8.ToArray());
        WritePtHeaderContents(bw, header);
        bw.Write((byte)PtHeaderField.EndOfHeader);
    }

    private static void WritePtHeaderContents(BinaryWriter bw, PtHeader header)
    {
        WritePtHeaderValues(bw, header);
        bw.Write((byte)PtHeaderField.AudioHeader);
        WriteAudioHeaderValues(bw, header);
        if (header.AltStream is not null)
        {
            bw.Write((byte)PtHeaderField.AlternateStream);
            WritePtHeaderContents(bw, header.AltStream);
        }
    }

    public static PtHeader ToPtHeader(AudioStreamBase blob)
    {
        var header = new PtHeader();
        header.Values.AddRange(blob.Properties.Select(p => new KeyValuePair<PtHeaderField, PtHeaderValue>((PtHeaderField)p.Key, p.Value)));
        header[PtAudioHeaderField.Channels] = blob.Channels;
        header[PtAudioHeaderField.Compression] = (byte)blob.Compression;
        header[PtAudioHeaderField.SampleRate] = blob.SampleRate;
        header[PtAudioHeaderField.NumSamples] = blob.TotalSamples;
        header[PtAudioHeaderField.LoopOffset] = blob.LoopStart;
        header[PtAudioHeaderField.LoopEnd] = blob.LoopEnd;
        header[PtAudioHeaderField.BytesPerSample] = blob.BytesPerSample;
        return header;
    }

    public static int CalculatePtHeaderSize(PtHeader header)
    {
        var sum = CalculatePtHeaderSizeNoAdjust(header);
        var padding = BlockAlignment - ((sum + 4) % BlockAlignment);
        sum += padding != BlockAlignment ? padding : 0;
        return sum - 1;
    }

    private static int CalculatePtHeaderSizeNoAdjust(PtHeader header)
    {
        /* The calculated size for a single PT header is equal to the sum of
         * each defined value, where a single header value includes a 1 byte
         * value ID, 1 byte indicating the value length (ranging from 1 to 4)
         * and the actual value, which is a Big endian int value.
         * 
         * Sub-headers (such as, audio props, alternate audio stream)
         * follow this same logic, but need to add 1 extra byte that marks the
         * start of that special sub-header.
         * 
         * Audio props header is always present. End of audio props header has
         * its own marker with an actual value. Alt stream might be missing,
         * and if included will end on the same end-of-PT-header marker, so no
         * additional bytes are necessary.
         * 
         * Another consideraion is that the PT headers must be aligned to
         * 8-byte boundaries. This means that writing a PT header may require
         * adding some bytes of padding to make sure those boundaries are
         * respected.
         * 
         * Finally, not all audio properties should be present. If a value is
         * equal to the defaults then it has to be ommitted. that way, older
         * parsers won't get confused in terms of how to attempt to load the
         * data. This is not a requirement *per the spec*, but it's an
         * unofficial requirement given how the games try to load audio data.
         */
        return header.Values.Sum(p => p.Value.Length + 2) +
            header.AudioValues.Where(p => p.Key == PtAudioHeaderField.EndOfHeader || (p.Key == PtAudioHeaderField.DataOffset && p.Value.Length != 0) || p.Value.Value != PtHeader.Default[p.Key].Value).Sum(p => p.Value.Length + 2) + 1 +
            (header.AltStream is not null ? CalculatePtHeaderSizeNoAdjust(header.AltStream) + 1 : 0) + (header.AudioValues[PtAudioHeaderField.NumSamples] != 0 ? 2 : 0);
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
        var specialSort = new Dictionary<PtAudioHeaderField, int>()
        {
            { PtAudioHeaderField.NumSamples, 0 },
            { PtAudioHeaderField.DataOffset, 256}
        };

        foreach (var j in header.AudioValues.OrderBy(p => specialSort.TryGetValue(p.Key, out var customOrder) ? customOrder : (int)p.Key))
        {
            if (j.Key != PtAudioHeaderField.EndOfHeader && (!PtHeader.Default.AudioValues.TryGetValue(j.Key, out var defaultValue) || j.Value.Value != defaultValue))
            {
                bw.Write((byte)j.Key);
                Write(bw, j.Value);
            }
        }
        bw.Write((byte)PtAudioHeaderField.EndOfHeader);
        Write(bw, new PtHeaderValue(4, 0));
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