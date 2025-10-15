using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo
{
    /// <summary>
    /// Builds <see cref="RenderState"/> objects from a GEO editor state data.
    /// </summary>
    public class GeoRenderStateBuilder
    {
        private const double ScaleFactor = 0.003;

        /// <summary>
        /// Builds the entire render state from the specified editor state.
        /// </summary>
        /// <param name="state">
        /// State from which to extract the render scene data.
        /// </param>
        /// <returns>
        /// A new <see cref="RenderState"/> that can be used to render and display
        /// the GEO model.
        /// </returns>
        public RenderState Build(GeoEditorState state) => new()
        {
            Objects = state.Parts.Where(p => p.IsVisible).Select(p => p.Part).Select(ToSceneObject),
            Textures = state.FshFile  ?? CreateMissingTexturesFshFile(state.Parts.SelectMany(p => p.Part.Faces).Select(p => p.TextureName).Distinct())
        };

        private static SceneObject ToSceneObject(GeoPart part, int index) => new()
        {
            Vertices = [.. part.Vertices.Select(p => (Vector3)((Point3D)p * ScaleFactor))],
            Faces = part.Faces,
        };

        private static FshFile CreateMissingTexturesFshFile(IEnumerable<string> textureNames)
        {
            FshFile fsh = new();
            fsh.Entries.AddRange(textureNames.Select(p => new KeyValuePair<string, FshBlob?>(p, null))!);
            return fsh;
        }
    }
}
