using System.Diagnostics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Bnk;

namespace TheXDS.Vivianne.Serializers.Bnk;

/// <summary>
/// Implements a serializer for BNK audio files.
/// </summary>
public class BnkSerializer : ISerializer<BnkFile>
{
    /// <inheritdoc/>
    public BnkFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<BnkHeader>();
        var headerSize = Marshal.SizeOf<BnkHeader>();
        int payloadSize = 0;
        if (header.Version == 0x04)
        {
            payloadSize = br.MarshalReadStruct<BnkV4Header>().PoolSize;
            headerSize += Marshal.SizeOf<BnkV4Header>();
        }
        var sampleOffsets = br.MarshalReadArray<int>(header.Streams);
        List<BnkStream?> samples = [];
        foreach (var (index, offset) in sampleOffsets.WithIndex())
        {
            if (offset == 0)
            {
                samples.Add(null);
                continue;
            }
            br.BaseStream.Seek(headerSize + offset + (sizeof(int) * index), SeekOrigin.Begin);
            if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
            {
                throw new InvalidDataException();
            }
            var ptHeader = ReadPtHeader(br);
            samples.Add(ToBnkStream(ptHeader, br, index.ToString()));
        }
        var bnk = new BnkFile()
        {
            FileVersion = header.Version,
            PayloadSize = header.Version == 4 ? payloadSize : (int)stream.Length - headerSize
        };
        bnk.Streams.AddRange(samples);
        return bnk;
    }

    /// <inheritdoc/>
    public void SerializeTo(BnkFile entity, Stream stream)
    {
        using BinaryWriter fileBw = new(stream);
        using MemoryStream headersStream = new();
        using BinaryWriter headersBw = new(headersStream);
        using MemoryStream poolStream = new();
        using BinaryWriter poolBw = new(poolStream);

        var bnkHeaderSize = CalculateBnkHeaderSize(entity);
        var ptHeadersSize = CalculateTotalPtHeadersSize(entity);
        var poolOffset = bnkHeaderSize + ptHeadersSize;
        var bnkHeader = new BnkHeader()
        {
            Magic = "BNKl"u8.ToArray(),
            Version = entity.FileVersion,
            Streams = (short)entity.Streams.Count,
            PoolOffset = poolOffset
        };
        var headerOffsets = new List<int>();
        foreach (var (index, j) in entity.Streams.WithIndex())
        {
            if (j is null)
            {
                headerOffsets.Add(0);
                continue;
            }
            var pt = WritePtHeaderData(poolBw, j, poolOffset);
            headerOffsets.Add((int)headersStream.Position + (entity.Streams.Count * 4) - (4*index));
            headersBw.Write("PT\0\0"u8.ToArray());
            WritePtHeader(headersBw, pt);
            headersBw.Write((byte)PtHeaderField.EndOfHeader);
        }
        fileBw.MarshalWriteStruct(bnkHeader);
        if (entity.FileVersion == 0x04) fileBw.MarshalWriteStruct(new BnkV4Header { PoolSize = (int)poolStream.Length, Unk_1 = -1 });
        fileBw.MarshalWriteStructArray(headerOffsets.ToArray());
        fileBw.Write(headersStream.ToArray());
        fileBw.Write(poolStream.ToArray());
    }

    private static PtHeader WritePtHeaderData(BinaryWriter bw, BnkStream stream, in int poolOffset)
    {
        var ptHeader = ToPtHeader(stream);
        ptHeader[PtAudioHeaderField.DataOffset] = (int)bw.BaseStream.Position + poolOffset;
        bw.Write(stream.SampleData);
        if (stream.AltStream is not null)
        {
            ptHeader.AltStream = WritePtHeaderData(bw, stream.AltStream, poolOffset);
        }
        return ptHeader;
    }

    private static BnkStream ToBnkStream(PtHeader header, BinaryReader br, string id, bool isAltStream = false)
    {
        var sampleData = br.ReadBytesAt(header[PtAudioHeaderField.DataOffset], int.Clamp(header[PtAudioHeaderField.BytesPerSample] * header[PtAudioHeaderField.NumSamples], 0, (int)(br.BaseStream.Length - header[PtAudioHeaderField.DataOffset])));
        if (header[PtAudioHeaderField.Compression].Value != 0)
        {
#if EnableBnkCompression
            sampleData = EaAdpcmCodec.Decompress(sampleData, header[PtAudioHeaderField.NumSamples]);
#else
            Debug.Print("No support has been implemented for compressed streams yet.");
#endif
        }
        return new BnkStream
        {
            Id = id,
            Channels = (byte)header[PtAudioHeaderField.Channels],
            Compression = header[PtAudioHeaderField.Compression].Value != 0,
            SampleRate = (ushort)header[PtAudioHeaderField.SampleRate],
            LoopStart = header[PtAudioHeaderField.LoopOffset],
            LoopEnd = header[PtAudioHeaderField.LoopLength],
            BytesPerSample = (byte)header[PtAudioHeaderField.BytesPerSample],
            SampleData = sampleData,
            IsAltStream = isAltStream,
            AltStream = header.AltStream is { } altHeader ? ToBnkStream(altHeader, br, $"Alt {id}", true) : null,
            Properties = new Dictionary<byte, PtHeaderValue>(header.Values.Select(p => new KeyValuePair<byte, PtHeaderValue>((byte)p.Key, p.Value)))
        };
    }

    private static int CalculateBnkHeaderSize(BnkFile bnk)
    {
        return Marshal.SizeOf<BnkHeader>() + (bnk.FileVersion == 4 ? Marshal.SizeOf<BnkV4Header>() : 0) + bnk.Streams.Count * sizeof(int);
    }

    private static int CalculateTotalPtHeadersSize(BnkFile entity)
    {
        /* The total size of the PT headers should be equal to the calculated
         * size of all of their defined internal values plus 5 bytes, which are
         * the string "PT\0\0" and the marker for the end of header. Further
         * padding bytes might be permitted (or even necessary, will need to
         * test and find out)
         */
        return entity.Streams.NotNull().Select(ToPtHeader).Sum(CalculatePtHeaderSize) + (entity.Streams.NotNull().Count() * 5);
    }

    private static int CalculatePtHeaderSize(PtHeader header)
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
         */
        return header.Values.Sum(p => p.Value.Length + 2) +
            header.AudioValues.Sum(p => p.Value.Length + 2) + 1 +
            (header.AltStream is not null ? CalculatePtHeaderSize(header.AltStream) + 1 : 0);
    }

    private static PtHeader ToPtHeader(BnkStream blob)
    {
        var header = new PtHeader();
        header.Values.AddRange(blob.Properties.Select(p => new KeyValuePair<PtHeaderField, PtHeaderValue>((PtHeaderField)p.Key, p.Value)));
        header[PtAudioHeaderField.Channels] = blob.Channels;
        header[PtAudioHeaderField.Compression] = blob.Compression;
        header[PtAudioHeaderField.SampleRate] = blob.SampleRate;
        header[PtAudioHeaderField.NumSamples] = blob.SampleData.Length / blob.BytesPerSample;
        header[PtAudioHeaderField.LoopOffset] = blob.LoopStart;
        header[PtAudioHeaderField.LoopLength] = blob.LoopEnd;
        header[PtAudioHeaderField.BytesPerSample] = blob.BytesPerSample;
        if (blob.AltStream is not null)
        {
            header.AltStream = ToPtHeader(blob.AltStream);
        }
        return header;
    }

    private static PtHeader ReadPtHeader(BinaryReader br)
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

    private static void WritePtHeader(BinaryWriter bw, PtHeader header)
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
            bw.Write((byte)j.Key);
            Write(bw, j.Value);
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
