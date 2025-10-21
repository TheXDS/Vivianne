using System.Runtime.InteropServices;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Serializers.Audio.Bnk;

public partial class BnkSerializer : ISerializer<BnkFile>
{
    private const int PtHeaderBlockAlignment = 4;
    private const int MajorBlockAlignment = 8;

    private static byte[] ReadHeaderAttachment(BinaryReader br, BnkHeader header)
    {
        var dataStart = (int)(br.BaseStream.Position + (PtHeaderBlockAlignment - (br.BaseStream.Position % PtHeaderBlockAlignment)));
        var dataLength = header.PoolOffset - dataStart;
        return dataLength > 0 ? br.ReadBytes(dataLength) : [];
    }

    private static IEnumerable<PtHeader?> ReadPtHeaders(BinaryReader br, int headerSize, int[] ptHeaderOffsets)
    {
        foreach (var (index, offset) in ptHeaderOffsets.WithIndex())
        {
            if (offset == 0)
            {
                yield return null;
                continue;
            }
            br.BaseStream.Seek(headerSize + offset + (sizeof(int) * index), SeekOrigin.Begin);
            if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
            {
                throw new InvalidDataException();
            }
            yield return PtHeaderSerializerHelper.ReadPtHeader(br);
        }
    }

    private static IEnumerable<BnkStream?> ReadBnkStreams(BinaryReader br, PtHeader?[] ptHeaders)
    {
        var streamOffsets = ptHeaders.NotNull().Select(p => p[PtAudioHeaderField.DataOffset].Value).ToArray();
        foreach (var (index, ptHeader) in ptHeaders.WithIndex())
        {
            if (ptHeader is null)
            {
                yield return null;
            }
            else
            {
                yield return ToBnkStream(ptHeader, br, index.ToString(), false, streamOffsets);
            }
        }
    }

    private static PtHeader WriteAudioData(BinaryWriter bw, BnkStream stream, in int poolOffset)
    {
        var ptHeader = ToPtHeader(stream, poolOffset);
        ptHeader[PtAudioHeaderField.DataOffset] = new PtHeaderValue(4, (int)bw.BaseStream.Position + poolOffset);
        bw.Write(stream.SampleData);
        if (stream.PostAudioStreamData.Length > 0)
        {
            bw.Write(stream.PostAudioStreamData);
        }
        if (stream.AltStream is not null)
        {
            ptHeader.AltStream = WriteAudioData(bw, stream.AltStream, poolOffset);
        }
        return ptHeader;
    }

    private static BnkStream ToBnkStream(PtHeader header, BinaryReader br, string id, bool isAltStream, int[] sampleOffsets)
    {
        var sampleData = Mappings.AudioCodecSelector.TryGetValue((CompressionMethod)header[PtAudioHeaderField.Compression].Value, out var codec)
            ? codec.Invoke().Decode(br.ReadBytesAt(header[PtAudioHeaderField.DataOffset], GetByteCount(br, header)), header)
            : throw new InvalidOperationException($"Unsupported audio codec: {header[PtAudioHeaderField.Compression].Value:X2}");

        return new BnkStream
        {
            Id = id,
            Channels = (byte)header[PtAudioHeaderField.Channels],
            Compression = (CompressionMethod)header[PtAudioHeaderField.Compression].Value,
            SampleRate = (ushort)header[PtAudioHeaderField.SampleRate],
            LoopStart = header[PtAudioHeaderField.LoopOffset],
            LoopEnd = header[PtAudioHeaderField.LoopEnd],
            BytesPerSample = (byte)header[PtAudioHeaderField.BytesPerSample],
            SampleData = sampleData,
            IsAltStream = isAltStream,
            AltStream = header.AltStream is { } altHeader ? ToBnkStream(altHeader, br, $"Alt {id}", true, sampleOffsets) : null,
            Properties = new Dictionary<byte, PtHeaderValue>(header.Values.Select(p => new KeyValuePair<byte, PtHeaderValue>((byte)p.Key, p.Value))),
            CustomAudioProperties = new Dictionary<byte, PtHeaderValue>(header.AudioValues.Select(p => new KeyValuePair<byte, PtHeaderValue>((byte)p.Key, p.Value)).Where(FilterOutCommonProps))
        };
    }

    private static bool FilterOutCommonProps(KeyValuePair<byte, PtHeaderValue> pair)
    {
        return FilterOutCommonProps(new KeyValuePair<PtAudioHeaderField, PtHeaderValue>((PtAudioHeaderField)pair.Key, pair.Value));
    }

    private static bool FilterOutCommonProps(KeyValuePair<PtAudioHeaderField, PtHeaderValue> pair)
    {
        return !((PtAudioHeaderField[])[
            PtAudioHeaderField.Channels,
            PtAudioHeaderField.Compression,
            PtAudioHeaderField.SampleRate,
            PtAudioHeaderField.NumSamples,
            PtAudioHeaderField.LoopOffset,
            PtAudioHeaderField.LoopEnd,
            PtAudioHeaderField.DataOffset,
            PtAudioHeaderField.BytesPerSample,
            PtAudioHeaderField.EndOfHeader,
            ]).Contains(pair.Key);
    }

    private static int GetByteCount(BinaryReader br, PtHeader header)
    {
        return int.Clamp(
            header[PtAudioHeaderField.Channels] * header[PtAudioHeaderField.BytesPerSample] * header[PtAudioHeaderField.NumSamples],
            0,
            (int)(br.BaseStream.Length - header[PtAudioHeaderField.DataOffset]));
    }

    private static int CalculateBnkHeaderSize(BnkFile bnk)
    {
        return (bnk.FileVersion == 4 ? Marshal.SizeOf<BnkV4Header>() - 4 : 0) + (bnk.Streams.Count * sizeof(int) * 2);
    }

    private static int CalculateTotalPtHeadersSize(BnkFile entity)
    {
        return entity.Streams.NotNull().Select(ToPtHeader).Sum(CalculatePtHeaderSize);
    }

    private static int CalculatePtHeaderSize(PtHeader header)
    {
        /* The total size of the PT headers should be equal to the calculated
         * size of all of their defined internal values plus 5 bytes, which are
         * the string "PT\0\0" and the marker for the end of header. Further
         * padding bytes might be necessary to align the PT header to an 8-byte
         * boundary.
         */
        var sum = CalculatePtHeaderSizeNoAdjust(header) + 5;
        var padding = PtHeaderBlockAlignment - (sum % PtHeaderBlockAlignment);
        return sum + (padding != PtHeaderBlockAlignment ? padding : 0);
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
         * equal to the defaults then it has to be ommitted. That way, older
         * parsers won't get confused in terms of how to attempt to load the
         * data. This is not a requirement *per the spec*, but it's an
         * unofficial requirement given how the games try to load audio data.
         */
        return header.Values.Sum(p => p.Value.Length + 2) +
            header.AudioValues.Where(ShouldValueBeIncludedInCount).Sum(p => p.Value.Length + 2) + 1 + /* +1 to account for `0xFD` PT Header audio props marker */
            (header.AltStream is not null ? CalculatePtHeaderSizeNoAdjust(header.AltStream) + 1 : 0) +  /* +1 to account for `0xFE` PT Header alt stream marker */
            (header.AudioValues[PtAudioHeaderField.DataOffset] != 0 ? 4 : 0) /* Always use 4 byte offsets for data offset */;
    }

    private static bool ShouldValueBeIncludedInCount(KeyValuePair<PtAudioHeaderField, PtHeaderValue> p)
    {
        return p.Key switch
        {
            // These PT values MUST be included even if they were to be 0
            PtAudioHeaderField.EndOfHeader or
            PtAudioHeaderField.DataOffset
            => true,

            // Otherwise, check for equality with its predefined default value
            _ => !PtHeader.Default.AudioValues.TryGetValue(p.Key, out var defaultValue) || p.Value.Value != defaultValue
        };
    }

    private static PtHeader ToPtHeader(BnkStream blob, int poolOffset)
    {
        var header = new PtHeader();
        header.Values.AddRange(blob.Properties.Select(p => new KeyValuePair<PtHeaderField, PtHeaderValue>((PtHeaderField)p.Key, p.Value)));
        header.AudioValues.AddRange(blob.CustomAudioProperties.Select(p => new KeyValuePair<PtAudioHeaderField, PtHeaderValue>((PtAudioHeaderField)p.Key, p.Value)).Where(FilterOutCommonProps));
        header[PtAudioHeaderField.Channels] = blob.Channels;
        header[PtAudioHeaderField.Compression] = (byte)blob.Compression;
        header[PtAudioHeaderField.SampleRate] = blob.SampleRate;
        header[PtAudioHeaderField.NumSamples] = ((blob.SampleData.Length) / blob.BytesPerSample) / blob.Channels;
        header[PtAudioHeaderField.LoopOffset] = blob.LoopStart;
        header[PtAudioHeaderField.LoopEnd] = blob.LoopEnd;
        header[PtAudioHeaderField.BytesPerSample] = blob.BytesPerSample;
        if (blob.AltStream is not null)
        {
            header.AltStream = ToPtHeader(blob.AltStream, poolOffset);
        }
        return header;
    }

    private static BnkHeader CreateNewHeader(BnkFile entity)
    {
        return new BnkHeader()
        {
            Magic = "BNKl"u8.ToArray(),
            Version = entity.FileVersion,
            Streams = (short)entity.Streams.Count,
        };
    }
}
