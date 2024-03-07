using System.Diagnostics;
using TheXDS.Vivianne.Models;
using static System.Text.Encoding;

namespace TheXDS.Vivianne.Containers;

/// <summary>
/// Represents a special archive for image data.
/// </summary>
public class FshFile
{
    private static readonly byte[] Header = "SHPI"u8.ToArray();
    private static readonly byte[] DirId = "GIMX"u8.ToArray();

    // Magic pixel format length. Used to calculate blob size if = 0
    private static readonly Dictionary<byte, byte> MagicFormat = new()
    {
        {0x2A, 4}, // 32-bit 256 Color palette
        {0x7B, 1}, // 1 byte per pixel (256 colors)
        {0x78, 2}, // 2 bytes per pixel (16 bit color).
        {0x7D, 4}, // 4 bytes per pixel (RGBA32)
    };

    /// <summary>
    /// Reads the contents of the FSH file contained in the specified stream.
    /// </summary>
    /// <param name="stream">
    /// Stream from where to read the FSH file contents.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="FshFile"/> class, with all contents of
    /// the FSH file loaded in the directory.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Thrown if the stream does not contain a valid FSH header.
    /// </exception>
    public static FshFile ReadFrom(Stream stream)
    {
        var fsh = new FshFile();
        using var reader = new BinaryReader(stream);
        if (!reader.ReadBytes(4).SequenceEqual(Header))
        {
            throw new InvalidDataException("Invalid header.");
        }
        var fshLength = reader.ReadInt32();
        if (stream.CanSeek && stream.Length != fshLength)
        {
            throw new InvalidDataException("FSH file length mismatch");
        }
        var entries = reader.ReadInt32();
        if (!reader.ReadBytes(4).SequenceEqual(DirId))
        {
            throw new InvalidDataException("Invalid Directory ID.");
        }
        Dictionary<string, int> fileOffsets = [];
        while (entries-- > 0)
        {
            var name = ASCII.GetString(reader.ReadBytes(4));
            var offset = reader.ReadInt32();
            fileOffsets.Add(name, offset);
        }

        using var e = fileOffsets.GetEnumerator();
        while (e.MoveNext())
        {
            reader.BaseStream.Seek(e.Current.Value, SeekOrigin.Begin);
            var magic = reader.ReadByte();
            if (!MagicFormat.ContainsKey(magic))
            {
                Debug.Print($"Unknown pixel format: 0x{magic:X}. Skipping '{e.Current.Key}'...");
                continue;
            }
            var footerOffset = BitConverter.ToInt32([.. reader.ReadBytes(3), (byte)0]);
            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var xrot = reader.ReadUInt16();
            var yrot = reader.ReadUInt16();
            var xpos = reader.ReadUInt16();
            var ypos = reader.ReadUInt16();
            var pixelDataSize = width * height * MagicFormat[magic];
            var pixelData = reader.ReadBytes(pixelDataSize);
            byte[] footer = [];
            if (footerOffset != 0)
            {
                var dataEndOffset = fileOffsets.Values.Cast<int?>().Order().FirstOrDefault(p => p > e.Current.Value) ?? (int)stream.Length;
                var footerSize = dataEndOffset - (e.Current.Value + footerOffset);
                footer = reader.ReadBytes(footerSize);
            }
            fsh.Images.Add(e.Current.Key, new Gimx
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
            });
        }
        return fsh;
    }

    /// <summary>
    /// Collection of images contained in this FSH.
    /// </summary>
    public Dictionary<string, Gimx> Images { get; } = [];

    private static int GetFileSize(Dictionary<string, Gimx> directory)
    {
        var sum = 16;
        foreach (var j in directory)
        {
            sum += 24 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        return sum;
    }

    public void WriteTo(Stream stream)
    {
        using BinaryWriter writer = new(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(Images));
        writer.Write(Images.Count);
        writer.Write(DirId);
        int o = Images.Count * 8 + 16;
        foreach (var j in Images)
        {
            writer.Write(ASCII.GetBytes(j.Key));
            writer.Write(o);
            o += 16 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        foreach (var j in Images.Values)
        {
            writer.Write(j.Magic);
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
}
