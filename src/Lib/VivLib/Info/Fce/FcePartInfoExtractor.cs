using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Info.Fce;

/// <summary>
/// Implements an information extractor for <see cref="FcePart"/> entities.
/// </summary>
public class FcePartInfoExtractor : IEntityInfoExtractor<FcePart>
{
    /// <inheritdoc/>
    public string[] GetInfo(FcePart entity)
    {
        return [
            string.Format("Name: {0}", entity.Name),
            string.Format("Origin: X={0}, Y={1}, Z={2}", entity.Origin.X, entity.Origin.Y, entity.Origin.Z),
            string.Format("Vertices: {0}", entity.Vertices.Length),
            string.Format("Triangles: {0}", entity.Triangles.Length)
        ];
    }
}
