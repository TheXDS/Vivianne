using System.Runtime.InteropServices;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
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
        var header = br.MarshalReadStruct<BnkV2Header>();
        var sampleOffsets = br.MarshalReadArray<int>(header.Samples);
        var samples = new List<BnkBlob>();
        foreach (var (index, offset) in sampleOffsets.WithIndex())
        {
            br.BaseStream.Seek(12 + offset + (sizeof(int) * index), SeekOrigin.Begin);
            var ptHeader = ReadPtHeader(br);
            samples.Add(new BnkBlob
            {
                Channels = (byte)ptHeader[PtAudioHeaderField.Channels],
                Compression = ptHeader[PtAudioHeaderField.Compression].Value != 0,
                SampleRate = (ushort)ptHeader[PtAudioHeaderField.SampleRate],
                LoopStart = ptHeader[PtAudioHeaderField.LoopOffset],
                LoopLength = ptHeader[PtAudioHeaderField.LoopLength],
                BytesPerSample = (byte)ptHeader[PtAudioHeaderField.BytesPerSample],
                SampleData = br.ReadBytesAt(ptHeader[PtAudioHeaderField.DataOffset], ptHeader[PtAudioHeaderField.BytesPerSample] * ptHeader[PtAudioHeaderField.NumSamples]),
                Properties = new Dictionary<byte, PtHeaderValue>(ptHeader.Values.Select(p => new KeyValuePair<byte, PtHeaderValue>((byte)p.Key, p.Value)))
            });
        }
        var bnk = new BnkFile();
        bnk.Streams.AddRange(samples);
        return bnk;
    }

    /// <inheritdoc/>
    public void SerializeTo(BnkFile entity, Stream stream)
    {
        using BinaryWriter bw = new(stream);
        using MemoryStream headersStream = new();
        using BinaryWriter headersBw = new(headersStream);
        using MemoryStream poolStream = new();
        using BinaryWriter poolBw = new(poolStream);

        var bnkHeaderSize = CalculateBnkHeaderSize(entity);
        var ptHeadersSize = CalculateTotalPtHeadersSize(entity);
        var poolOffset = bnkHeaderSize + ptHeadersSize;
        var bnkHeader = BnkV2Header.Empty();
        var headerOffsets = new List<int>();
        var sampleOffsets = new List<int>();

        bnkHeader.PoolOffset = poolOffset;
        bnkHeader.Samples = (short)entity.Streams.Count;

        foreach (var (index, j) in entity.Streams.WithIndex())
        {
            sampleOffsets.Add((int)poolStream.Position + poolOffset);
            poolBw.Write(j.SampleData);

            headerOffsets.Add((int)headersStream.Position + (entity.Streams.Count * 4) - (4*index));
            var ptHeader = ToPtHeader(j);
            ptHeader[PtAudioHeaderField.DataOffset] = sampleOffsets.Last();
            WritePtHeader(headersBw, ptHeader);
        }
        bw.MarshalWriteStruct(bnkHeader);
        bw.MarshalWriteStructArray(headerOffsets.ToArray());
        bw.Write(headersStream.ToArray());
        bw.Write(poolStream.ToArray());
    }

    private static int CalculateBnkHeaderSize(BnkFile bnk)
    {
        return Marshal.SizeOf<BnkV2Header>() + bnk.Streams.Count * sizeof(int);
    }

    private static int CalculateTotalPtHeadersSize(BnkFile entity)
    {
        return entity.Streams.Select(ToPtHeader).Sum(CalculatePtHeaderSize);
    }

    private static int CalculatePtHeaderSize(PtHeader header)
    {
        return 6 + // Magic, = PT\0\0; 0xFD for audio header marker and 0xFF for end of header
            header.Values.Sum(p => p.Value.Length + 2) + // field byte (1) + length byte itself(1) + value length
            header.AudioValues.Sum(p => p.Value.Length + 2); // field byte (1) + length byte itself(1) + value length
    }

    private static PtHeader ToPtHeader(BnkBlob blob)
    {
        var header = new PtHeader();
        header.Values.AddRange(blob.Properties.Select(p => new KeyValuePair<PtHeaderField, PtHeaderValue>((PtHeaderField)p.Key, p.Value)));
        header[PtAudioHeaderField.Channels] = blob.Channels;
        header[PtAudioHeaderField.Compression] = blob.Compression;
        header[PtAudioHeaderField.SampleRate] = blob.SampleRate;
        header[PtAudioHeaderField.NumSamples] = blob.SampleData.Length / blob.BytesPerSample;
        header[PtAudioHeaderField.LoopOffset] = blob.LoopStart;
        header[PtAudioHeaderField.LoopLength] = blob.LoopLength;
        header[PtAudioHeaderField.BytesPerSample] = blob.BytesPerSample;
        return header;
    }

    private static PtHeader ReadPtHeader(BinaryReader br)
    {
        var result = new PtHeader();
        if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
        {
            throw new InvalidDataException();
        }
        while (true)
        {
            var field = (PtHeaderField)br.ReadByte();
            if (field == PtHeaderField.EndOfHeader) break;
            if (field != PtHeaderField.AudioHeader)
            {
                var length = br.ReadByte();
                result[field] = new PtHeaderValue
                {
                    Length = length,
                    Value = ReadBytes(br, length)
                };
            }
            else
            {
                ReadAudioHeader(result, br);
            }
        }
        return result;
    }

    private static void WritePtHeader(BinaryWriter bw, PtHeader header)
    {
        bw.Write("PT\0\0"u8.ToArray());
        WriteHeader(bw, header);
        bw.Write((byte)PtHeaderField.AudioHeader);
        WriteAudioHeader(bw, header);
        bw.Write((byte)PtHeaderField.EndOfHeader);
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
             * to get out of the parser loop lives here.
             */
            if (field == PtAudioHeaderField.EndOfHeader) break;
        }
    }

    private static void WriteHeader(BinaryWriter bw, PtHeader header)
    {
        foreach (var j in header.Values)
        {
            bw.Write((byte)j.Key);
            Write(bw, j.Value);
        }
    }

    private static void WriteAudioHeader(BinaryWriter bw, PtHeader header)
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
