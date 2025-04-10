using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Tga;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Base class for an FCE renderer that can convert FCE render state data into
/// a <see cref="Model3D"/> that can be displayed by WPF.
/// </summary>
/// <typeparam name="TRender"></typeparam>
/// <typeparam name="TFcePart"></typeparam>
/// <typeparam name="TFceTriangle"></typeparam>
/// <typeparam name="TFceColor"></typeparam>
/// <typeparam name="THsbColor"></typeparam>
/// <typeparam name="TFceFile"></typeparam>
/// <typeparam name="TTriangleFlags"></typeparam>
public abstract class FceRendererConverterBase<TRender, TFcePart, TFceTriangle, TFceColor, THsbColor, TFceFile, TTriangleFlags> : IOneWayValueConverter<TRender?, Model3DGroup?>
    where TRender : FceRenderStateBase<TFcePart, TFceTriangle, TFceColor, THsbColor, TFceFile>
    where TFceFile : FceFileBase<THsbColor, TFcePart, TFceTriangle>
    where TFceColor : IFceColor<THsbColor>
    where THsbColor : IHsbColor
    where TFcePart : FcePartBase<TFceTriangle>
    where TFceTriangle : IFceTriangle, new()
    where TTriangleFlags : unmanaged, Enum
{
    private record VertexUv(Point3D Vertex, Point Uv, Vector3d Normal);

    private const double SizeFactor = 10.0;

    private static Point3D Vector3dToPoint3D(Vector3d vertex, Vector3d partOrigin)
    {
        return new Point3D(SizeFactor * (vertex.Y + partOrigin.Y), SizeFactor * (-vertex.Z + -partOrigin.Z), SizeFactor * (vertex.X + partOrigin.X));
    }

    private static void TryCloneVertex(List<VertexUv?> vertex, Point3D vert, int vertIndex, Point uv, TFcePart part, Action<int> vertexSetCallback)
    {
        const double epsilon = 0.0005;
        if (vertex[vertIndex] is { Uv: { } testUv1 } && (Math.Abs(testUv1.X - uv.X) > epsilon || Math.Abs(testUv1.Y - uv.Y) > epsilon))
        {
            vertex.Add(new(vert, new Point(uv.X, uv.Y), part.Normals[vertIndex]));
            vertexSetCallback.Invoke(vertex.Count - 1);
        }
    }

    private static bool IsTextureLikelyTga(TRender state, [NotNullWhen(true)] out TargaHeader? header)
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

    private static (Brush brush, bool flipU, bool flipV) CheckUvFlip(TRender value)
    {
        Brush? brush = null;
        bool flipU = false, flipV = true; // FCE models has the V coordinate flipped by default.
        if (value.Texture is byte[] textureData)
        {
            brush = new RawImageToBrushConverter().Convert(textureData, value.SelectedColor, CultureInfo.InvariantCulture);
            if (IsTextureLikelyTga(value, out var tgaHeader))
            {
                flipU = tgaHeader.Value.ImageInfo.XOrigin != 0;
                flipV = tgaHeader.Value.ImageInfo.YOrigin == 0;

                if (tgaHeader.Value.ImageInfo.PixelFormatDescriptor == 40)
                {
                    flipU ^= flipU;
                    flipV ^= flipV;
                }
            }
        }
        brush ??= Brushes.Gray;
        return (brush, flipU, flipV);
    }

    private static GeometryModel3D CreateShadow(TFceFile file)
    {
        var verts = new Point3D[]
        {
            new(-file.YHalfSize * SizeFactor, file.ZHalfSize * SizeFactor, file.XHalfSize * SizeFactor),
            new(-file.YHalfSize * SizeFactor, file.ZHalfSize * SizeFactor, -file.XHalfSize * SizeFactor),
            new(-file.YHalfSize * SizeFactor, -file.ZHalfSize * SizeFactor, file.XHalfSize * SizeFactor),
            new(-file.YHalfSize * SizeFactor, -file.ZHalfSize * SizeFactor, -file.XHalfSize * SizeFactor),
        };
        var normals = new Vector3D[]
        {
            new(file.YHalfSize * SizeFactor, file.ZHalfSize * SizeFactor, file.XHalfSize * SizeFactor),
            new(file.YHalfSize * SizeFactor, file.ZHalfSize * SizeFactor, -file.XHalfSize * SizeFactor),
            new(file.YHalfSize * SizeFactor, -file.ZHalfSize * SizeFactor, file.XHalfSize * SizeFactor),
            new(file.YHalfSize * SizeFactor, -file.ZHalfSize * SizeFactor, -file.XHalfSize * SizeFactor),
        };
        var tris = new int[] { 1, 0, 2, 1, 2, 3 };
        var uv = new Point[]
        {
            new(0, 0),
            new(0, 1),
            new(1, 0),
            new(1, 1)
        };
        return new GeometryModel3D(new MeshGeometry3D()
        {
            Positions = [.. verts],
            TriangleIndices = [.. tris],
            TextureCoordinates = [.. uv],
            Normals = [.. normals]
        }, new DiffuseMaterial(Brushes.Black));
    }

    /// <summary>
    /// provides of a simple way of declaring a material that has both Diffuse
    /// and Specular properties.
    /// </summary>
    /// <param name="brush">Brush that defines the diffuse color/texture.</param>
    /// <param name="specularPower">Inversely specifies the desired specular power.</param>
    /// <param name="specularBrush">Brush that defines the specular color to use. Defaults to <see cref="Brushes.White"/>.</param>
    /// <returns></returns>
    protected static MaterialGroup CreateMaterialGroup(Brush brush, double specularPower, Brush? specularBrush = null) => new()
    {
        Children =
        {
            new DiffuseMaterial(brush),
            new SpecularMaterial(specularBrush ?? Brushes.White, specularPower)
        }
    };

    private MeshGeometry3D? FcePartToGeometry(TRender render, TFcePart value, bool flipU, bool flipV, TTriangleFlags flags)
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

        var filteredTriangles = value.Triangles.Where(p => GetFlags(p).Equals(flags)).ToArray();
        if (filteredTriangles.Length == 0) return null;
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        var workingCopy = new TFceTriangle[filteredTriangles.Length];
        for (int i = 0; i < filteredTriangles.Length; i++)
        {
            var j = filteredTriangles[i];
            var uFlip = flipU ? -1 : 1;
            var vFlip = flipV ? -1 : 1;
            var vert1 = Vector3dToPoint3D(GetVector(render, value, j.I1), value.Origin);
            var vert2 = Vector3dToPoint3D(GetVector(render, value, j.I2), value.Origin);
            var vert3 = Vector3dToPoint3D(GetVector(render, value, j.I3), value.Origin);
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

    /// <summary>
    /// Gets the vector at the specified index based on the current render
    /// state data.
    /// </summary>
    /// <param name="render">
    /// State that contains the current render state data.
    /// </param>
    /// <param name="part">FCE part from which to get the vector.</param>
    /// <param name="index">Index of the vector to get.</param>
    /// <returns>
    /// A vector at the specified index in the vector table of the FCE part.
    /// </returns>
    protected abstract Vector3d GetVector(TRender render, TFcePart part, int index);

    /// <summary>
    /// Gets the triangle flags from the specified FCE triangle.
    /// </summary>
    /// <param name="triangle">Triangle for which to get the material flags.</param>
    /// <returns>The material flags for the triangle.</returns>
    protected abstract TTriangleFlags GetFlags(TFceTriangle triangle);

    /// <summary>
    /// Enumerates the existing materials based on the available triangle flags for the FCE format.
    /// </summary>
    /// <returns></returns>
    protected abstract IDictionary<TTriangleFlags, (Material, bool)> EnumerateMaterials(Brush brush, Brush semiBrush);

    /// <inheritdoc/>
    public Model3DGroup? Convert(TRender? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;

        var (brush, flipU, flipV) = CheckUvFlip(value);
        var semiBrush = brush.Clone();
        semiBrush.Opacity = 0.5;

        var materials = EnumerateMaterials(brush, semiBrush);

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
            foreach (var part in value.VisibleParts)
            {
                group.Children.Add(new GeometryModel3D(FcePartToGeometry(value, part, flipU, flipV, flags), material) { BackMaterial = noCulling ? material : null });
            }
        }

        if (value.FceFile is TFceFile fceFile)
        {
            group.Children.Add(CreateShadow(fceFile));
        }

        return group;
    }
}
