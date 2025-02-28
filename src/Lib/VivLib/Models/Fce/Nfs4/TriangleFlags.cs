namespace TheXDS.Vivianne.Models.Fce.Nfs4;

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

    Unknown_Elni = 0x10,

    WindowGeneric = 0x20,

    FrontWindow = 0x40,

    LeftWindow = 0x80,

    BackWindow = 0x100,

    RightWindow = 0x200,
    
    BrokenWindow = 0x400,

    Unk_0x0800 = 0x800,

    Unk_0x1000 = 0x1000,
}
