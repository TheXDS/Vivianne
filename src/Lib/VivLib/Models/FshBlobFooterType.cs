namespace TheXDS.Vivianne.Models;

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
    CarDashboard,
    /// <summary>
    /// Indicates that the footer contains a local color palette for FSH blobs
    /// with the <see cref="FshBlobFormat.Indexed8"/> pixel format.
    /// </summary>
    ColorPalette,
    /// <summary>
    /// Indicates that the footer contains padding bytes.
    /// </summary>
    Padding
}
