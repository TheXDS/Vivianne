namespace TheXDS.Vivianne.Models.Shared;

/// <summary>
/// Enumerates predefined material types for 3D models and dummies.
/// </summary>
/// <remarks>
/// FCE/GEO models do not include material definitions, and rely on triangle
/// flags to determine how each triangle would be rendered. By default, the
/// base material is a single textured diffuse map with semi-gloss effects
/// applied.
/// </remarks>
[Flags]
public enum MaterialFlags
{
    /// <summary>
    /// Default material. Rendered as semi-glossy, textured surface.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Matte. Indicates that the surface must be rendered without any gloss or
    /// specular lighting effects. Mutually exclusive with <see cref="High"/>.
    /// </summary>
    Matte = 1,

    /// <summary>
    /// High blending. Indicates that the material must be rendered as
    /// high-gloss surface. Mutually exclusive with <see cref="Matte"/>.
    /// </summary>
    High = 2,

    /// <summary>
    /// No culling. Indicates that the surface is double-sided.
    /// </summary>
    NoCulling = 4,

    /// <summary>
    /// Semi-transparent. Indicates that the surface must be semi-transparent.
    /// </summary>
    Semitrans = 8,

    /// <summary>
    /// Identifies the mask used to extract material information values from
    /// the original triangle flags.
    /// </summary>
    FceMaterialMask = 15,

    /// <summary>
    /// Identifies the material as having no shading.
    /// </summary>
    NoShading = 0x010000,

    /// <summary>
    /// Identifies the material as including a red color channel component.
    /// </summary>
    RedChannel = 0x020000,

    /// <summary>
    /// Identifies the material as including a green color channel component.
    /// </summary>
    GreenChannel = 0x040000,

    /// <summary>
    /// Identifies the material as including a blue color channel component.
    /// </summary>
    BlueChannel = 0x080000,

    /// <summary>
    /// Quick shortcut to indicate a white dummy with red, green and blue
    /// color components.
    /// </summary>
    WhiteDummy = NoShading | RedChannel | GreenChannel | BlueChannel,

    /// <summary>
    /// Identifies the mask used to extract extended material properties used
    /// internally by Vivianne for rendering special objects, such as dummies.
    /// </summary>
    ExtendedMaterialProps = 0x7fff0000
}