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

    /// <summary>
    /// Flag specifically seen on NFS4's La Niña (Elni) car model with unknown
    /// purpose. Might be trash data.
    /// </summary>
    Unknown_Elni = 0x10,

    /// <summary>
    /// Flag that indicates a generic window element (breakable)
    /// </summary>
    WindowGeneric = 0x20,

    /// <summary>
    /// Flag that indicates that marked triangles are part of the windshield
    /// (front window).
    /// </summary>
    FrontWindow = 0x40,

    /// <summary>
    /// Flag that indicates that marked triangles are part of the left window.
    /// </summary>
    LeftWindow = 0x80,

    /// <summary>
    /// Flag that indicates that marked triangles are part of the rear window.
    /// </summary>
    BackWindow = 0x100,

    /// <summary>
    /// Flag that indicates that marked triangles are part of the right window.
    /// </summary>
    RightWindow = 0x200,

    /// <summary>
    /// Flag that indicates that marked triangles are part of a generic broken
    /// window.
    /// </summary>
    BrokenWindow = 0x400,

    /// <summary>
    /// Unknown flag seen on some car models. Might be trash data.
    /// </summary>
    Unk_0x0800 = 0x800,

    /// <summary>
    /// Unknown flag seen on some car models. Might be trash data.
    /// </summary>
    Unk_0x1000 = 0x1000,
}
