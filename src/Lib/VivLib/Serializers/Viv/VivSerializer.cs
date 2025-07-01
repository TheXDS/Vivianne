using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Viv;
using static System.Text.Encoding;
using St = TheXDS.Vivianne.Resources.Strings.Serializers.VivSerializer;

namespace TheXDS.Vivianne.Serializers.Viv;

/// <summary>
/// Implements a serializer that can read and write <see cref="VivFile"/>
/// entities.
/// </summary>
public partial class VivSerializer : ISerializer<VivFile>
{
    /// <summary>
    /// Gets or sets the directory sorting algorithm to use.
    /// </summary>
    public Func<SortType>? Sort { get; set; }

    /// <inheritdoc/>
    public VivFile Deserialize(Stream stream)
    {
        var viv = new VivFile();
        using var reader = new BinaryReader(stream);
        if (!reader.ReadBytes(4).SequenceEqual(Header))
        {
            throw new InvalidDataException(St.InvalidHeader);
        }
        var vivLength = reader.ReadInt32().FlipEndianness();
        if (stream.CanSeek && stream.Length != vivLength)
        {
            //throw new InvalidDataException(St.VivFileLengthMismatch);
        }
        var entries = reader.ReadInt32().FlipEndianness();
        var blobPool = reader.ReadInt32().FlipEndianness();
        Dictionary<string, (int offset, int length)> fileOffsets = [];
        while (entries-- > 0)
        {
            var offset = reader.ReadInt32().FlipEndianness();
            var length = reader.ReadInt32().FlipEndianness();
            var name = reader.ReadNullTerminatedString();
            if (!fileOffsets.TryAdd(name, (offset, length)))
            {
                var rename = 1;
                string newName;
                do
                {
                    newName = $"{Path.GetFileNameWithoutExtension(name)} ({rename++}){Path.GetExtension(name)}";
                } while (fileOffsets.ContainsKey(newName));
                fileOffsets.Add(newName, (offset, length));
            }
        }
        if (stream.CanSeek && stream.Position != blobPool)
        {
            // TODO: Define actual course of action - Extra bytes after reading the header, but before the data pool (probably for alignment reasons?)
        }
        foreach (var j in ApplySort(fileOffsets))
        {
            stream.Seek(j.Value.offset, SeekOrigin.Begin);
            viv.Directory.Add(j.Key, reader.ReadBytes(j.Value.length));
        }
        return viv;
    }

    /// <inheritdoc/>
    public void SerializeTo(VivFile entity, Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(entity.Directory).FlipEndianness());
        writer.Write(entity.Directory.Count.FlipEndianness());
        var o = GetDirectoryOffset(entity.Directory);
        writer.Write(o.FlipEndianness());
        foreach (var j in entity.Directory)
        {
            writer.Write(o.FlipEndianness());
            writer.Write(j.Value.Length.FlipEndianness());
            writer.Write(ASCII.GetBytes(DedupName(j.Key)));
            writer.Write((byte)0);
            o += j.Value.Length;
        }

        foreach (var j in entity.Directory)
        {
            writer.Write(j.Value);
        }
    }

    /// <summary>
    /// Infers the file size in bytes by adding up the size of all blobs and the resulting header.
    /// </summary>
    /// <param name="directory">Directory of the VIV file.</param>
    /// <returns>The estimated file size.</returns>
    public static int GetFileSize(Dictionary<string, byte[]> directory)
    {
        return GetFileSize(directory.Select(p => new KeyValuePair<string, int>(p.Key, p.Value.Length)));
    }

    /// <summary>
    /// Infers the file size in bytes by adding up the size of all blobs and the resulting header.
    /// </summary>
    /// <param name="directory">Directory of the VIV file.</param>
    /// <returns>The estimated file size.</returns>
    public static int GetFileSize(IEnumerable<KeyValuePair<string, int>> directory)
    {
        var sum = 16;
        foreach (var j in directory)
        {
            sum += j.Key.Length + 9 + j.Value;
        }
        return sum;
    }
}
