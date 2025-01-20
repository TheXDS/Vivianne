using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
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

    private static Point3D FromVertex(Vector3d vertex, Vector3d partOrigin)
    {
        return new Point3D(SizeFactor * (vertex.Y + partOrigin.Y), SizeFactor * (-vertex.Z + -partOrigin.Z), SizeFactor * (vertex.X + partOrigin.X));
    }

    private static MeshGeometry3D? ToGeometry(FcePart value, bool flipU, bool flipV, TriangleFlags flags)
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

        var filteredTriangles = value.Triangles.Where(p => p.Flags == flags).ToArray();
        if (filteredTriangles.Length == 0) return null;
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        const double epsilon = 0.0005;
        var workingCopy = new Triangle[filteredTriangles.Length];
        for (int i = 0; i < filteredTriangles.Length; i++)
        {
            var j = filteredTriangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;
            var vert1 = FromVertex(value.Vertices[j.I1], value.Origin);
            var vert2 = FromVertex(value.Vertices[j.I2], value.Origin);
            var vert3 = FromVertex(value.Vertices[j.I3], value.Origin);
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
            vertex[j.I1] = new(vert1, uv1, value.Normals[filteredTriangles[i].I1]);
            vertex[j.I2] = new(vert2, uv2, value.Normals[filteredTriangles[i].I2]);
            vertex[j.I3] = new(vert3, uv3, value.Normals[filteredTriangles[i].I3]);
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

    private static (Brush brush, bool flipU, bool flipV) CheckUvFlip(RenderTreeState value)
    {
        Brush? brush = null;
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
        brush ??= Brushes.Gray;
        return (brush, flipU, flipV);
    }

    /// <inheritdoc/>
    public Model3DGroup? Convert(RenderTreeState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;

        var (brush, flipU, flipV) = CheckUvFlip(value);
        var semiBrush = brush.Clone();
        semiBrush.Opacity = 0.5;

        var materials = new Dictionary<TriangleFlags, Material>
        {
            { TriangleFlags.None, CreateMaterialGroup(brush, Brushes.White, 1) },
            { TriangleFlags.NoBlending, new DiffuseMaterial(brush) },
            { TriangleFlags.HighBlending, CreateMaterialGroup(brush, Brushes.White, 0.1) },
            { TriangleFlags.Semitrans, CreateMaterialGroup(semiBrush, Brushes.White, 1) },
            { TriangleFlags.SemitransNoBlending, new DiffuseMaterial(semiBrush) }
        };

        var group = new Model3DGroup
        {
            Children =
            {
                new DirectionalLight { Color = Colors.White, Direction = new Vector3D(1, 1, 3) },
                new AmbientLight { Color = Color.FromRgb(0x40, 0x40, 0x40) }
            }
        };

        foreach (var (flags, material) in materials.Concat(materials.Select(p => new KeyValuePair<TriangleFlags, Material>(p.Key | TriangleFlags.NoCulling, p.Value))))
        {
            foreach (var part in value.Parts)
            {
                group.Children.Add(new GeometryModel3D(ToGeometry(part, flipU, flipV, flags), material) { BackMaterial = flags.HasFlag(TriangleFlags.NoCulling) ? material : null });
            }
        }

        return group;
    }

    private static MaterialGroup CreateMaterialGroup(Brush brush, Brush specularBrush, double specularPower)
    {
        return new MaterialGroup
        {
            Children =
            {
                new DiffuseMaterial(brush),
                new SpecularMaterial(specularBrush, specularPower)
            }
        };
    }
}
