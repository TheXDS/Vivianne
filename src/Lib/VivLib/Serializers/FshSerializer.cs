using System.Diagnostics;
using TheXDS.Vivianne.Models;
using static System.Text.Encoding;

namespace TheXDS.Vivianne.Serializers;

/// <summary>
/// Implements a serializer that can read and write <see cref="FshTexture"/>
/// entities.
/// </summary>
public class FshSerializer : ISerializer<FshTexture>
{
    private static readonly byte[] Header = "SHPI"u8.ToArray();
    private static readonly byte[] DirId = "GIMX"u8.ToArray();

    // Magic pixel format length. Used to calculate blob size if = 0
    private static readonly Dictionary<GimxFormat, byte> MagicFormat = new()
    {
        {GimxFormat.Palette,    4}, // 32-bit 256 Color palette
        {GimxFormat.Indexed8,   1}, // 1 byte per pixel (256 colors)
        {GimxFormat.Bgr565,     2}, // 2 bytes per pixel (16 bit color).
        {GimxFormat.Bgra32,     4}, // 4 bytes per pixel (RGBA32)
    };

    /// <inheritdoc/>
    public FshTexture Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var entries = GetFshEntries(reader);
        Dictionary<string, int> fileOffsets = [];
        while (entries-- > 0)
        {
            var name = ASCII.GetString(reader.ReadBytes(4));
            var offset = reader.ReadInt32();
            fileOffsets.Add(name, offset);
        }

        var fsh = new FshTexture();
        foreach (var j in fileOffsets)
        {
            reader.BaseStream.Seek(j.Value, SeekOrigin.Begin);
            var endOffset = fileOffsets.Values.Cast<int?>().Order().FirstOrDefault(p => p > j.Value) ?? (int)stream.Length;
            if (ReadGimx(reader, endOffset) is { } gimx)
            {
                fsh.Images.Add(j.Key, gimx);
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
    public void SerializeTo(FshTexture entity, Stream stream)
    {
        using BinaryWriter writer = new(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(entity.Images));
        writer.Write(entity.Images.Count);
        writer.Write(DirId);
        int o = entity.Images.Count * 8 + 16;
        foreach (var j in entity.Images)
        {
            writer.Write(ASCII.GetBytes(j.Key));
            writer.Write(o);
            // 16 = GIMX header (16 bytes)
            o += 16 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        foreach (var j in entity.Images.Values)
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

    private static int GetFileSize(Dictionary<string, Gimx> directory)
    {
        var sum = 16; // FSH header size
        foreach (var j in directory)
        {
            // 24 = GIMX header (16 bytes) + entry name (4 bytes) + data offset (4 bytes)
            sum += 24 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        return sum;
    }

    private static int GetFshEntries(BinaryReader reader)
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
        var entries = reader.ReadInt32();
        if (!reader.ReadBytes(4).SequenceEqual(DirId))
        {
            throw new InvalidDataException("Invalid Directory ID.");
        }
        return entries;
    }

    private static Gimx? ReadGimx(BinaryReader reader, int dataEndOffset)
    {
        int currentOffset = (int)reader.BaseStream.Position;
        var magic = (GimxFormat)reader.ReadByte();
        if (!MagicFormat.TryGetValue(magic, out byte value)) return null;
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
        return new Gimx
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