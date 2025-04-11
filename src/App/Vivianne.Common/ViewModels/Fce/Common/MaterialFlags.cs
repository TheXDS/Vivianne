using System;

namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Enumerates predefined material types for FCE models and dummies.
/// </summary>
/// <remarks>
/// FCE models do not include material definitions, and rely on triangle flags
/// to determine how each triangle would be rendered. By default, the base
/// material is a single textured diffuse map with semi-gloss effects applied.
/// </remarks>
[Flags]
public enum MaterialFlags : byte
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
    NoShading = 16,

    /// <summary>
    /// Identifies the material as including a red color channel component.
    /// </summary>
    RedChannel = 32,

    /// <summary>
    /// Identifies the material as including a green color channel component.
    /// </summary>
    GreenChannel = 64,

    /// <summary>
    /// Identifies the material as including a blue color channel component.
    /// </summary>
    BlueChannel = 128,

    /// <summary>
    /// Quick shortcut to indicate a white dummy with red, green and blue
    /// color components.
    /// </summary>
    WhiteDummy = NoShading | RedChannel | GreenChannel | BlueChannel,
}