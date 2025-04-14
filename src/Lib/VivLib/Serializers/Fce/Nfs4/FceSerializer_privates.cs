using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.Serializers.Fce.Nfs4;

public partial class FceSerializer
{
    private static readonly int DataOffset = Marshal.SizeOf<FceFileHeader>();

    private static FceFileHeader CreateHeader(FceFile entity)
    {
        return new FceFileHeader()
        {
            Magic = entity.Magic,
            Unk_0x4 = entity.Unk_0x0004,
            Triangles = entity.Parts.Sum(p => p.Triangles.Length),
            Vertices = entity.Parts.Sum(p => p.Vertices.Length),
            Arts = entity.Arts,
            VertexTblOffset = 0x0,
            UndamagedVertexTblOffset = 0x0,
            XHalfSize = entity.XHalfSize,
            YHalfSize = entity.YHalfSize,
            ZHalfSize = entity.ZHalfSize,
            DummyCount = entity.Dummies.Count,
            Dummies = entity.Dummies.Select(p => p.Position).ArrayOfSize(16),
            CarPartCount = entity.Parts.Count,
            CarPartsCoords = entity.Parts.Select(p => p.Origin).ArrayOfSize(64),
            PartVertexCount = entity.Parts.Select(p => p.Vertices.Length).ArrayOfSize(64),
            PartTriangleCount = entity.Parts.Select(p => p.Triangles.Length).ArrayOfSize(64),
            Colors = entity.PrimaryColors.Count,
            PrimaryColorTable = entity.PrimaryColors.ArrayOfSize(16),
            InteriorColorTable = entity.InteriorColors.ArrayOfSize(16),
            SecondaryColorTable = entity.SecondaryColors.ArrayOfSize(16),
            DriverColorTable = entity.DriverHairColors.ArrayOfSize(16),
            Unk_0x0924 = entity.Unk_0x0924,
            Unk_0x0928 = entity.Unk_0x0928.ArrayOfSize(256),
            DummyNames = entity.Dummies.Select(p => (FceAsciiBlob)p.Name).ArrayOfSize(16, FceAsciiBlob.Empty),
            PartNames = entity.Parts.Select(p => (FceAsciiBlob)p.Name).ArrayOfSize(64, FceAsciiBlob.Empty),
            Unk_0x1e28 = entity.Unk_0x1e28.ArrayOfSize(528)
        };
    }

    private static Fce4Part LoadPart(FceData data, int index)
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

    private static IEnumerable<Fce4Part> GetParts(FceData data)
    {
        return Enumerable.Range(0, data.Header.CarPartCount).Select(p => LoadPart(data, p));
    }

    private static byte[] TryReadBytesAt(in BinaryReader br, in long offset, in int count, string tableName)
    {
        try
        {
            return br.ReadBytesAt(offset + DataOffset, count);
        }
#if DEBUG
        catch (Exception ex)
        {
            Debug.Print($"Could not read {tableName} (asked for {count} bytes at offset 0x{offset:X8}). Skipping...");
            Debug.Print(TheXDS.MCART.Resources.Strings.Composition.ExDump(ex, MCART.Resources.Strings.ExDumpOptions.AllFormatted));
#else
        catch
        {
            Debug.Print($"Could not read {tableName} (asked for {count} bytes at offset 0x{offset:X8}). Skipping...");
#endif
            return new byte[count];
        }
    }
}
