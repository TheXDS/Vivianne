using System.Text;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Containers;

/// <summary>
/// Class that represents a VIV file that can be edited in memory.
/// </summary>
public class VivFile
{
    private static readonly byte[] Header = "BIGF"u8.ToArray();

    /// <summary>
    /// Reads the contents of the VIV file contained in the specified stream.
    /// </summary>
    /// <param name="stream">
    /// Stream from where to read the VIV file contents.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="VivFile"/> class, with all contents of
    /// the VIV file loaded in the directory.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Thrown if the stream does not contain a valid VIV header.
    /// </exception>
    public static VivFile ReadFrom(Stream stream)
    {
        var viv = new VivFile();
        using var reader = new BinaryReader(stream);
        if (!reader.ReadBytes(4).SequenceEqual(Header))
        {
            throw new InvalidDataException("Invalid header.");
        }
        var vivLength = reader.ReadInt32().FlipEndianness();
        if (stream.CanSeek && stream.Length != vivLength)
        {
            throw new InvalidDataException("VIV file length mismatch");
        }
        var entries = reader.ReadInt32().FlipEndianness();
        var blobPool = reader.ReadInt32().FlipEndianness();
        Dictionary<string, (int offset, int length)> fileOffsets = [];
        while (entries-- > 0)
        {
            var offset = reader.ReadInt32().FlipEndianness();
            var length = reader.ReadInt32().FlipEndianness();
            var name = reader.ReadNullTerminatedString();
            fileOffsets.Add(name, (offset, length));
        }
        if (stream.CanSeek && stream.Position != blobPool)
        {
            throw new InvalidDataException("Blob pool location mismatch");
        }
        foreach (var j in fileOffsets.OrderBy(p => p.Value.offset))
        {
            stream.Seek(j.Value.offset, SeekOrigin.Begin);
            viv.Directory.Add(j.Key, reader.ReadBytes(j.Value.length));
        }
        return viv;
    }

    /// <summary>
    /// Collection of files contained in this VIV.
    /// </summary>
    public Dictionary<string, byte[]> Directory { get; } = [];

    /// <summary>
    /// Gets the calculated VIV file size.
    /// </summary>
    public int FileSize => GetFileSize(Directory);

    /// <summary>
    /// Writes the data in this instance as a VIV file.
    /// </summary>
    /// <param name="stream">Stream where to write the VIV file to.</param>
    public void WriteTo(Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(Directory).FlipEndianness());
        writer.Write(Directory.Count.FlipEndianness());
        var o = GetDirectoryOffset(Directory);
        writer.Write(o.FlipEndianness());
        foreach (var j in Directory)
        {
            writer.Write(o.FlipEndianness());
            writer.Write(j.Value.Length.FlipEndianness());
            writer.Write(Encoding.ASCII.GetBytes(j.Key));
            writer.Write((byte)0);
            o += j.Value.Length;
        }

        foreach (var j in Directory)
        {
            writer.Write(j.Value);
        }
    }

    private static int GetFileSize(Dictionary<string, byte[]> directory)
    {
        var sum = 16;
        foreach (var j in directory)
        {
            sum += j.Key.Length + 9 + j.Value.Length;
        }
        return sum;
    }

    private static int GetDirectoryOffset(Dictionary<string, byte[]> directory)
    {
        var sum = 16;
        foreach (var j in directory)
        {
            sum += j.Key.Length + 9;
        }
        return sum;
    }
}
