using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Contains a series of extension methods for the <see cref="VivFile"/>
/// class.
/// </summary>
public static class VivExtensions
{
    /// <summary>
    /// Gets a friendly, descriptive name for the VIV file extracted from
    /// FeData.
    /// </summary>
    /// <param name="viv">VIV file to get a friendly name for.</param>
    /// <returns>
    /// A friendly name for the VIV file extracted from any available FeData.
    /// If the VIV did not have any FeData files, <see langword="null"/> will
    /// be returned.
    /// </returns>
    public static string? GetFriendlyName(this VivFile viv)
    {
        foreach (var ext in FeData.KnownExtensions)
        {
            if (viv.TryGetValue($"fedata{ext}", out var fd)) return GetFromFeData(fd);
        }
        return null;
    }

    private static string GetFromFeData(byte[] feData)
    {
        return ((ISerializer<FeData>)new FeDataSerializer()).Deserialize(feData).CarName;
    }
}
