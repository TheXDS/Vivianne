using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.ViewModels.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents the FCE editor state.
/// </summary>
public class FceEditorState : FceEditorStateBase<FceFile, FceColor, HsbColor, FcePart, FceTriangle>
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
