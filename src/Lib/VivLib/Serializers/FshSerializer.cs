using System.Diagnostics;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using static System.Text.Encoding;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a serializer that can read and write <see cref="FshFile"/>
/// entities.
/// </summary>
public class FshSerializer : ISerializer<FshFile>
{
    private static readonly byte[] Header = "SHPI"u8.ToArray();
    private static readonly byte[][] DirIds =
    [
        "GIMX"u8.ToArray(),
#if EnableFullFshFormat
        "G354"u8.ToArray(),
        "G264"u8.ToArray(),
        "G266"u8.ToArray(),
        "G290"u8.ToArray(),
        "G315"u8.ToArray(),
        "G344"u8.ToArray(),
#endif
    ];

    /// <inheritdoc/>
    public FshFile Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        VerifyValidFsh(reader);
        var entries = reader.ReadInt32();
        var dirId = reader.ReadBytes(4);
        if (!DirIds.Any(dirId.SequenceEqual))
        {
            throw new InvalidDataException("Invalid Directory ID.");
        }
        Dictionary<string, int> fileOffsets = [];
        while (entries-- > 0)
        {
            var name = Latin1.GetString(reader.ReadBytes(4));
            var offset = reader.ReadInt32();
            fileOffsets.Add(name, offset);
        }

        var fsh = new FshFile() { DirectoryId = Latin1.GetString(dirId) };
        foreach (var j in fileOffsets)
        {
            reader.BaseStream.Seek(j.Value, SeekOrigin.Begin);
            var endOffset = fileOffsets.Values.Cast<int?>().Order().FirstOrDefault(p => p > j.Value) ?? (int)stream.Length;
            if (ReadFshBlob(reader, endOffset) is { } gimx)
            {
                fsh.Entries.Add(j.Key, gimx);
            }
#if DEBUG
            else
            {
                Debug.Print($"Could not read '{j.Key}'. Skipping...");
            }
#endif
        }
        return fsh;
    }

    /// <inheritdoc/>
    public void SerializeTo(FshFile entity, Stream stream)
    {
        using BinaryWriter writer = new(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(entity.Entries));
        writer.Write(entity.Entries.Count);
        writer.Write(Latin1.GetBytes(entity.DirectoryId));
        int o = (entity.Entries.Count * 8) + 16;
        foreach (var j in entity.Entries)
        {
            writer.Write(Latin1.GetBytes(j.Key));
            writer.Write(o);
            // 16 = GIMX header (16 bytes)
            o += 16 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        foreach (var j in entity.Entries.Values)
        {
            writer.Write((byte)j.Magic);
            writer.Write(BitConverter.GetBytes(j.Footer.Length != 0 ? j.PixelData.Length + 16 : 0)[0..3]);
            writer.Write(j.Width);
            writer.Write(j.Height);
            writer.Write(j.XRotation);
            writer.Write(j.YRotation);
            writer.Write(j.XPosition);
            writer.Write(j.YPosition);
            writer.Write(j.PixelData);
            if (j.Footer.Length != 0) writer.Write(j.Footer);
        }
    }

    private static void VerifyValidFsh(BinaryReader reader)
    {
        if (!reader.ReadBytes(4).SequenceEqual(Header))
        {
            throw new InvalidDataException("Invalid header.");
        }
        var fshLength = reader.ReadInt32();
        if (reader.BaseStream.CanSeek && reader.BaseStream.Length != fshLength)
        {
            throw new InvalidDataException("FSH file length mismatch");
        }
    }

    private static int GetFileSize(Dictionary<string, FshBlob> directory)
    {
        var sum = 16; // FSH header size
        foreach (var j in directory)
        {
            // 24 = GIMX header (16 bytes) + entry name (4 bytes) + data offset (4 bytes)
            sum += 24 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        return sum;
    }

    private static FshBlob? ReadFshBlob(BinaryReader reader, int dataEndOffset)
    {
        int currentOffset = (int)reader.BaseStream.Position;
        var magic = (FshBlobFormat)reader.ReadByte();
        if (!Mappings.FshBlobBytesPerPixel.TryGetValue(magic, out byte value)) return null;
        var footerOffset = BitConverter.ToInt32([.. reader.ReadBytes(3), (byte)0]);
        var width = reader.ReadUInt16();
        var height = reader.ReadUInt16();
        var xrot = reader.ReadUInt16();
        var yrot = reader.ReadUInt16();
        var xpos = reader.ReadUInt16();
        var ypos = reader.ReadUInt16();
        var pixelDataSize = width * height * value;
        var pixelData = reader.ReadBytes(pixelDataSize);
        byte[] footer = [];
        if (footerOffset != 0)
        {
            var footerSize = dataEndOffset - (currentOffset + footerOffset);
            footer = reader.ReadBytes(footerSize);
        }
        return new FshBlob
        {
            Magic = magic,
            Width = width,
            Height = height,
            XRotation = xrot,
            YRotation = yrot,
            XPosition = xpos,
            YPosition = ypos,
            PixelData = pixelData,
            Footer = footer
        };
    }
}