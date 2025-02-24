namespace TheXDS.Vivianne.Models;

/// <summary>
/// Enumerates the possible FCE LOD presets available.
/// </summary>
public enum FceLodPreset
{
    /// <summary>
    /// High detail parts.
    /// </summary>
    High,

    /// <summary>
    /// Medium detail parts.
    /// </summary>
    Medium,

    /// <summary>
    /// Low detail part (usually ":LB").
    /// </summary>
    Low,

    /// <summary>
    /// Tiny detail part (usually ":TB").
    /// </summary>
    Tiny
}
