using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo
{
    /// <summary>
    /// Builds <see cref="RenderState"/> objects from a GEO editor state data.
    /// </summary>
    public static class GeoRenderStateBuilder
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
        public static RenderState Build(GeoEditorState state)
        {
            return new()
            {
                Objects = state.Parts.Where(p => p.IsVisible).Select(p => p.Part).Select(p => ToSceneObject(p, state)),
                Textures = state.FshFile ?? CreateMissingTexturesFshFile(state.Parts.SelectMany(p => p.Part.Faces).Select(p => p.TextureName).Distinct())
            };
        }

        private static SceneObject ToSceneObject(GeoPart part, GeoEditorState state)
        {
            var renderCopy = part.Faces.Select(p => p.ShallowClone()).ToArray();
            foreach (var j in renderCopy)
            {
                AdjustTextureBasedOnState(j, state);
            }
            return new()
            {
                Vertices = [.. part.Vertices.Select(TransformVert)],
                Faces = renderCopy,
            };
        }

        private static Vector3 TransformVert(Vector3 p)
        {
            return (Vector3)((Point3D)p * ScaleFactor);
        }

        private static void AdjustTextureBasedOnState(GeoFace face, GeoEditorState state)
        {
            // TODO: Eventually, see if instead of names the Unknown values in the face's data has this information... 🤔

            if (state.BrakelightsOn && face.TextureName.StartsWith("l0", StringComparison.InvariantCultureIgnoreCase))
            {
                face.TextureName = $"l1{face.TextureName[2..]}";
            }
            else if (face.TextureName.StartsWith("m0", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (state.WheelsState)
                {
                    case WheelsState.SlowSpin1:
                        face.TextureName = $"m1{face.TextureName[2..]}";
                        break;
                    case WheelsState.SlowSpin2:
                        face.TextureName = $"m2{face.TextureName[2..]}";
                        break;
                    case WheelsState.SpinningFast:
                        face.TextureName = $"m3{face.TextureName[2..]}";
                        break;
                }
            }
        }

        private static FshFile CreateMissingTexturesFshFile(IEnumerable<string> textureNames)
        {
            FshFile fsh = new();
            fsh.Entries.AddRange(textureNames.Select(p => new KeyValuePair<string, FshBlob?>(p, null))!);
            return fsh;
        }
    }
}
