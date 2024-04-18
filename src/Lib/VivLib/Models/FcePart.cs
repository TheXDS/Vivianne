using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

[StructLayout(LayoutKind.Sequential)]
public struct FceFileHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] Magic;
    public int Triangles;
    public int Vertices;
    public int Arts;
    public int VertexTblOffset;
    public int NormalsTblOffset;
    public int TriangleTblOffset;
    public int Rsvd1Offset;
    public int Rsvd2Offset;
    public int Rsvd3Offset;
    public float XHalfSize;
    public float YHalfSize;
    public float ZHalfSize;
    public int DummyCount; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public Vector3d[] Dummies;
    public int CarParts; // Up to 64
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public Vector3d[] CarPartsCoords;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexOffset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartVertexCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleOffset;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] PartTriangleCount;
    public int PrimaryColors; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] PrimaryColorTable;
    public int SecondaryColors; // Up to 16
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceColor[] SecondaryColorTable;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public FceAsciiBlob[] DummyNames;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public FceAsciiBlob[] PartNames;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public int[] Unk_0x1e04;
}

[StructLayout(LayoutKind.Sequential)]
public struct FceAsciiBlob
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] Value;

    public string ToString()
    {
        var end = Value.FindIndexOf<byte>(0);
        if (end == -1) end = 64;
        return Encoding.Latin1.GetString(Value[0..end]);
    }

    public static implicit operator string(FceAsciiBlob x) => x.ToString();
}

[StructLayout(LayoutKind.Sequential)]
public record struct Vector3d(float X, float Y, float Z);

[StructLayout(LayoutKind.Sequential)]
public record struct FceColor(int Hue, int Saturation, int Brightness, int Transparency);

[StructLayout(LayoutKind.Sequential)]
public struct Triangle
{
    public int TexturePage;
    public int I1;
    public int I2;
    public int I3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public short[] Unk_0x10;
    public int Smoothing;
    public float U1;
    public float U2;
    public float U3;
    public float V1;
    public float V2;
    public float V3;
}

public class FceFile : IReadOnlyDictionary<string, FcePart>
{
    public FceFileHeader Header { get; set; }
    public Vector3d[] Vertices { get; set; }
    public Vector3d[] Normals { get; set; }
    public Triangle[] Triangles { get; set; }
    public byte[] RsvdTable1 { get; set; }
    public byte[] RsvdTable2 { get; set; }
    public byte[] RsvdTable3 { get; set; }

    public IEnumerable<string> Keys => Header.PartNames.Select(p => p.ToString()).Where(p => !string.IsNullOrWhiteSpace(p));

    public IEnumerable<FcePart> Values => Enumerable.Range(0, Header.CarParts).Select(p => this[p]);

    public int Count => Header.CarParts;

    public FcePart? this[string partName]
    {
        get
        {
            var index = Header.PartNames.Select(p => p.ToString()).FindIndexOf(partName);
            return index != -1 ? this[index] : null;
        }
    }

    public FcePart this[int index] => new()
    {
        Origin = Header.CarPartsCoords[index],
        Vertices = Vertices[Header.PartVertexOffset[index]..(Header.PartVertexOffset[index] + Header.PartVertexCount[index])],
        Normals = Normals[Header.PartVertexOffset[index]..(Header.PartVertexOffset[index] + Header.PartVertexCount[index])],
        Triangles = Triangles[Header.PartTriangleOffset[index]..(Header.PartTriangleOffset[index] + Header.PartTriangleCount[index])]
    };

    private IEnumerable<KeyValuePair<string, FcePart>> GetParts()
    {
        return Header.PartNames.Where(p => !string.IsNullOrWhiteSpace(p.ToString())).Select<FceAsciiBlob, KeyValuePair<string, FcePart>>(p => new(p.ToString(), this[p]!));
    }

    public bool ContainsKey(string key) => Keys.Contains(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out FcePart value)
    {
        var index = Header.PartNames.Select(p => p.ToString()).FindIndexOf(key);
        value = index != -1 ? this[index] : null;
        return index != -1;
    }

    public IEnumerator<KeyValuePair<string, FcePart>> GetEnumerator() => GetParts().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class FcePart
{
    public Vector3d Origin { get; set; }
    public Vector3d[] Vertices { get; set; }
    public Vector3d[] Normals { get; set; }
    public Triangle[] Triangles { get; set; }
}
