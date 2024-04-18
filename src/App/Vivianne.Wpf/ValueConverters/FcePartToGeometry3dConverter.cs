using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

public class LookBackConverter : IOneWayValueConverter<Point3D, Point3D>
{
    public Point3D Convert(Point3D value, object? parameter, CultureInfo? culture)
    {
        return (Point3D)(new Point3D(0, 0, 0) - value);
    }
}

public class FcePartToMeshGeometryConverter : IOneWayValueConverter<FcePart, MeshGeometry3D>
{
    public MeshGeometry3D Convert(FcePart value, object? parameter, CultureInfo? culture)
    {
        if (value is null || value.Vertices is null) return null;
        return new MeshGeometry3D()
        {
            Positions = new Point3DCollection(value.Vertices.Select(p => new Point3D(p.Z * -10, p.X * 10, p.Y * 10))),
            TriangleIndices = new Int32Collection(value.Triangles.SelectMany(p => (int[])[p.I1, p.I2, p.I3])),
            Normals = new Vector3DCollection(value.Normals.Select(p => new Vector3D(p.Z, p.X, p.Y))),
            TextureCoordinates = new PointCollection(value.Triangles.SelectMany(p => (Point[])[new Point(p.U1, p.V1), new Point(p.U2, p.V2), new Point(p.U3, p.V3)]))
        };
    }
}
public class FcePreviewViewModelToModel3DGroupConverter : IOneWayValueConverter<RenderTreeState?, Model3DGroup?>
{
    private const double SizeFactor = 10.0;

    private static Point3D ToPoint3D(Vector3d vert, Vector3d origin)
    {
        // NFS3 coords system is left handed; with X lengthwise, Y heightwise and Z widthwise.
        // WPF utilizes right handed coords; X widthwise, Y lenghwise and Z heightwise.
        return new Point3D((vert.Z + origin.Z) * -SizeFactor, (vert.X + origin.X) * SizeFactor, (vert.Y + origin.Y) * SizeFactor);
    }

    private MeshGeometry3D ToGeometry(FcePart value)
    {
        var vertices = value.Vertices.Select(p => ToPoint3D(p, value.Origin)).ToList();
        var uvData = new Dictionary<int, Point>();

        // WPF rendering engine does not support per-triangle UV coords. Remap them.
        void RemapUv(Triangle t, Func<Triangle, double>u, Func<Triangle, double> v, Func<Triangle, int> i)
        {
            var p = new Point(u(t), v(t));
            if (uvData.TryGetValue(i(t), out var uv) && (uv.X != p.X && uv.Y != p.Y))
            {
                var o = vertices[i(t)];
                var copy = new Point3D(o.X, o.Y, o.Z);
                vertices.Add(copy);
                uvData.Add(vertices.Count, p);
            }
            else if (!uvData.ContainsKey(i(t)))
            {
                uvData.Add(i(t), p);
            }

        }

        for (var j = 0; j < value.Triangles.Length; j++)
        {
            RemapUv(value.Triangles[j], t => t.U1, t => t.V1, t => t.I1);
            RemapUv(value.Triangles[j], t => t.U2, t => t.V2, t => t.I2);
            RemapUv(value.Triangles[j], t => t.U3, t => t.V3, t => t.I3);
        }
        Debug.Print(DumpUvCoords(uvData.Values));
        return new MeshGeometry3D()
        {
            Positions = new Point3DCollection(vertices),
            TriangleIndices = new Int32Collection(value.Triangles.SelectMany(p => (int[])[p.I1, p.I2, p.I3])),
            Normals = new Vector3DCollection(value.Normals.Select(p => new Vector3D(p.Z, p.X, p.Y))),
            TextureCoordinates = new PointCollection(uvData.OrderBy(p => p.Key).Select(p => p.Value)),
        };
    }

    private string DumpUvCoords(IEnumerable<Point> uv)
    {
        return string.Join("  ;  ", uv.Select(p => $"{p.X*256}, {p.Y*256}").ToArray());
    }

    public Model3DGroup? Convert(RenderTreeState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;
        var m1 = new MaterialGroup()
        {
            Children =
            {
                new DiffuseMaterial(value.Texture is not null ? new RawImageToBrushConverter().Convert(value.Texture, null, CultureInfo.InvariantCulture) : Brushes.Gray),
                //new SpecularMaterial(Brushes.White, 1)
            }
        };
        var group = new Model3DGroup()
        {
            Children = {
                new DirectionalLight() { Color = Colors.White, Direction = new Vector3D(-1, -1, -3) },
                new AmbientLight() { Color = new Color(){ R = 0x20, G = 0x20, B = 0x20 } }
            }
        };
        foreach (var j in value.Parts)
        {
            group.Children.Add(new GeometryModel3D(ToGeometry(j), m1) { BackMaterial = m1 });
        }
        return group;
    }
}
