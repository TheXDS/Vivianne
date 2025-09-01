using System.Runtime.InteropServices;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Serializers.Audio.Bnk;

/// <summary>
/// Implements a serializer for BNK audio files.
/// </summary>
public partial class BnkSerializer : ISerializer<BnkFile>
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
        var ptHeaderOffsets = br.MarshalReadArray<int>(header.Streams);
        List<PtHeader?> ptHeaders = [];
        List<BnkStream?> streams = [];
        foreach (var (index, offset) in ptHeaderOffsets.WithIndex())
        {
            if (offset == 0)
            {
                ptHeaders.Add(null);
                continue;
            }
            br.BaseStream.Seek(headerSize + offset + (sizeof(int) * index), SeekOrigin.Begin);
            if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
            {
                throw new InvalidDataException();
            }
            ptHeaders.Add(PtHeaderSerializerHelper.ReadPtHeader(br));
        }
        var streamOffsets = ptHeaders.NotNull().Select(p => p[PtAudioHeaderField.DataOffset].Value).ToArray();
        foreach (var (index, ptHeader) in ptHeaders.WithIndex())
        {
            if (ptHeader is null)
            {
                streams.Add(null);
            }
            else
            {
                streams.Add(ToBnkStream(ptHeader, br, index.ToString(), false, streamOffsets));
            }
        }
        var bnk = new BnkFile()
        {
            FileVersion = header.Version,
            PayloadSize = header.Version == 4 ? payloadSize : (int)stream.Length - headerSize
        };
        bnk.Streams.AddRange(streams);
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

        var poolOffsetPadding = MajorBlockAlignment - (poolOffset % MajorBlockAlignment);
        poolOffset += (poolOffsetPadding != MajorBlockAlignment ? poolOffsetPadding : 0);

        var bnkHeader = new BnkHeader()
        {
            Magic = "BNKl"u8.ToArray(),
            Version = entity.FileVersion,
            Streams = (short)entity.Streams.Count,
        };
        var headerOffsets = new List<int>();
        foreach (var (index, j) in entity.Streams.WithIndex())
        {
            if (j is null)
            {
                headerOffsets.Add(0);
                continue;
            }
            var pt = WriteAudioData(poolBw, j, poolOffset);
            headerOffsets.Add((int)headersStream.Position + (entity.Streams.Count * 4) - (4 * index));
            PtHeaderSerializerHelper.WritePtHeader(headersBw, pt);
            var padding = PtHeaderBlockAlignment - ((int)(headersStream.Length + bnkHeaderSize) % PtHeaderBlockAlignment);
            if (padding != PtHeaderBlockAlignment)
            {
                headersBw.Write(new byte[padding]);
            }
        }
        bnkHeader.PoolOffset = poolOffset;
        fileBw.MarshalWriteStruct(bnkHeader);
        if (entity.FileVersion == 0x04) fileBw.MarshalWriteStruct(new BnkV4Header { PoolSize = (int)poolStream.Length, Unk_1 = -1 });
        fileBw.MarshalWriteStructArray(headerOffsets.ToArray());
        fileBw.Write(headersStream.ToArray());

        if (fileBw.BaseStream.Length < poolOffset)
        {
            fileBw.Write(new byte[poolOffset - fileBw.BaseStream.Length]);
        }

        fileBw.Write(poolStream.ToArray());
    }
}
