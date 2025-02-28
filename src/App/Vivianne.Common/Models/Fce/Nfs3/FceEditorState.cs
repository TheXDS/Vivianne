using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

public class FceEditorState : FileStateBase<FceFile>
{
    private ObservableListWrap<FceColor>? _colors;
    private ObservableListWrap<FcePart>? _parts;
    private ObservableListWrap<FceDummy>? _dummies;

    private static List<FceColor> ColorsFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return primary.Zip(secondary).Select(p => new FceColor { Name = p.First.ToString(), PrimaryColor = p.First, SecondaryColor = p.Second }).ToList();
    }

    /// <summary>
    /// Gets a collection of the available colors from the FCE file.
    /// </summary>
    public ObservableListWrap<FceColor> Colors => _colors ??= GetObservable(ColorsFromFce(File));

    public ObservableListWrap<FcePart> Parts => _parts ??= GetObservable(File.Parts.ToList());

    public ObservableListWrap<FceDummy> Dummies => _dummies ??= GetObservable(File.Dummies.ToList());
}
