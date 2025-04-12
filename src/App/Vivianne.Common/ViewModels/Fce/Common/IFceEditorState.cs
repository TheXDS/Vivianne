using System.Collections.Generic;
using System.Numerics;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type that acts as the state
/// for an FCE file editor.
/// </summary>
/// <typeparam name="TPart">
/// Type of part contained in the FCE file. Other FCE objects shall be of
/// common type and not part of this interface directly.
/// </typeparam>
public interface IFceEditorState<TPart>
    where TPart : FcePart
{
    /// <summary>
    /// Enumerates the available parts from the parts list in the state.
    /// </summary>
    IEnumerable<FcePartListItem<TPart>> Parts { get; }

    /// <summary>
    /// Enumerates the available dummies from the dummy list in the state.
    /// </summary>
    IEnumerable<FcePartListItem<FceDummy>> Dummies { get; }

    /// <summary>
    /// Gets the raw file contents of the texture to be used when rendering the
    /// model, or <see langword="null"/> if no texture has been selected.
    /// </summary>
    byte[]? SelectedTexture { get; }

    /// <summary>
    /// Gets a value that indicates if the user selected the bounding box
    /// shadow mesh to be included in the final render
    /// </summary>
    bool RenderShadow { get; }

    /// <summary>
    /// Gets the bounding box half-size vectors.
    /// </summary>
    Vector3 HalfSize { get; }
}
