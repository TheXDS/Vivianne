using System.Collections.Generic;
using System.Linq;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.ViewModels.Fce.Nfs4;

/// <summary>
/// Represents the current state of the Fce3 editor.
/// </summary>
public class Fce4EditorState : FceEditorStateBase<FceFile, FceColor, HsbColor, Fce4Part>
{
    private bool _showDamagedModel;

    /// <inheritdoc/>
    protected override List<FceColor> ColorsFromFce(FceFile fce)
    {
        return [.. fce.PrimaryColors.Zip(fce.InteriorColors, fce.SecondaryColors).Zip(fce.DriverHairColors)
            .Select(p => new FceColor
            {
                Name = p.First.First.ToString(),
                PrimaryColor = p.First.First,
                InteriorColor = p.First.Second,
                SecondaryColor = p.First.Third,
                DriverHairColor = p.Second
            })];
    }

    /// <summary>
    /// Gets or sets a value that indicates if the user wants to see the
    /// damaged version of the FCE model.
    /// </summary>
    public bool ShowDamagedModel
    {
        get => _showDamagedModel;
        set => Change(ref _showDamagedModel, value);
    }
}