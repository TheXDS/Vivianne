using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs3;

public partial class FceSerializer
{
    private const int DataOffset = 0x1f04; //sizeof(Fce3FileHeader)

    private static FceFileHeader CreateHeader(FceFile entity)
    {
        return new FceFileHeader()
        {
            Unk_0x0 = entity.Unk_0x0,
            Triangles = entity.Parts.Sum(p => p.Triangles.Length),
            Vertices = entity.Parts.Sum(p => p.Vertices.Length),
            Arts = entity.Arts,
            VertexTblOffset = 0x0,
            XHalfSize = entity.XHalfSize,
            YHalfSize = entity.YHalfSize,
            ZHalfSize = entity.ZHalfSize,
            DummyCount = entity.Dummies.Count,
            Dummies = ArrayOfSize(entity.Dummies.Select(p => p.Position), 16),
            CarPartCount = entity.Parts.Count,
            CarPartsCoords = ArrayOfSize(entity.Parts.Select(p => p.Origin), 64),
            PartVertexCount = ArrayOfSize(entity.Parts.Select(p => p.Vertices.Length), 64),
            PartTriangleCount = ArrayOfSize(entity.Parts.Select(p => p.Triangles.Length), 64),
            PrimaryColors = entity.PrimaryColors.Count,
            PrimaryColorTable = ArrayOfSize([.. entity.PrimaryColors], 16),
            SecondaryColors = entity.SecondaryColors.Count,
            SecondaryColorTable = ArrayOfSize([.. entity.SecondaryColors], 16),
            DummyNames = ArrayOfSize(entity.Dummies.Select(p => (FceAsciiBlob)p.Name), 16, FceAsciiBlob.Empty),
            PartNames = ArrayOfSize(entity.Parts.Select(p => (FceAsciiBlob)p.Name), 64, FceAsciiBlob.Empty),
            Unk_0x1e04 = ArrayOfSize(entity.Unk_0x1e04, 256)
        };
    }

    private static T[] ArrayOfSize<T>(IEnumerable<T> collection, int size, T defaultValue = default) where T : struct
    {
        var arr = collection.ToArray();
        var appendSize = size - arr.Length;
        return appendSize <= size
            ? [.. arr, .. Enumerable.Repeat(defaultValue, appendSize)]
            : [.. arr.Take(size)];
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
