using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.Models.Tga;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a part converter that converts a <see cref="RenderState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class FceRendererConverter : IOneWayValueConverter<RenderState?, Model3DGroup?>
{
    private record VertexUv(Point3D Vertex, Vector2 Uv, Vector3 Normal);
    private const double SizeFactor = 10.0;

    private static ReadOnlyDictionary<MaterialFlags, (Material, bool)> MapMaterials(Brush brush, Brush semiBrush) => new Dictionary<MaterialFlags, (Material, bool)>()
    {
        { MaterialFlags.Default, (CreateMaterialGroup(brush, 10), false) },
        { MaterialFlags.Default | MaterialFlags.NoCulling, (CreateMaterialGroup(brush, 10), true) },
        { MaterialFlags.Matte, (new DiffuseMaterial(brush), false) },
        { MaterialFlags.Matte | MaterialFlags.NoCulling, (new DiffuseMaterial(brush), true) },
        { MaterialFlags.High, (CreateMaterialGroup(brush, 1), false) },
        { MaterialFlags.High | MaterialFlags.NoCulling, (CreateMaterialGroup(brush, 1), true) },
        { MaterialFlags.Semitrans, (CreateMaterialGroup(semiBrush, 10), false) },
        { MaterialFlags.Semitrans | MaterialFlags.NoCulling, (CreateMaterialGroup(semiBrush, 10), true) },
        { MaterialFlags.Semitrans | MaterialFlags.Matte, (new DiffuseMaterial(semiBrush), false) },
        { MaterialFlags.Semitrans | MaterialFlags.Matte | MaterialFlags.NoCulling, (new DiffuseMaterial(semiBrush), true) },
        { MaterialFlags.Semitrans | MaterialFlags.High, (CreateMaterialGroup(semiBrush, 1), false) },
        { MaterialFlags.Semitrans | MaterialFlags.High | MaterialFlags.NoCulling, (CreateMaterialGroup(semiBrush, 1), true) },
        { MaterialFlags.NoShading, (new DiffuseMaterial(Brushes.Black){ AmbientColor = Colors.Black }, false)},
        { MaterialFlags.RedChannel, (new DiffuseMaterial(Brushes.Red), false)},
        { MaterialFlags.GreenChannel, (new DiffuseMaterial(Brushes.Green), false)},
        { MaterialFlags.BlueChannel, (new DiffuseMaterial(Brushes.Blue), false)},
        { MaterialFlags.RedChannel | MaterialFlags.GreenChannel, (new DiffuseMaterial(Brushes.Yellow), false)},
        { MaterialFlags.RedChannel | MaterialFlags.BlueChannel, (new DiffuseMaterial(Brushes.Magenta), false)},
        { MaterialFlags.GreenChannel | MaterialFlags.BlueChannel, (new DiffuseMaterial(Brushes.Cyan), false)},
        { MaterialFlags.RedChannel | MaterialFlags.GreenChannel | MaterialFlags.BlueChannel, (new DiffuseMaterial(Brushes.White), false)},
        { MaterialFlags.NoShading | MaterialFlags.RedChannel, (new EmissiveMaterial(Brushes.Red), false)},
        { MaterialFlags.NoShading | MaterialFlags.GreenChannel, (new EmissiveMaterial(Brushes.Green), false)},
        { MaterialFlags.NoShading | MaterialFlags.BlueChannel, (new EmissiveMaterial(Brushes.Blue), false)},
        { MaterialFlags.NoShading | MaterialFlags.RedChannel | MaterialFlags.GreenChannel, (new EmissiveMaterial(Brushes.Yellow), false)},
        { MaterialFlags.NoShading | MaterialFlags.RedChannel | MaterialFlags.BlueChannel, (new EmissiveMaterial(Brushes.Magenta), false)},
        { MaterialFlags.NoShading | MaterialFlags.GreenChannel | MaterialFlags.BlueChannel, (new EmissiveMaterial(Brushes.Cyan), false)},
        { MaterialFlags.WhiteDummy, (new EmissiveMaterial(Brushes.White), false)},
    }.AsReadOnly();

    private static Point3D Vector3ToPoint3D(Vector3 vertex)
    {
        return new Point3D(SizeFactor * vertex.Y, SizeFactor * -vertex.Z, SizeFactor * vertex.X);
    }

    private static void TryCloneVertex(List<VertexUv?> vertex, Point3D vert, int vertIndex, Vector2 uv, SceneObject part, Action<int> vertexSetCallback)
    {
        const double epsilon = 0.0005;
        if (vertex[vertIndex] is { Uv: { } testUv1 } && (Math.Abs(testUv1.X - uv.X) > epsilon || Math.Abs(testUv1.Y - uv.Y) > epsilon))
        {
            vertex.Add(new(vert, new Vector2(uv.X, uv.Y), part.Normals[vertIndex]));
            vertexSetCallback.Invoke(vertex.Count - 1);
        }
    }

    private static bool IsTextureLikelyTga(RenderState state, [NotNullWhen(true)] out TargaHeader? header)
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
        return header is not null
            && Enum.IsDefined(header.Value.ImageType)
            && Enum.IsDefined(header.Value.ColorMapType)
            && ((byte[])[8,16,24,32,48,64,96,128]).Contains(header.Value.ImageInfo.BitsPerPixel)
            && ((int)header.Value.ImageInfo.XOrigin).IsBetween(0, header.Value.ImageInfo.Width)
            && ((int)header.Value.ImageInfo.YOrigin).IsBetween(0, header.Value.ImageInfo.Height);
    }

    private static (Brush brush, bool flipU, bool flipV) CheckUvFlip(RenderState value)
    {
        Brush? brush = null;
        bool flipU = false, flipV = false;
        if (value.Texture is byte[] textureData)
        {
            brush = new RawImageToBrushConverter().Convert(textureData, value.TextureColors, CultureInfo.InvariantCulture);
            if (IsTextureLikelyTga(value, out var tgaHeader))
            {
                flipU = value.ForceUFlip ?? tgaHeader.Value.ImageInfo.XOrigin != 0;
                flipV = value.ForceVFlip ?? !tgaHeader.Value.ImageInfo.PixelFormatDescriptor.HasFlag(ImageDescriptor.TopLeftOrigin);
            }
            else
            {
                flipU = value.ForceUFlip ?? false;
                flipV = value.ForceVFlip ?? false;
            }
        }
        brush ??= Brushes.Gray;
        return (brush, flipU, flipV);
    }

    private static MaterialGroup CreateMaterialGroup(Brush brush, double specularPower, Brush? specularBrush = null) => new()
    {
        Children =
        {
            new DiffuseMaterial(brush),
            new SpecularMaterial(specularBrush ?? Brushes.White, specularPower)
        }
    };

    private static MeshGeometry3D? FcePartToGeometry(SceneObject value, bool flipU, bool flipV, MaterialFlags flags)
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
         * that uses (or prefers to use) per-vertex UV.
         */

        var filteredTriangles = value.Triangles.Where(p => p.Flags == (int)flags).ToArray();
        if (filteredTriangles.Length == 0) return null;
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        var workingCopy = new FceTriangle[filteredTriangles.Length];
        for (int i = 0; i < filteredTriangles.Length; i++)
        {
            var j = filteredTriangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;
            var vert1 = Vector3ToPoint3D(value.Vertices[j.I1]);
            var vert2 = Vector3ToPoint3D(value.Vertices[j.I2]);
            var vert3 = Vector3ToPoint3D(value.Vertices[j.I3]);
            var uv1 = new Vector2(uFlip * j.U1, vFlip * j.V1);
            var uv2 = new Vector2(uFlip * j.U2, vFlip * j.V2);
            var uv3 = new Vector2(uFlip * j.U3, vFlip * j.V3);
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
            TextureCoordinates = [.. vertex.Select(p => p?.Uv is { X: float x, Y: float y } ? new Point(x, y) : default)],
        };
    }

    /// <inheritdoc/>
    public Model3DGroup? Convert(RenderState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;

        var (brush, flipU, flipV) = CheckUvFlip(value);
        var semiBrush = brush.Clone();
        semiBrush.Opacity = 0.5;
        var materials = MapMaterials(brush, semiBrush);
        var group = new Model3DGroup
        {
            Children =
            {
                new DirectionalLight { Color = Colors.White, Direction = new Vector3D(1, 1, 3) },
                new AmbientLight { Color = Color.FromRgb(0x40, 0x40, 0x40) }
            }
        };
        foreach (var (flags, (material, noCulling)) in materials)
        {
            foreach (var part in value.Objects)
            {
                if (FcePartToGeometry(part, flipU, flipV, flags) is { } geometry)
                {
                    group.Children.Add(new GeometryModel3D(geometry, material) { BackMaterial = noCulling ? material : null });
                }
            }
        }
        return group;
    }
}
