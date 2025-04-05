using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs4;

/// <summary>
/// Implements a serializer that can read and write FCE4 files.
/// </summary>
public class FceSerializer : ISerializer<FceFile>
{
    private static readonly int DataOffset = Marshal.SizeOf<FceFileHeader>();

    /// <inheritdoc />
    public FceFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream);
        var header = br.MarshalReadStruct<FceFileHeader>();
        var vertices = br.MarshalReadArray<Vector3d>(header.VertexTblOffset + DataOffset, header.Vertices);
        var damagedVertices = br.MarshalReadArray<Vector3d>(header.DamagedVertexTblOffset + DataOffset, header.Vertices);
        var normals = br.MarshalReadArray<Vector3d>(header.NormalsTblOffset + DataOffset, header.Vertices);
        var damagedNormals = br.MarshalReadArray<Vector3d>(header.DamagedNormalsTblOffset + DataOffset, header.Vertices);
        var triangles = br.MarshalReadArray<FceTriangle>(header.TriangleTblOffset + DataOffset, header.Triangles);
        var data = new FceData(header, vertices, damagedVertices, normals, damagedNormals, triangles);
        return new FceFile()
        {
            Magic = header.Magic,
            Unk_0x0004 = header.Unk_0x4,
            Arts = header.Arts,
            XHalfSize = header.XHalfSize,
            YHalfSize = header.YHalfSize,
            ZHalfSize = header.ZHalfSize,
            RsvdTable1 = br.ReadBytesAt(header.Rsvd1Offset + DataOffset, header.Vertices * 32),
            RsvdTable2 = br.ReadBytesAt(header.Rsvd2Offset + DataOffset, header.Vertices * Vector3d.SizeOf),
            RsvdTable3 = br.ReadBytesAt(header.Rsvd3Offset + DataOffset, header.Vertices * Vector3d.SizeOf),
            RsvdTable4 = br.ReadBytesAt(header.Rsvd4Offset + DataOffset, header.Vertices * 4),
            RsvdTable5 = br.ReadBytesAt(header.Rsvd5Offset + DataOffset, header.Vertices * 4),
            RsvdTable6 = br.ReadBytesAt(header.Rsvd6Offset + DataOffset, header.Triangles * 12),
            AnimationTable = br.ReadBytesAt(header.AnimationTblOffset + DataOffset, header.Vertices * 4),
            PrimaryColors = [.. header.PrimaryColorTable.Take(header.Colors)],
            InteriorColors = [.. header.InteriorColorTable.Take(header.Colors)],
            SecondaryColors = [.. header.SecondaryColorTable.Take(header.Colors)],
            DriverHairColors = [.. header.DriverColorTable.Take(header.Colors)],
            Parts = [.. GetParts(data)],
            Dummies = [.. GetDummies(header)],
            Unk_0x0924 = header.Unk_0x0924,
            Unk_0x0928 = header.Unk_0x0928,
            Unk_0x1e28 = header.Unk_0x1e04,
        };
    }

    /// <inheritdoc />
    public void SerializeTo(FceFile entity, Stream stream)
    {
        throw new NotImplementedException();
    }


    private static FcePart LoadPart(FceData data, int index)
    {
        return index < data.Header.CarPartCount ? new()
        {
            Name = data.Header.PartNames[index],
            Origin = data.Header.CarPartsCoords[index],
            Vertices = data.Vertices[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            DamagedVertices = data.DamagedVertices[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            Normals = data.Normals[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            DamagedNormals = data.DamagedNormals[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            Triangles = data.Triangles[data.Header.PartTriangleOffset[index]..(data.Header.PartTriangleOffset[index] + data.Header.PartTriangleCount[index])]
        } : throw new IndexOutOfRangeException();
    }

    private static IEnumerable<FceDummy> GetDummies(FceFileHeader header)
    {
        return header.Dummies
            .Zip(header.DummyNames)
            .Take(header.DummyCount)
            .Select(p => new FceDummy() { Name = p.Second, Position = p.First });
    }

    private static IEnumerable<FcePart> GetParts(FceData data)
    {
        return Enumerable.Range(0, data.Header.CarPartCount).Select(p => LoadPart(data, p));
    }

    private readonly record struct FceData(in FceFileHeader Header, in Vector3d[] Vertices, in Vector3d[] DamagedVertices, in Vector3d[] Normals, in Vector3d[] DamagedNormals, in FceTriangle[] Triangles);
}
