using SixLabors.ImageSharp;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a single FshBlob image inside a FSH file.
/// </summary>
public class FshBlob
{
    /// <summary>
    /// Gets the magic signature of this FshBlob blob, which determines its pixel
    /// format.
    /// </summary>
    public FshBlobFormat Magic { get; set; }

    /// <summary>
    /// Gets the FshBlob blob width, in pixels.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// Gets the FshBlob blob height, in pixels.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    /// Gets the FshBlob rotation axis X coordinate.
    /// </summary>
    public ushort XRotation { get; set; }

    /// <summary>
    /// Gets the FshBlob rotation axis Y coordinate.
    /// </summary>
    public ushort YRotation { get; set; }

    /// <summary>
    /// Gets the FshBlob X position coordinate.
    /// </summary>
    public ushort XPosition { get; set; }

    /// <summary>
    /// Gets the FshBlob Y position coordinate.
    /// </summary>
    public ushort YPosition { get; set; }

    /// <summary>
    /// Gets the raw pixel data for this FshBlob. Renderers should use a pixel
    /// format according to the <see cref="Magic"/> signature.
    /// </summary>
    public byte[] PixelData { get; set; } = [];

    /// <summary>
    /// Gets or sets the local color palette for this blob when the
    /// <see cref="Magic"/> property is equal to
    /// <see cref="FshBlobFormat.Indexed8"/>.
    /// </summary>
    /// <remarks>
    /// This property can be set to <see langword="null"/> to indicate that an
    /// external color palette must be used.
    /// </remarks>
    public Color[]? LocalPalette { get; set; }

    /// <summary>
    /// Gets or sets the gauge data associated with this FSH blob.
    /// </summary>
    public GaugeData? GaugeData { get; set; }

    /// <summary>
    /// Gets the extra raw data that may exist after the pixel data.
    /// </summary>
    /// <remarks>
    /// If there is no data after the <see cref="PixelData"/>, when saving this
    /// blob, the relative footer data offset must be set to zero to indicate
    /// that there is no footer data. Otherwise, the offset must be equal to
    /// the relative end of pixel data.
    /// </remarks>
    public byte[] Footer { get; set; } = [];
}
