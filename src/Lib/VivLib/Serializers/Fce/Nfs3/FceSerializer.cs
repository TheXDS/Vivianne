using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs3;

/// <summary>
/// Implements a serializer that can read and write FCE3 files.
/// </summary>
public partial class FceSerializer : ISerializer<FceFile>
{
    /// <summary>
    /// Tries to read an FCEv3 file.
    /// </summary>
    /// <param name="bytes">Byte array from which to read the Fce file.</param>
    /// <returns>
    /// An FCE file conforming to the expected file format for NFS3, or
    /// <see langword="null"/> if a valid FCEv3 file cannot be read from the
    /// specified bytes.
    /// </returns>
    public FceFile? TryGetFce(byte[]? bytes)
    {
        try
        {
            if (bytes is null || bytes.Length < Marshal.SizeOf<FceFileHeader>()) return null;
            var file = ((ISerializer<FceFile>)this).Deserialize(bytes);
            return file.PrimaryColors.Count <= 16
                && file.SecondaryColors.Count <= 16
                && file.Parts.Count <= 64
                && file.Dummies.Count <= 16 ? file : null;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public FceFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<FceFileHeader>();
        var vertices = br.MarshalReadArray<Vector3>(header.VertexTblOffset + DataOffset, header.Vertices);
        var normals = br.MarshalReadArray<Vector3>(header.NormalsTblOffset + DataOffset, header.Vertices);
        var triangles = br.MarshalReadArray<FceTriangle>(header.TriangleTblOffset + DataOffset, header.Triangles);
        var data = new FceData(header, vertices, normals, triangles);
        return new FceFile()
        {
            Magic = header.Magic,
            Arts = header.Arts,
            XHalfSize = header.XHalfSize,
            YHalfSize = header.YHalfSize,
            ZHalfSize = header.ZHalfSize,
            RsvdTable1 = br.ReadBytesAt(header.Rsvd1Offset + DataOffset, header.Vertices * 32),
            RsvdTable2 = br.ReadBytesAt(header.Rsvd2Offset + DataOffset, header.Vertices * Marshal.SizeOf<Vector3>()),
            RsvdTable3 = br.ReadBytesAt(header.Rsvd3Offset + DataOffset, header.Vertices * Marshal.SizeOf<Vector3>()),
            PrimaryColors = [.. header.PrimaryColorTable.Take(header.PrimaryColors)],
            SecondaryColors = [.. header.SecondaryColorTable.Take(header.SecondaryColors)],
            Parts = [.. GetParts(data)],
            Dummies = [.. GetDummies(data)],
            Unk_0x1e04 = header.Unk_0x1e04
        };
    }

    /// <inheritdoc />
    public void SerializeTo(FceFile fce, Stream stream)
    {
        using MemoryStream poolStream = new();
        using BinaryWriter pool = new(poolStream);
        FceFileHeader header = CreateHeader(fce);
        List<int> vertexOffsets = [];
        foreach (var j in fce.Parts.Select(p => p.Vertices))
        {
            vertexOffsets.Add((int)poolStream.Position / Marshal.SizeOf<Vector3>());
            pool.MarshalWriteStructArray(j);
        }
        header.PartVertexOffset = vertexOffsets.ArrayOfSize(64);
        header.NormalsTblOffset = (int)poolStream.Position;
        foreach (var j in fce.Parts.Select(p => p.Normals))
        {
            pool.MarshalWriteStructArray(j);
        }
        header.TriangleTblOffset = (int)poolStream.Position;
        List<int> triangleOffsets = [0];
        foreach (var j in fce.Parts.Select(p => p.Triangles))
        {
            triangleOffsets.Add(triangleOffsets.Last() + ((pool.MarshalWriteStructArray(j)) / Marshal.SizeOf<FceTriangle>()));
        }
        header.PartTriangleOffset = triangleOffsets[..^1].ArrayOfSize(64);
        header.Rsvd1Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable1);
        header.Rsvd2Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable2);
        header.Rsvd3Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable3);
        using BinaryWriter fileWriter = new(stream);
        fileWriter.MarshalWriteStruct(header);
        fileWriter.Write(poolStream.ToArray());
    }
}
