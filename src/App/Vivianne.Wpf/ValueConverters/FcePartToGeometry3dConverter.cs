using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.MCART.ValueConverters.Base;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.ViewModels;

namespace TheXDS.Vivianne.ValueConverters;

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

/// <summary>
/// Implements a value converter that converts a <see cref="RenderTreeState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class FcePreviewViewModelToModel3DGroupConverter : IOneWayValueConverter<RenderTreeState?, Model3DGroup?>
{
    private const double SizeFactor = 10.0;

    private static Point3D ToPoint3D(Vector3d vert, Vector3d origin)
    {
        // NFS3 coords system is left handed; with X lengthwise, Y heightwise and Z widthwise.
        // WPF utilizes right handed coords; X widthwise, Y lenghwise and Z heightwise.
        return new Point3D((vert.Z + origin.Z) * -SizeFactor, (vert.X + origin.X) * SizeFactor, (vert.Y + origin.Y) * SizeFactor);
    }

    private static MeshGeometry3D ToGeometry(FcePart value)
    {
        var vertex = new (Point3D, Point)[value.Vertices.Length];
        foreach (var j in value.Triangles)
        {
            var v1 = value.Vertices[j.I1];
            var v2 = value.Vertices[j.I2];
            var v3 = value.Vertices[j.I3];
            vertex[j.I1] = (new Point3D(SizeFactor * v1.Z, SizeFactor * v1.X, SizeFactor * v1.Y), new Point(j.U1, -j.V1));
            vertex[j.I2] = (new Point3D(SizeFactor * v2.Z, SizeFactor * v2.X, SizeFactor * v2.Y), new Point(j.U2, -j.V2));
            vertex[j.I3] = (new Point3D(SizeFactor * v3.Z, SizeFactor * v3.X, SizeFactor * v3.Y), new Point(j.U3, -j.V3));
        }
        return new MeshGeometry3D()
        {
            Positions = new Point3DCollection(vertex.Select(p => p.Item1)),
            TriangleIndices = new Int32Collection(value.Triangles.SelectMany(p => (int[])[p.I1, p.I2, p.I3])),
            Normals = new Vector3DCollection(value.Normals.Select(p => new Vector3D(p.Z, p.X, p.Y))),
            TextureCoordinates = new PointCollection(vertex.Select(p => p.Item2)),
        };
    }

    /// <inheritdoc/>
    public Model3DGroup? Convert(RenderTreeState? value, object? parameter, CultureInfo? culture)
    {
        if (value is null) return null;

        var matte = new DiffuseMaterial(value.Texture is not null ? new RawImageToBrushConverter().Convert(value.Texture, value.SelectedColor, CultureInfo.InvariantCulture) : Brushes.Gray);
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
            group.Children.Add(new GeometryModel3D(ToGeometry(j), m1) { BackMaterial = m1 });
        }
        return group;
    }
}
