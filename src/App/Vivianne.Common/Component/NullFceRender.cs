using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a dummy, null FCE render service for when the service is not
/// registered on the target platform.
/// </summary>
public class NullFceRender : IStaticFceRender
{
    /// <inheritdoc/>
    public FshBlob RenderCompare(FceFile fce, byte[] textureData)
    {
        return new FshBlob();
    }
}
