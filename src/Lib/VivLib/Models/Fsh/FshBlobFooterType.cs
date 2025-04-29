namespace TheXDS.Vivianne.Models.Fsh;

/// <summary>
/// Enumerates the knwon data formats that might be present on a FSH blob.
/// </summary>
public enum FshBlobFooterType
{
    /// <summary>
    /// Indicates that the footer contains no data.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that the footer contains car dashboard data.
    /// </summary>
    /// <remarks>
    /// This type of data is used exclusively by Need For Speed 3.
    /// </remarks>
    CarDashboard,

    /// <summary>
    /// Indicates that the footer contains a local color palette for FSH blobs
    /// with the <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </summary>
    ColorPalette,

    /// <summary>
    /// Indicates that the footer contains some form of padding bytes.
    /// </summary>
    Padding,

    /// <summary>
    /// Indicates that the footer contains a "Metal Bin" attachment.
    /// </summary>
    MetalBin,

    /// <summary>
    /// Indicates that the footer contains a 16-byte blob name attachment.
    /// </summary>
    BlobName,

    /// <summary>
    /// Indicates that the footer contains data that cannot be identified.
    /// </summary>
    Unknown,
}
