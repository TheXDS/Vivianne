namespace TheXDS.Vivianne.Models.Audio.Base;

/// <summary>
/// Enumerates the possible compression methods used in audio files.
/// </summary>
public enum CompressionMethod : byte
{
    /// <summary>
    /// No compression.
    /// </summary>
    None = 0x00,
    /// <summary>
    /// EA ADPCM compression.
    /// </summary>
    EA_ADPCM = 0x07,
}
