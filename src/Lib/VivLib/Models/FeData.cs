using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Contains useful members for FeData3 and FeData4 files.
/// </summary>
public static class FeData
{
    /// <summary>
    /// Enumerates all the known fedata file extensions.
    /// </summary>
    public static readonly string[] KnownExtensions = [".bri", ".eng", ".fre", ".ger", ".ita", ".spa", ".swe"];

    /// <summary>
    /// Gets a FeData deserializer for the given raw data.
    /// </summary>
    /// <param name="data">Data to deserialize.</param>
    /// <returns>
    /// A FeData deserializer for the given raw data, automatically selecting
    /// the proper serializer class to use for the file format.
    /// </returns>
    public static IOutSerializer<IFeData> GetSerializer(byte[] data)
    {
        return FeData4.IsValid(data) ? new FeData4Serializer() : new FeData3Serializer();
    }

    /// <summary>
    /// Gets a deserialized FeData instance from the given raw data.
    /// </summary>
    /// <param name="data">Data to deserialize</param>
    /// <returns>
    /// A game-agnostic FeData instance deserialized from the given raw data.
    /// </returns>
    public static IFeData GetFeData(byte[] data)
    {
        return GetSerializer(data).Deserialize(data);
    }
}
