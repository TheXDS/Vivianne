using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs3;

public partial class FceSerializer
{
    private const int DataOffset = 0x1f04; //sizeof(Fce3FileHeader)

    private static FceFileHeader CreateHeader(FceFile entity)
    {
        return new FceFileHeader()
        {
            Magic = entity.Magic,
            Triangles = entity.Parts.Sum(p => p.Triangles.Length),
            Vertices = entity.Parts.Sum(p => p.Vertices.Length),
            Arts = entity.Arts,
            VertexTblOffset = 0x0,
            XHalfSize = entity.XHalfSize,
            YHalfSize = entity.YHalfSize,
            ZHalfSize = entity.ZHalfSize,
            DummyCount = entity.Dummies.Count,
            Dummies = entity.Dummies.Select(p => p.Position).ArrayOfSize(16),
            CarPartCount = entity.Parts.Count,
            CarPartsCoords = entity.Parts.Select(p => p.Origin).ArrayOfSize(64),
            PartVertexCount = entity.Parts.Select(p => p.Vertices.Length).ArrayOfSize(64),
            PartTriangleCount = entity.Parts.Select(p => p.Triangles.Length).ArrayOfSize(64),
            PrimaryColors = entity.PrimaryColors.Count,
            PrimaryColorTable = entity.PrimaryColors.ArrayOfSize(16),
            SecondaryColors = entity.SecondaryColors.Count,
            SecondaryColorTable = entity.SecondaryColors.ArrayOfSize(16),
            DummyNames = entity.Dummies.Select(p => (FceAsciiBlob)p.Name).ArrayOfSize(16, FceAsciiBlob.Empty),
            PartNames = entity.Parts.Select(p => (FceAsciiBlob)p.Name).ArrayOfSize(64, FceAsciiBlob.Empty),
            Unk_0x1e04 = entity.Unk_0x1e04.ArrayOfSize(256)
        };
    }

    private static IEnumerable<FcePart> GetParts(FceData data)
    {
        return Enumerable.Range(0, data.Header.CarPartCount).Select(p => LoadPart(data, p));
    }

    private static FcePart LoadPart(FceData data, int index)
    {
        return index < data.Header.CarPartCount ? new()
        {
            Name = data.Header.PartNames[index],
            Origin = data.Header.CarPartsCoords[index],
            Vertices = data.Vertices[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            Normals = data.Normals[data.Header.PartVertexOffset[index]..(data.Header.PartVertexOffset[index] + data.Header.PartVertexCount[index])],
            Triangles = data.Triangles[data.Header.PartTriangleOffset[index]..(data.Header.PartTriangleOffset[index] + data.Header.PartTriangleCount[index])]
        } : throw new IndexOutOfRangeException();
    }

    private static IEnumerable<FceDummy> GetDummies(FceData data)
    {
        return data.Header.Dummies
            .Zip(data.Header.DummyNames)
            .Take(data.Header.DummyCount)
            .Select(p => new FceDummy() { Name = p.Second, Position = p.First });
    }
}
