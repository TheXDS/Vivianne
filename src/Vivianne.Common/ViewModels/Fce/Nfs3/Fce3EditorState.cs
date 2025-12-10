using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs3;

/// <summary>
/// Represents the current state of the Fce3 editor.
/// </summary>
public class Fce3EditorState : FceEditorStateBase<FceFile, FceColor, HsbColor, FcePart>
{
    /// <inheritdoc/>
    protected override List<FceColor> ColorsFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return [.. primary.Zip(secondary).Select(p => new FceColor { Name = p.First.ToString(), PrimaryColor = p.First, SecondaryColor = p.Second })];
    }
}