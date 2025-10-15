using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.ViewModels.Geo;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a part converter that converts a <see cref="RenderState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class GeoRendererConverter : IOneWayValueConverter<RenderState?, Model3DGroup?>
{
    private static readonly FshImageConverter _fsh2Brush = new();
    private const double SizeFactor = 10.0;
    private const float TextureInset = 0.0f;

    private struct Triangle
    {
        public int I1;
        public int I2;
        public int I3;
        public bool IsFirstHalf;
    }
    private record VertexUv(Point3D Vertex, Vector2 Uv);

    /// <inheritdoc/>
    public Model3DGroup? Convert(RenderState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        Dictionary<string, Material> materials = new(value.Textures.Entries.Select(p => new KeyValuePair<string, Material>(p.Key, ToMaterial(p.Value, value.Textures))));
        var group = new Model3DGroup
        {
            Children =
            {
                new DirectionalLight { Color = Colors.White, Direction = new Vector3D(1, 1, 3) },
                new AmbientLight { Color = Color.FromRgb(0x40, 0x40, 0x40) }
            }
        };
        foreach (var (materialKey, material) in materials)
        {
            foreach (var part in value.Objects)
            {
                if (GeoPartToGeometry(part, materialKey, GetTextureSize(value.Textures.Entries[materialKey])) is { } geometry)
                {
                    group.Children.Add(new GeometryModel3D(geometry, material) { BackMaterial = material });
                }
            }
        }
        return group;
    }

    private static System.Drawing.SizeF GetTextureSize(FshBlob? fsh)
    {
        return fsh is not null ? new System.Drawing.SizeF(fsh.Width, fsh.Height) : System.Drawing.SizeF.Empty;
    }

    private static MeshGeometry3D? GeoPartToGeometry(SceneObject value, string textureName, System.Drawing.SizeF textureSize)
    {
        /* GEO 3D models use a very old rendering technique based on quads and
         * per-face texturing.
         * 
         * This is a WPF app, and WPF itself includes a simple yet easy to use
         * 3D rendering engine, but it's limited to use triangles and
         * per-vertex UV. This means that in order to properly render GEO
         * models, we have to convert from quads and per-face texturing to
         * triangles and per-vertex UV format.
         */

        var filteredTriangles = value.Faces.Where(p => p.TextureName == textureName).SelectMany(QuadsToTriangles).ToArray();
        if (filteredTriangles.Length == 0) return null;
        var vertex = new List<VertexUv?>(new VertexUv[value.Vertices.Length]);
        var workingCopy = new Triangle[filteredTriangles.Length];
        for (int i = 0; i < filteredTriangles.Length; i++)
        {
            var j = filteredTriangles[i];
            var vert1 = Vector3ToPoint3D(value.Vertices[j.I1]);
            var vert2 = Vector3ToPoint3D(value.Vertices[j.I2]);
            var vert3 = Vector3ToPoint3D(value.Vertices[j.I3]);
            var uv2 = j.IsFirstHalf ? new Vector2(textureSize.Width - (textureSize.Width * TextureInset), textureSize.Height - (textureSize.Height * TextureInset)) : new Vector2(TextureInset, textureSize.Height - (textureSize.Height * TextureInset));
            var uv3 = new Vector2(textureSize.Width - (textureSize.Width * TextureInset), TextureInset);
            var uv1 = j.IsFirstHalf ? new Vector2(0, textureSize.Height - (textureSize.Height * TextureInset)) : new Vector2(TextureInset, TextureInset);
            TryCloneVertex(vertex, vert1, j.I1, uv1, p => j.I1 = p);
            TryCloneVertex(vertex, vert2, j.I2, uv2, p => j.I2 = p);
            TryCloneVertex(vertex, vert3, j.I3, uv3, p => j.I3 = p);
            vertex[j.I1] = new(vert1, uv1);
            vertex[j.I2] = new(vert2, uv2);
            vertex[j.I3] = new(vert3, uv3);
            workingCopy[i] = j;
        }
        return new MeshGeometry3D()
        {
            Positions = [.. vertex.Select(p => p?.Vertex ?? default)],
            TriangleIndices = [.. workingCopy.SelectMany(p => (int[])[p.I1, p.I2, p.I3])],
            Normals = [.. vertex.Select(p => p?.Vertex ?? default).Select(p => new Vector3D(-p.Z * 1.1, p.X * 1.1, -p.Y * 1.1))],
            TextureCoordinates = [.. vertex.Select(p => p?.Uv is { X: float x, Y: float y } ? new Point(x, y) : default)],
        };

    }

    private static void TryCloneVertex(List<VertexUv?> vertex, Point3D vert, int vertIndex, Vector2 uv, Action<int> vertexSetCallback)
    {
        const double epsilon = 0.0005;
        if (vertex[vertIndex] is { Uv: { } testUv1 } && (Math.Abs(testUv1.X - uv.X) > epsilon || Math.Abs(testUv1.Y - uv.Y) > epsilon))
        {
            vertex.Add(new(vert, new Vector2(uv.X, uv.Y)));
            vertexSetCallback.Invoke(vertex.Count - 1);
        }
    }

    private static Triangle[] QuadsToTriangles(GeoFace face)
    {
        return [
            new()
            {
                I1 = face.Vertex1,
                I2 = face.Vertex2,
                I3 = face.Vertex3,
                IsFirstHalf = true
            },
            new()
            {
                I1 = face.Vertex4,
                I2 = face.Vertex1,
                I3 = face.Vertex3,
                IsFirstHalf = false
            }
            ];
    }

    private static Point3D Vector3ToPoint3D(Vector3 vertex)
    {
        return new Point3D(SizeFactor * vertex.Y, SizeFactor * -vertex.Z, SizeFactor * vertex.X);
    }

    private static DiffuseMaterial ToMaterial(FshBlob? fsh, FshFile fshFile)
    {
        return fsh is not null ? new DiffuseMaterial(new ImageBrush(_fsh2Brush.Convert(fsh, fshFile, CultureInfo.InvariantCulture))
        {
            ViewportUnits = BrushMappingMode.Absolute,
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, fsh.Width, fsh.Height)
        }) : new DiffuseMaterial(Brushes.Magenta);
    }
}
