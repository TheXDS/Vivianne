using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheXDS.Vivianne.Models.Fce.Nfs4;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a part converter that converts a <see cref="FceRenderState"/>
/// into a <see cref="Model3DGroup"/> that can be rendered by WPF.
/// </summary>
public class Fce4RendererConverter : FceRendererConverterBase<FceRenderState, FcePart, FceTriangle, FceColor, HsbColor, FceFile, TriangleFlags>
{
    /// <inheritdoc/>
    protected override IDictionary<TriangleFlags, (Material, bool)> EnumerateMaterials(Brush brush, Brush semiBrush) => new Dictionary<TriangleFlags, (Material, bool)>()
    {
        { TriangleFlags.None, (CreateMaterialGroup(brush, 10), false) },
        { TriangleFlags.None | TriangleFlags.NoCulling, (CreateMaterialGroup(brush, 10), true) },
        { TriangleFlags.NoBlending, (new DiffuseMaterial(brush), false) },
        { TriangleFlags.NoBlending | TriangleFlags.NoCulling, (new DiffuseMaterial(brush), true) },
        { TriangleFlags.HighBlending, (CreateMaterialGroup(brush, 1), false) },
        { TriangleFlags.HighBlending | TriangleFlags.NoCulling, (CreateMaterialGroup(brush, 1), true) },
        { TriangleFlags.Semitrans, (CreateMaterialGroup(semiBrush, 10), false) },
        { TriangleFlags.Semitrans | TriangleFlags.NoCulling, (CreateMaterialGroup(semiBrush, 10), true) },
        { TriangleFlags.Semitrans | TriangleFlags.NoBlending, (new DiffuseMaterial(semiBrush), false) },
        { TriangleFlags.Semitrans | TriangleFlags.NoBlending | TriangleFlags.NoCulling, (new DiffuseMaterial(semiBrush), true) },
        { TriangleFlags.Semitrans | TriangleFlags.HighBlending, (CreateMaterialGroup(semiBrush, 1), false) },
        { TriangleFlags.Semitrans | TriangleFlags.HighBlending | TriangleFlags.NoCulling, (CreateMaterialGroup(semiBrush, 1), true) },
    };

    /// <inheritdoc/>
    protected override TriangleFlags GetFlags(FceTriangle triangle)
    {
        return triangle.Flags & (TriangleFlags)15;
    }
}
