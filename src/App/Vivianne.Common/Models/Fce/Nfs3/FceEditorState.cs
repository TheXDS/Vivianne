using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents the FCE editor state.
/// </summary>
public class FceEditorState : FceEditorStateBase<FceFile, FceColor, HsbColor, FcePart, FceTriangle>
{
    protected override List<FceColor> ColorsFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return primary.Zip(secondary).Select(p => new FceColor { Name = p.First.ToString(), PrimaryColor = p.First, SecondaryColor = p.Second }).ToList();
    }
}
