using System.Diagnostics;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Helpers;
using TheXDS.Vivianne.Models.Fsh;
using static System.Text.Encoding;
using St = TheXDS.Vivianne.Resources.Strings.Serializers.FshSerializer;

namespace TheXDS.Vivianne.Serializers.Fsh;

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

    private FshFile DeserializeQfs(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var x = ((ISerializer<FshFile>)this).Deserialize(QfsCodec.Decompress(ms.ToArray()));
        x.IsCompressed = true;
        return x;
    }

    /// <inheritdoc/>
    public FshFile Deserialize(Stream stream)
    {
        if (QfsCodec.IsCompressed(stream)) return DeserializeQfs(stream);

        using var reader = new BinaryReader(stream);
        VerifyValidFsh(reader);
        var entries = reader.ReadInt32();
        var dirId = reader.ReadBytes(4);
        if (!DirIds.Any(dirId.SequenceEqual))
        {
            throw new InvalidDataException(St.InvalidDirectoryID);
        }
        Dictionary<string, int> fileOffsets = [];
        while (entries-- > 0)
        {
            var name = Latin1.GetString(reader.ReadBytes(4));
            var offset = reader.ReadInt32();

            if (!fileOffsets.TryAdd(name, offset))
            {
                Debug.Print(string.Format(St.DuplicatedFshEntityEntry, name, offset));
            }
        }

        var fsh = new FshFile() { DirectoryId = Latin1.GetString(dirId) };
        var fshBlobSerializer = new FshBlobSerializer();
        foreach (var j in fileOffsets)
        {
            reader.BaseStream.Seek(j.Value, SeekOrigin.Begin);
            var endOffset = fileOffsets.Values.Cast<int?>().Order().FirstOrDefault(p => p > j.Value) ?? (int)reader.BaseStream.Length;
            using var ms = new MemoryStream(reader.ReadBytes(endOffset - j.Value));
            if (fshBlobSerializer.Deserialize(ms) is { } blob)
            {
                fsh.Entries.Add(j.Key, blob);
            }
#if DEBUG
            else
            {
                Debug.Print(string.Format(St.CouldNotReadX, j.Key));
            }
#endif
        }
        return fsh;
    }

    /// <inheritdoc/>
    public void SerializeTo(FshFile entity, Stream stream)
    {
        if (entity.IsCompressed)
        {
            using var ms = new MemoryStream();
            entity.IsCompressed = false;
            SerializeTo(entity, ms);
            entity.IsCompressed = true;
            stream.WriteBytes(QfsCodec.Compress(ms.ToArray()));
            return;
        }
        using BinaryWriter writer = new(stream);
        writer.Write(Header);
        writer.Write(GetFileSize(entity.Entries));
        writer.Write(entity.Entries.Count);
        writer.Write(Latin1.GetBytes(entity.DirectoryId));
        int o = entity.Entries.Count * 8 + 16;
        foreach (var j in entity.Entries)
        {
            writer.Write(Latin1.GetBytes(j.Key));
            writer.Write(o);
            // 16 = blob header (16 bytes)
            o += 16 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        ISerializer<FshBlob?> fshBlobSerializer = new FshBlobSerializer();
        foreach (var j in entity.Entries.Values)
        {
            writer.Write(fshBlobSerializer.Serialize(j));
        }
    }

    private static void VerifyValidFsh(BinaryReader reader)
    {
        if (!reader.ReadBytes(4).SequenceEqual(Header))
        {
            throw new InvalidDataException(St.InvalidHeader);
        }
        var fshLength = reader.ReadInt32();
        if (reader.BaseStream.CanSeek && reader.BaseStream.Length != fshLength)
        {
            throw new InvalidDataException(St.FshFileLengthMismatch);
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
}
