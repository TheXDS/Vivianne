using System.Diagnostics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Codecs;
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FshHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Header;
        public int Length;
        public int Entries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] DirectoryId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct FshDirEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Name;
        public int Offset;

        public readonly (string Name, int Offset) Deconstruct()
        {
            return (Latin1.GetString(Name), Offset);
        }
    }

    private static readonly byte[] Header = "SHPI"u8.ToArray();

    private FshFile DeserializeQfs(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var x = ((ISerializer<FshFile>)this).Deserialize(RefPackCodec.Decompress(ms.ToArray()));
        x.IsCompressed = true;
        return x;
    }

    /// <inheritdoc/>
    public FshFile Deserialize(Stream stream)
    {
        if (RefPackCodec.IsCompressed(stream)) return DeserializeQfs(stream);
        using var reader = new BinaryReader(stream);

        var header = reader.MarshalReadStruct<FshHeader>();
        var entries = reader.MarshalReadArray<FshDirEntry>(header.Entries).OrderBy(p => p.Offset).ToArray();
        var fsh = new FshFile() { DirectoryId = Latin1.GetString(header.DirectoryId) };
        var firstOffset = entries.Select(p => p.Offset).FirstOrDefault();
        if (stream.CanSeek && stream.Position < firstOffset)
        {
            fsh.ExtraData = reader.ReadBytes((int)(firstOffset - stream.Position));
        }
        var fshBlobSerializer = new FshBlobSerializer();
        foreach ((string name, int offset) in entries.Select(p => p.Deconstruct()))
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            var endOffset = entries.Select(p => p.Offset).FirstOrDefault(p => p > offset, (int)reader.BaseStream.Length);
            using var ms = new MemoryStream(reader.ReadBytes(endOffset - offset));
            if (fshBlobSerializer.Deserialize(ms) is { } blob)
            {
                if (!fsh.Entries.TryAdd(name, blob))
                {
                    Debug.Print(string.Format(St.DuplicatedFshEntityEntry, name, offset));
                }
            }
            else
            {
                Debug.Print(string.Format(St.CouldNotReadX, name));
            }
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
            stream.WriteBytes(RefPackCodec.Compress(ms.ToArray()));
            return;
        }
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
            // 16 = blob header (16 bytes)
            o += 16 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        writer.Write(entity.ExtraData);
        ISerializer<FshBlob?> fshBlobSerializer = new FshBlobSerializer();
        foreach (var j in entity.Entries.Values)
        {
            writer.Write(fshBlobSerializer.Serialize(j));
        }
    }

    private static int GetFileSize(Dictionary<string, FshBlob> directory)
    {
        var sum = 16; // FSH header size
        foreach (var j in directory)
        {
            // 24 = Blob header (16 bytes) + entry name (4 bytes) + data offset (4 bytes)
            sum += 24 + j.Value.PixelData.Length + j.Value.Footer.Length;
        }
        return sum;
    }
}
