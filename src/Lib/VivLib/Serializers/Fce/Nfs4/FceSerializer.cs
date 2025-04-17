using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs4;

/// <summary>
/// Implements a serializer that can read and write FCE4 files.
/// </summary>
public partial class FceSerializer : ISerializer<FceFile>, IOutSerializer<IFceFile<FcePart>>
{
    /// <inheritdoc />
    public FceFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<FceFileHeader>();
        var vertices = br.MarshalReadArray<Vector3>(header.VertexTblOffset + DataOffset, header.Vertices);
        var damagedVertices = br.MarshalReadArray<Vector3>(header.DamagedVertexTblOffset + DataOffset, header.Vertices);
        var normals = br.MarshalReadArray<Vector3>(header.NormalsTblOffset + DataOffset, header.Vertices);
        var damagedNormals = br.MarshalReadArray<Vector3>(header.DamagedNormalsTblOffset + DataOffset, header.Vertices);
        var triangles = br.MarshalReadArray<FceTriangle>(header.TriangleTblOffset + DataOffset, header.Triangles);
        var data = new FceData(header, vertices, damagedVertices, normals, damagedNormals, triangles);

        var fce = new FceFile()
        {
            Magic = header.Magic,
            Unk_0x0004 = header.Unk_0x4,
            Arts = header.Arts,
            XHalfSize = header.XHalfSize,
            YHalfSize = header.YHalfSize,
            ZHalfSize = header.ZHalfSize,
            RsvdTable1 = TryReadBytesAt(br, header.Rsvd1Offset, header.Vertices * 32, nameof(FceFile.RsvdTable1)),
            RsvdTable2 = TryReadBytesAt(br, header.Rsvd2Offset, header.Vertices * Marshal.SizeOf<Vector3>(), nameof(FceFile.RsvdTable2)),
            RsvdTable3 = TryReadBytesAt(br, header.Rsvd3Offset, header.Vertices * Marshal.SizeOf<Vector3>(), nameof(FceFile.RsvdTable3)),
            RsvdTable4 = TryReadBytesAt(br, header.Rsvd4Offset, header.Vertices * 4, nameof(FceFile.RsvdTable4)),
            RsvdTable5 = TryReadBytesAt(br, header.Rsvd5Offset, header.Vertices * 4, nameof(FceFile.RsvdTable5)),
            RsvdTable6 = TryReadBytesAt(br, header.Rsvd6Offset, header.Triangles * 12, nameof(FceFile.RsvdTable6)),
            AnimationTable = TryReadBytesAt(br, header.AnimationTblOffset, header.Vertices * 4, nameof(FceFile.AnimationTable)),
            PrimaryColors = [.. header.PrimaryColorTable.Take(header.Colors)],
            InteriorColors = [.. header.InteriorColorTable.Take(header.Colors)],
            SecondaryColors = [.. header.SecondaryColorTable.Take(header.Colors)],
            DriverHairColors = [.. header.DriverColorTable.Take(header.Colors)],
            Parts = [.. GetParts(data)],
            Dummies = [.. GetDummies(header)],
            Unk_0x0924 = header.Unk_0x0924,
            Unk_0x0928 = header.Unk_0x0928,
            Unk_0x1e28 = header.Unk_0x1e28,
        };

        return fce;
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


        header.NormalsTblOffset = header.UndamagedNormalsTblOffset = (int)poolStream.Position;
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
        header.DamagedVertexTblOffset = (int)poolStream.Position;
        foreach (var j in fce.Parts.Select(p => p.DamagedVertices))
        {
            pool.MarshalWriteStructArray(j);
        }
        header.DamagedNormalsTblOffset = (int)poolStream.Position;
        foreach (var j in fce.Parts.Select(p => p.DamagedNormals))
        {
            pool.MarshalWriteStructArray(j);
        }
        header.Rsvd4Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable4);
        header.AnimationTblOffset = (int)poolStream.Position;
        pool.Write(fce.AnimationTable);
        header.Rsvd5Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable5);
        header.Rsvd6Offset = (int)poolStream.Position;
        pool.Write(fce.RsvdTable6);
        using BinaryWriter fileWriter = new(stream);
        fileWriter.MarshalWriteStruct(header);
        fileWriter.Write(poolStream.ToArray());
    }

    IFceFile<FcePart> IOutSerializer<IFceFile<FcePart>>.Deserialize(Stream stream)
    {
        return Deserialize(stream);
    }
}
