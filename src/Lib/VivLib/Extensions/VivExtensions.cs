using System.Diagnostics;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;
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
    public static string? GetFriendlyName(this IDictionary<string, byte[]> viv)
    {
        return viv.GetAnyFeData() is byte[] data ? GetCarNameFromFeData(data) : null;
    }

    /// <summary>
    /// Gets the first available FeData file from the VIV file as raw data.
    /// </summary>
    /// <param name="viv">VIV file to get any available FeData from.</param>
    /// <returns>
    /// A <c><see cref="byte"/></c> array with the raw contents of a FeData
    /// file that has been found inside the VIV file, or <see langword="null"/>
    /// if no FeData files could be found.
    /// </returns>
    public static byte[]? GetAnyFeData(this IDictionary<string, byte[]> viv)
    {
        foreach (var ext in FeDataBase.KnownExtensions)
        {
            if (viv.TryGetValue($"fedata{ext}", out var feData)) return feData;
        }
        return null;
    }

    private static string? GetCarNameFromFeData(byte[] feData)
    {
        try
        {
            if (feData[0] == 4)
            {
                return ((IOutSerializer<Models.Fe.Nfs4.FeData>)new Serializers.Fe.Nfs4.FeDataSerializer()).Deserialize(feData).CarName;
            }
            return ((IOutSerializer<Models.Fe.Nfs3.FeData>)new Serializers.Fe.Nfs3.FeDataSerializer()).Deserialize(feData).CarName;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            return null;
        }
    }
}
