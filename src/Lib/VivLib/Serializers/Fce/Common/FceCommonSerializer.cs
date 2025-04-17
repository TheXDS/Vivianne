using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Serializers.Fce.Common;

/// <summary>
/// Implements a read-only serializer that reads FCE files into a common format
/// that can be used to extract basic information and mesh data.
/// </summary>
public class FceCommonSerializer : IOutSerializer<IFceFile<FcePart>>
{
    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">
    /// Thrown if trying to deserialize an FCE file that seems invalid or for
    /// an unrecognized game version.
    /// </exception>
    public IFceFile<FcePart> Deserialize(Stream stream)
    {
        return (VersionIdentifier.FceVersion(stream) switch
        {
            NfsVersion.Nfs3 => new Nfs3.FceSerializer(),
            NfsVersion.Nfs4 or NfsVersion.Mco => (IOutSerializer<IFceFile<FcePart>>)new Nfs4.FceSerializer(),
            _ => throw new NotSupportedException()
        }).Deserialize(stream);
    }
}
