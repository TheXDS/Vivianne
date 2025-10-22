using TheXDS.Vivianne.Serializers.Audio;

namespace TheXDS.Vivianne.Models.Audio.Bnk;

/// <summary>
/// Represents a BNK audio library.
/// </summary>
public class BnkFile
{
    /// <summary>
    /// Gets or sets the BNK version to use when storing this BNK file.
    /// </summary>
    public short FileVersion { get; set; }

    /// <summary>
    /// Gets or sets a value that describes the total payload size as
    /// declared on the BNK v4 header.
    /// </summary>
    /// <remarks>
    /// On BNKv2, this value will just be the calculated sum of the data
    /// pool.
    /// </remarks>
    public int PayloadSize { get; set; }

    /// <summary>
    /// Get or sets the unknown value at 0x10 on BNK v4 files.
    /// </summary>
    /// <remarks>
    /// This value can be (and must be) ignored on BNK v2 files.
    /// </remarks>
    public int Unk_0x10 { get; set; }

    /// <summary>
    /// Gets or sets an attachment that may be present between the
    /// <see cref="PtHeader"/> table and the audio payload.
    /// </summary>
    /// <remarks>
    /// This information is not an official part of the BNK spec, but it's a
    /// consequence of how the format works by specifying the start of the data
    /// pool, where it might not match the end of the <see cref="PtHeader"/>
    /// table.
    /// </remarks>
    public byte[] HeaderAttachment { get; set; } = [];

    /// <summary>
    /// Gets the collection of audio stream blobs contained in this BNK
    /// file.
    /// </summary>
    public IList<BnkStream?> Streams { get; } = [];
}
