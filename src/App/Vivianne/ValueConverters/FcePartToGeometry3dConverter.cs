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
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Tga;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a part converter that converts a <see cref="FceRenderState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class FcePreviewViewModelToModel3DGroupConverter : IOneWayValueConverter<FceRenderState?, Model3DGroup?>
{
    private record VertexUv(Point3D Vertex, Point Uv, Vector3d Normal);

    private const double SizeFactor = 10.0;

    private static Point3D Vector3dToPoint3D(Vector3d vertex, Vector3d partOrigin)
    {
        return new Point3D(SizeFactor * (vertex.Y + partOrigin.Y), SizeFactor * (-vertex.Z + -partOrigin.Z), SizeFactor * (vertex.X + partOrigin.X));
    }

    private static MeshGeometry3D? FcePartToGeometry(FcePart value, bool flipU, bool flipV, TriangleFlags flags)
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

        var filteredTriangles = value.Triangles.Where(p => (p.Flags & (TriangleFlags)15) == flags).ToArray();
        if (filteredTriangles.Length == 0) return null;
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        var workingCopy = new FceTriangle[filteredTriangles.Length];
        for (int i = 0; i < filteredTriangles.Length; i++)
        {
            var j = filteredTriangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;
            var vert1 = Vector3dToPoint3D(value.Vertices[j.I1], value.Origin);
            var vert2 = Vector3dToPoint3D(value.Vertices[j.I2], value.Origin);
            var vert3 = Vector3dToPoint3D(value.Vertices[j.I3], value.Origin);
            var uv1 = new Point(uFlip * j.U1, vFlip * j.V1);
            var uv2 = new Point(uFlip * j.U2, vFlip * j.V2);
            var uv3 = new Point(uFlip * j.U3, vFlip * j.V3);
            TryCloneVertex(vertex, vert1, j.I1, uv1, value, p => j.I1 = p);
            TryCloneVertex(vertex, vert2, j.I2, uv2, value, p => j.I2 = p);
            TryCloneVertex(vertex, vert3, j.I3, uv3, value, p => j.I3 = p);
            vertex[j.I1] = new(vert1, uv1, value.Normals[filteredTriangles[i].I1]);
            vertex[j.I2] = new(vert2, uv2, value.Normals[filteredTriangles[i].I2]);
            vertex[j.I3] = new(vert3, uv3, value.Normals[filteredTriangles[i].I3]);
            workingCopy[i] = j;
        }
        return new MeshGeometry3D()
        {
            Positions = [.. vertex.Select(p => p?.Vertex ?? default)],
            TriangleIndices = [.. workingCopy.SelectMany(p => (int[])[p.I1, p.I2, p.I3])],
            Normals = [.. vertex.Select(p => p?.Normal ?? default).Select(p => new Vector3D(-p.Z, p.X, -p.Y))],
            TextureCoordinates = [.. vertex.Select(p => p?.Uv ?? default)],
        };
    }

    private static void TryCloneVertex(List<VertexUv?> vertex, Point3D vert, int vertIndex, Point uv, FcePart part, Action<int> vertexSetCallback)
    {
        const double epsilon = 0.0005;
        if (vertex[vertIndex] is { Uv: { } testUv1 } && (Math.Abs(testUv1.X - uv.X) > epsilon || Math.Abs(testUv1.Y - uv.Y) > epsilon))
        {
            vertex.Add(new(vert, new Point(uv.X, uv.Y), part.Normals[vertIndex]));
            vertexSetCallback.Invoke(vertex.Count - 1);
        }
    }

    private static bool IsTextureLikelyTga(FceRenderState state, [NotNullWhen(true)] out TargaHeader? header)
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

    private static (Brush brush, bool flipU, bool flipV) CheckUvFlip(FceRenderState value)
    {
        Brush? brush = null;
        bool flipU = false, flipV = true; // NFS3 has the V coordinate flipped by default.
        if (value.Texture is byte[] textureData)
        {
            brush = new RawImageToBrushConverter().Convert(textureData, value.SelectedColor, CultureInfo.InvariantCulture);
            if (IsTextureLikelyTga(value, out var tgaHeader))
            {
                flipU = tgaHeader.Value.ImageInfo.XOrigin != 0;
                flipV = tgaHeader.Value.ImageInfo.YOrigin == 0;
            }
        }
        brush ??= Brushes.Gray;
        return (brush, flipU, flipV);
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

    /// <inheritdoc/>
    public Model3DGroup? Convert(FceRenderState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;

        var (brush, flipU, flipV) = CheckUvFlip(value);
        var semiBrush = brush.Clone();
        semiBrush.Opacity = 0.5;

        var materials = new Dictionary<TriangleFlags, Material>
        {
            { TriangleFlags.None, CreateMaterialGroup(brush, Brushes.White, 10) },
            { TriangleFlags.NoBlending, new DiffuseMaterial(brush) },
            { TriangleFlags.HighBlending, CreateMaterialGroup(brush, Brushes.White, 1) },
            { TriangleFlags.Semitrans, CreateMaterialGroup(semiBrush, Brushes.White, 10) },
            { TriangleFlags.SemitransNoBlending, new DiffuseMaterial(semiBrush) },
            { TriangleFlags.SemitransHighBlending, CreateMaterialGroup(semiBrush, Brushes.White, 1) },
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
            foreach (var part in value.VisibleParts)
            {
                group.Children.Add(new GeometryModel3D(FcePartToGeometry(part, flipU, flipV, flags), material) { BackMaterial = flags.HasFlag(TriangleFlags.NoCulling) ? material : null });
            }
        }

        return group;
    }
}
