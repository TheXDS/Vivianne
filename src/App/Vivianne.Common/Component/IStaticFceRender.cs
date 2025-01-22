using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes
/// functionality to render FCE files for different purposes.
/// </summary>
public interface IStaticFceRender
{
    /// <summary>
    /// Renders an FCE from the front, and generates a compare image suitable
    /// to be injected into the <c>compare.qfs</c> file.
    /// </summary>
    /// <param name="fce">FCE model to render.</param>
    /// <param name="textureData">Texture to apply to the FCE file.</param>
    /// <returns>
    /// A <see cref="FshBlob"/> that can be added to the <c>compare.qfs</c>
    /// file.
    /// </returns>
    FshBlob RenderCompare(FceFile fce, byte[] textureData);
}