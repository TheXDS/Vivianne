using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents an FCE car model.
/// </summary>
public class FceFile : FceFileBase<HsbColor, FcePart>
{
    /// <summary>
    /// Gets or sets a 64-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x1e04 { get; set; } = [];

    /// <inheritdoc/>
    public override IEnumerable<IHsbColor[]> Colors
    {
        get
        {
            foreach (var (primary, secondary) in PrimaryColors.Zip(SecondaryColors))
            {
                yield return [primary, secondary];
            }
        }
    }
}
