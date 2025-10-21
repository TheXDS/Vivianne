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
        BnkV4Header? bnkv4 = null;
        if (header.Version == 0x04)
        {
            payloadSize = (bnkv4 = br.MarshalReadStruct<BnkV4Header>()).Value.PoolSize;
            headerSize += Marshal.SizeOf<BnkV4Header>();
        }
        var ptHeaderOffsets = br.MarshalReadArray<int>(header.Streams);
        var ptHeaders = ReadPtHeaders(br, headerSize, ptHeaderOffsets).ToArray();
        var headerAttachment = ReadHeaderAttachment(br, header);
        var streams = ReadBnkStreams(br, ptHeaders);
        var bnk = new BnkFile()
        {
            FileVersion = header.Version,
            PayloadSize = header.Version == 4 ? payloadSize : (int)stream.Length - headerSize,
            HeaderAttachment = headerAttachment,
            Unk_0x10 = bnkv4?.Unk_0x10 ?? -1
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

        var bnkHeader = CreateNewHeader(entity);
        var bnkHeaderSize = CalculateBnkHeaderSize(entity);
        var poolOffset = bnkHeaderSize + CalculateTotalPtHeadersSize(entity) + entity.HeaderAttachment.Length;
        var poolOffsetPadding = MajorBlockAlignment - (poolOffset % MajorBlockAlignment);
        var headerOffsets = new List<int>();

        poolOffset += (poolOffsetPadding != MajorBlockAlignment ? poolOffsetPadding : 0);
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
        headersBw.Write(entity.HeaderAttachment);
        bnkHeader.PoolOffset = poolOffset;
        fileBw.MarshalWriteStruct(bnkHeader);
        if (entity.FileVersion == 0x04) fileBw.MarshalWriteStruct(new BnkV4Header { PoolSize = (int)poolStream.Length, Unk_0x10 = entity.Unk_0x10 });
        fileBw.MarshalWriteStructArray(headerOffsets.ToArray());
        fileBw.Write(headersStream.ToArray());

        if (fileBw.BaseStream.Length < poolOffset)
        {
            fileBw.Write(new byte[poolOffset - fileBw.BaseStream.Length]);
        }

        fileBw.Write(poolStream.ToArray());
    }
}
