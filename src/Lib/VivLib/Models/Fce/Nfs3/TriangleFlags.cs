namespace TheXDS.Vivianne.Models.Fce.Nfs3;

/// <summary>
/// Enumerates the possible material flags for a triangle.
/// </summary>
[Flags]
public enum TriangleFlags : int
{
    /// <summary>
    /// No flags. Rendered as semi-glossy textured material.
    /// </summary>
    None,

    /// <summary>
    /// No blending. Rendered as matte textured material.
    /// </summary>
    NoBlending = 1,

    /// <summary>
    /// High blending. Rendered as high-gloss textured material.
    /// </summary>
    HighBlending = 2,

    /// <summary>
    /// No Culling. Triangles will be drawn on both faces.
    /// </summary>
    NoCulling = 4,

    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, semi-glossy textured material.
    /// </summary>
    Semitrans = 8,

    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, matte textured material.
    /// </summary>
    SemitransNoBlending = Semitrans | NoBlending,

    /// <summary>
    /// Semi transparent. Rendered as semi-transparent, high-gloss textured material.
    /// </summary>
    SemitransHighBlending = Semitrans | HighBlending,

    /// <summary>
    /// No culling, no blending.
    /// </summary>
    NcNoBlending = NoBlending | NoCulling,

    /// <summary>
    /// No culling, high blending.
    /// </summary>
    NcHighBlending = HighBlending | NoCulling,

    /// <summary>
    /// No culling, semi-transparent.
    /// </summary>
    NcSemitrans = Semitrans | NoCulling,

    /// <summary>
    /// No culling, semi-transparent, no blending.
    /// </summary>
    NcSemitransNoBlending = Semitrans | NoBlending | NoCulling,

    /// <summary>
    /// No culling, semi-transparent, high blending.
    /// </summary>
    NcSemitransHighBlending = Semitrans | HighBlending | NoCulling,
}
