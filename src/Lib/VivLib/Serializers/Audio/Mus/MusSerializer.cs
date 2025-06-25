using System.IO;
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
            if (ReadAsfFile(br) is { } asf) mus.AsfSubStreams.Add((int)stream.Position, asf);
        } while ((stream.Position + Marshal.SizeOf<AsfBlockHeader>()) < stream.Length);
        return mus;
    }

    /// <inheritdoc/>
    public void SerializeTo(MusFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }

    private static AsfFile? ReadAsfFile(BinaryReader br)
    {
        AsfData data = new();
        try
        {
            while (true)
            {
                var blockHeader = br.MarshalReadStruct<AsfBlockHeader>();

                // Check for possible alignment issues...
                if (!blockHeader.Magic[0..2].SequenceEqual("SC"u8.ToArray()))
                {
                    // This file likely uses 4-byte alignment. Align to 4 bytes and try again.
                    br.BaseStream.Position -= Marshal.SizeOf<AsfBlockHeader>();
                    br.BaseStream.Position += 4 - (br.BaseStream.Position % 4);
                    blockHeader = br.MarshalReadStruct<AsfBlockHeader>();

                    // If we're still in the same situation, throw an exception.
                    if (!blockHeader.Magic[0..2].SequenceEqual("SC"u8.ToArray()))
                    {
                        throw new InvalidDataException($"Invalid or corrupt ASF block: {Encoding.Latin1.GetString(blockHeader.Magic)}");
                    }
                }
                var blockData = br.ReadBytes(blockHeader.BlockSize - Marshal.SizeOf<AsfBlockHeader>());
                switch (Encoding.Latin1.GetString(blockHeader.Magic))
                {
                    case "SCHl": ReadPtHeader(data, blockData); break;
                    case "SCCl": ReadCount(data, blockData); break;
                    case "SCDl": ReadAudioBlock(data, blockData); break;
                    case "SCLl": data.LoopOffset = BitConverter.ToInt32(blockData); break;
                    case "SCEl": return data.ToFile();
                    default:
                        throw new InvalidDataException($"Unknown ASF block type: {Encoding.Latin1.GetString(blockHeader.Magic)}. Length: {blockData.Length} bytes");
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
        return ReadAsfFile(br) ?? throw new InvalidDataException("The file does not seem to be a valid ASF stream.");
    }
}
