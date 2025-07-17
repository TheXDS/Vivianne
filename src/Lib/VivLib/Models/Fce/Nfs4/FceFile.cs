using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents an FCE car model.
/// </summary>
public class FceFile : FceFileBase<HsbColor, Fce4Part>, IFceFile<FcePart>
{
    /// <summary>
    /// Gets or sets the unknown 0x0004 value.
    /// </summary>
    public int Unk_0x0004 { get; set; }

    /// <summary>
    /// Gets or sets the contents of the fourth reserved data table.
    /// </summary>
    public byte[] RsvdTable4 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the fifth reserved data table.
    /// </summary>
    public byte[] RsvdTable5 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the sixth reserved data table.
    /// </summary>
    public byte[] RsvdTable6 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the animation table.
    /// </summary>
    public byte[] AnimationTable { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined interior colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// </remarks>
    public IList<HsbColor> InteriorColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined driver hair colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// </remarks>
    public IList<HsbColor> DriverHairColors { get; set; } = [];

    /// <summary>
    /// Gets or sets an integer value whose purpose is currently unknown.
    /// </summary>
    public int Unk_0x0924 { get; set; }

    /// <summary>
    /// Gets or sets a 256-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x0928 { get; set; } = [];

    /// <summary>
    /// Gets or sets a 528-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x1e28 { get; set; } = [];

    /// <inheritdoc/>
    public override IEnumerable<IHsbColor[]> Colors
    {
        get
        {
            foreach (var ((primary, interior, secondary), driverHair) in PrimaryColors.Zip(InteriorColors, SecondaryColors).Zip(DriverHairColors))
            {
                yield return [primary, interior, secondary, driverHair];
            }
        }
    }

    IList<FcePart> IFceFile<FcePart>.Parts => [..Parts.Cast<FcePart>()];
}
