using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce;
using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Models;

public class Fce3EditorState : FileStateBase<FceFile>
{
    private ObservableListWrap<Fce3Color>? _colors;
    private ObservableListWrap<FcePart>? _parts;
    private ObservableListWrap<FceDummy>? _dummies;

    private static List<Fce3Color> ColorsFromFce(FceFile fce)
    {
        ICollection<HsbColor> primary = fce.PrimaryColors;
        IEnumerable<HsbColor> secondary = fce.SecondaryColors.Count > 0 ? fce.SecondaryColors.ToArray().Wrapping(16) : primary;
        return primary.Zip(secondary).Select(p => new Fce3Color { Name = p.First.ToString(), PrimaryColor = p.First, SecondaryColor = p.Second }).ToList();
    }

    /// <summary>
    /// Gets a collection of the available colors from the FCE file.
    /// </summary>
    public ObservableListWrap<Fce3Color> Colors => _colors ??= GetObservable(ColorsFromFce(File));

    public ObservableListWrap<FcePart> Parts => _parts ??= GetObservable(File.Parts.ToList());

    public ObservableListWrap<FceDummy> Dummies => _dummies ??= GetObservable(File.Dummies.ToList());
}
