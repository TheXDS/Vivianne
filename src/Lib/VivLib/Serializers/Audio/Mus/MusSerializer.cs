using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

/// <summary>
/// Implements a serializer for MUS/ASF files.
/// </summary>
/// <remarks>
/// Technically, a MUS file with a single ASF sub-stream is essentially an ASF
/// file. Therefore, this serializer can be used to read an ASF file and return
/// it as a <see cref="MusFile"/> instance. However, a MUS file with multiple
/// sub-streams is not a valid .ASF file, and therefore this serializer can
/// only implement deserialization of <see cref="AsfFile"/> instances, even if
/// the formats are otherwise equivalent.
/// </remarks>
public class MusSerializer : ISerializer<MusFile>, IOutSerializer<AsfFile>
{
    /// <inheritdoc/>
    public MusFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var mus = new MusFile();
        do
        {
            mus.AsfSubStreams.Add((int)stream.Position, ReadAsfFile(br));
            stream.Position += 4 - (stream.Position % 4);
        } while (stream.Position < stream.Length);
        return mus;
    }

    /// <inheritdoc/>
    public void SerializeTo(MusFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }

    private static AsfFile ReadAsfFile(BinaryReader br)
    {
        AsfData data = new();
        try
        {
            while (true)
            {
                var blockHeader = br.MarshalReadStruct<AsfBlockHeader>();
                var blockData = br.ReadBytes(blockHeader.BlockSize - Marshal.SizeOf<AsfBlockHeader>());
                switch (Encoding.Latin1.GetString(blockHeader.Magic))
                {
                    case "SCHl": ReadPtHeader(data, blockData); break;
                    case "SCCl": ReadCount(data, blockData); break;
                    case "SCDl": ReadAudioBlock(data, blockData); break;
                    case "SCLl": data.LoopOffset = BitConverter.ToInt32(blockData); break;
                    case "SCEl": return data.ToFile();
                    default: 
                        System.Diagnostics.Debug.Print($"Unknown ASF block type: {Encoding.Latin1.GetString(blockHeader.Magic)}. Length: {blockData.Length} bytes");
                        br.BaseStream.Skip(blockData.Length);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new EndOfStreamException("Unexpected end of file.", ex);
        }
    }

    private static void ReadAudioBlock(AsfData d, byte[] blockData)
    {
        var data = Mappings.AudioCodecSelector.TryGetValue((CompressionMethod)d.PtHeader[PtAudioHeaderField.Compression].Value, out var codec)
            ? codec.Invoke().Decode(blockData, d.PtHeader)
            : throw new InvalidOperationException($"Unsupported audio codec: {d.PtHeader[PtAudioHeaderField.Compression].Value:X2}");

        d.AudioBlocks.Add(data);
    }

    private static void ReadCount(AsfData d, byte[] blockData)
    {
        d.BlockCount = BitConverter.ToInt32(blockData);
    }

    private static void ReadPtHeader(AsfData d, byte[] blockData)
    {
        using BinaryReader br = new(new MemoryStream(blockData));
        if (!br.ReadBytes(4).SequenceEqual("PT\0\0"u8.ToArray()))
        {
            throw new InvalidDataException();
        }
        d.PtHeader = PtHeaderSerializerHelper.ReadPtHeader(br);
    }

    AsfFile IOutSerializer<AsfFile>.Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        return ReadAsfFile(br);
    }
}
