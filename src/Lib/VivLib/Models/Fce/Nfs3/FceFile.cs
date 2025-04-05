using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Represents an FCE car model.
/// </summary>
public class FceFile : FceFileBase<HsbColor, FcePart, FceTriangle>
{
    /// <summary>
    /// Gets or sets the unknown 0x0 value.
    /// </summary>
    /// <remarks>
    /// Might be a magic header, but NFS3 will happily load an FCE file where
    /// these bytes are set to <c>0</c>.
    /// </remarks>
    public int Unk_0x0 { get; set; }

    /// <summary>
    /// Gets or sets a 64-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x1e04 { get; set; } = [];
}
