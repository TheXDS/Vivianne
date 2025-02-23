﻿using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs3;

/// <summary>
/// Implements a serializer that can read and write FCE3 files.
/// </summary>
public partial class FceSerializer : ISerializer<FceFile>
{
    /// <inheritdoc/>
    public FceFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<Fce3FileHeader>();
        var vertices = br.MarshalReadArray<Vector3d>(header.VertexTblOffset + DataOffset, header.Vertices);
        var normals = br.MarshalReadArray<Vector3d>(header.NormalsTblOffset + DataOffset, header.Vertices);
        var triangles = br.MarshalReadArray<FceTriangle>(header.TriangleTblOffset + DataOffset, header.Triangles);
        var data = new FceData(header, vertices, normals, triangles);
        return new FceFile()
        {
            Unk_0x0 = header.Unk_0x0,
            Arts = header.Arts,
            XHalfSize = header.XHalfSize,
            YHalfSize = header.YHalfSize,
            ZHalfSize = header.ZHalfSize,
            RsvdTable1 = br.ReadBytesAt(header.Rsvd1Offset + DataOffset, header.Vertices * 32),
            RsvdTable2 = br.ReadBytesAt(header.Rsvd2Offset + DataOffset, header.Vertices * 12),
            RsvdTable3 = br.ReadBytesAt(header.Rsvd3Offset + DataOffset, header.Vertices * 12),
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
        Fce3FileHeader header = CreateHeader(fce);
        List<int> vertexOffsets = [];
        foreach (var j in fce.Parts.Select((FcePart p) => p.Vertices))
        {
            vertexOffsets.Add((int)poolStream.Position);
            pool.MarshalWriteStructArray(j);
        }
        header.PartVertexOffset = ArrayOfSize(vertexOffsets, 64);
        header.NormalsTblOffset = (int)poolStream.Position;
        foreach (var j in fce.Parts.Select((FcePart p) => p.Normals))
        {
            pool.MarshalWriteStructArray(j);
        }
        header.TriangleTblOffset = (int)poolStream.Position;
        List<int> triangleOffsets = [0];
        foreach (var j in fce.Parts.Select((FcePart p) => p.Triangles))
        {
            triangleOffsets.Add(triangleOffsets.Last() + (pool.MarshalWriteStructArray(j) - 1));
        }
        header.PartTriangleOffset = ArrayOfSize(triangleOffsets, 64);
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
