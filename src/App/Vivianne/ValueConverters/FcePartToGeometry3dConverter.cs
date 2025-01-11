using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ValueConverters.Base;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a value converter that converts a <see cref="RenderTreeState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class FcePreviewViewModelToModel3DGroupConverter : IOneWayValueConverter<RenderTreeState?, Model3DGroup?>
{
    private record VertexUv(Point3D Vertex, Point Uv, Vector3d Normal);
    private const double SizeFactor = 10.0;

    private static MeshGeometry3D ToGeometryOld(FcePart value, bool flipU, bool flipV)
    {
        /* This convoluted method has a reason for being
         * =============================================
         * 3D models using the FCE file format support per-triangle UV. That
         * means that the UV data can be defined for each individual triangle,
         * even if they share vertices. Modern rendering engines moved away
         * from that, and while they could still support it, this usually
         * requires some more direct manipulation of the 3D api in use
         * (DirectX).
         * 
         * This is a WPF app, and WPF itself includes a simple yet easy to use
         * 3D rendering engine, but it's limited to use per-vertex UV. This
         * means that in order to properly render FCE models that utilize
         * per-triangle UV mapping, we have to convert from per-triangle UV to
         * per-vertex UV format. This implies duplicating vertices and normals
         * just so we can pair that new vertex with its own UV data, and then
         * we remap the triangle to use the copy.
         * 
         * This is not a problem if the FCE file was created in such a way that
         * any triangle sharing vertices was made to use the exact same UV
         * coordinates for whichever shared vertices they have; in other words,
         * if the FCE model was created using any modern 3D modeling software
         * that uses per-vertex UV.
         */
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        const double epsilon = 0.0005;
        var workingCopy = new Triangle[value.Triangles.Length];
        for (int i = 0; i < value.Triangles.Length; i++)
        {
            var j = value.Triangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;
            var v1 = value.Vertices[j.I1];
            var v2 = value.Vertices[j.I2];
            var v3 = value.Vertices[j.I3];
            var vert1 = new Point3D(SizeFactor * (v1.Z + value.Origin.Z), SizeFactor * (-v1.X + -value.Origin.X), SizeFactor * (v1.Y + value.Origin.Y));
            var vert2 = new Point3D(SizeFactor * (v2.Z + value.Origin.Z), SizeFactor * (-v2.X + -value.Origin.X), SizeFactor * (v2.Y + value.Origin.Y));
            var vert3 = new Point3D(SizeFactor * (v3.Z + value.Origin.Z), SizeFactor * (-v3.X + -value.Origin.X), SizeFactor * (v3.Y + value.Origin.Y));
            var uv1 = new Point(uFlip * j.U1, vFlip * j.V1);
            var uv2 = new Point(uFlip * j.U2, vFlip * j.V2);
            var uv3 = new Point(uFlip * j.U3, vFlip * j.V3);
            if (vertex[j.I1] is { Uv: { } testUv1 } && (Math.Abs(testUv1.X - uv1.X) > epsilon || Math.Abs(testUv1.Y - uv1.Y) > epsilon))
            {
                vertex.Add(new(vert1, new Point(uv1.X, uv1.Y), value.Normals[j.I1]));
                j.I1 = vertex.Count - 1;
            }
            if (vertex[j.I2] is { Uv: { } testUv2 } && (Math.Abs(testUv2.X - uv2.X) > epsilon || Math.Abs(testUv2.Y - uv2.Y) > epsilon))
            {
                vertex.Add(new(vert2, new Point(uv2.X, uv2.Y), value.Normals[j.I2]));
                j.I2 = vertex.Count - 1;
            }
            if (vertex[j.I3] is { Uv: { } testUv3 } && (Math.Abs(testUv3.X - uv3.X) > epsilon || Math.Abs(testUv3.Y - uv3.Y) > epsilon))
            {
                vertex.Add(new(vert3, new Point(uv3.X, uv3.Y), value.Normals[j.I3]));
                j.I3 = vertex.Count - 1;
            }
            vertex[j.I1] = new(vert1, uv1, value.Normals[value.Triangles[i].I1]);
            vertex[j.I2] = new(vert2, uv2, value.Normals[value.Triangles[i].I2]);
            vertex[j.I3] = new(vert3, uv3, value.Normals[value.Triangles[i].I3]);
            workingCopy[i] = j;
        }
        return new MeshGeometry3D()
        {
            Positions = new Point3DCollection(vertex.Select(p => p?.Vertex ?? default)),
            TriangleIndices = new Int32Collection(workingCopy.SelectMany(p => (int[])[p.I1, p.I2, p.I3])),
            Normals = new Vector3DCollection(vertex.Select(p => p?.Normal ?? default).Select(p => new Vector3D(-p.Z, p.X, -p.Y))),
            TextureCoordinates = new PointCollection(vertex.Select(p => p?.Uv ?? default)),
        };
    }
    
    private static bool IsTextureLikelyTga(RenderTreeState state, [NotNullWhen(true)] out TargaHeader? header)
    {
        var bytes = state.Texture?.Take(18).ToArray() ?? throw new TamperException();
        if (bytes.Length == 18)
        {
            using var ms = new MemoryStream(bytes);
            using var br = new BinaryReader(ms);
            header = br.MarshalReadStruct<TargaHeader>();
        }
        else
        {
            header = null;
        }
        return header is not null;
    }

    private static (Brush? brush, bool flipU, bool flipV) CheckUvFlip(RenderTreeState value)
    {
        Brush? brush;
        bool flipU = false, flipV = true; // NFS3 has the V coordinate flipped by default.
        if (value.Texture is not null)
        {
            brush = new RawImageToBrushConverter().Convert(value.Texture, value.SelectedColor, CultureInfo.InvariantCulture);
            if (IsTextureLikelyTga(value, out var tgaHeader))
            {
                flipU = tgaHeader.Value.ImageInfo.XOrigin != 0;
                flipV = tgaHeader.Value.ImageInfo.YOrigin == 0;
            }
        }
        else
        {
            brush = Brushes.Gray;
        }
        return (brush, flipU, flipV);
    }
    
    /// <inheritdoc/>
    public Model3DGroup? Convert(RenderTreeState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        var (brush, flipU, flipV) = CheckUvFlip(value);
        var matte = new DiffuseMaterial(brush);
        var m1 = new MaterialGroup()
        {
            Children =
            {
                matte,
                //new SpecularMaterial(Brushes.White, 0.5)
            }
        };
        var group = new Model3DGroup()
        {
            Children = {
                new DirectionalLight() { Color = Colors.White, Direction = new Vector3D(1, 1, 3) },
                new AmbientLight() { Color = new Color(){ R = 0x20, G = 0x20, B = 0x20 } }
            }
        };

        foreach (var j in value.Parts)
        {
            group.Children.Add(new GeometryModel3D(ToGeometry(j, flipU, flipV), m1) { BackMaterial = m1 });
        }
        return group;
    }


















    private static MeshGeometry3D ToGeometry(FcePart value, bool flipU, bool flipV)
    {
        const double epsilon = 0.0005;
        var vertexMap = new Dictionary<(int Index, Point Uv), int>();
        var vertices = new List<VertexUv?>();
        var workingCopy = new Triangle[value.Triangles.Length];

        for (int i = 0; i < value.Triangles.Length; i++)
        {
            var triangle = value.Triangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;

            foreach ((int index, float u, float v) in new[]
            {
            (triangle.I1, triangle.U1 * uFlip, triangle.V1 * vFlip),
            (triangle.I2, triangle.U2 * uFlip, triangle.V2 * vFlip),
            (triangle.I3, triangle.U3 * uFlip, triangle.V3 * vFlip)
        })
            {
                var vertexIndex = GetOrAddVertex(vertexMap, vertices, value, index, new Point(u, v));
                if (index == triangle.I1) workingCopy[i].I1 = vertexIndex;
                else if (index == triangle.I2) workingCopy[i].I2 = vertexIndex;
                else if (index == triangle.I3) workingCopy[i].I3 = vertexIndex;
            }
        }

        return new MeshGeometry3D()
        {
            Positions = new Point3DCollection(vertices.Select(p => p?.Vertex ?? default)),
            TriangleIndices = new Int32Collection(workingCopy.SelectMany(p => (int[])[p.I1, p.I2, p.I3])),
            Normals = new Vector3DCollection(vertices.Select(p => p?.Normal ?? default).Select(p => new Vector3D(-p.Z, p.X, -p.Y))),
            TextureCoordinates = new PointCollection(vertices.Select(p => p?.Uv ?? default)),
        };
    }

    private static int GetOrAddVertex(Dictionary<(int Index, Point Uv), int> vertexMap, List<VertexUv?> vertices, FcePart value, int index, Point uv)
    {
        var key = (index, uv);
        if (vertexMap.TryGetValue(key, out int existingIndex))
            return existingIndex;

        var vertex = new VertexUv(
            new Point3D(SizeFactor * (value.Vertices[index].Z + value.Origin.Z), SizeFactor * (-value.Vertices[index].X + -value.Origin.X), SizeFactor *
    (value.Vertices[index].Y + value.Origin.Y)),
            uv,
            value.Normals[index]
        );

        vertices.Add(vertex);
        vertexMap[key] = vertices.Count - 1;
        return vertices.Count - 1;
    }
}
