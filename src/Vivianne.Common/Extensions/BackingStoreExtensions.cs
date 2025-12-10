using System.Threading.Tasks;
using TheXDS.Vivianne.Component;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes extension methods for the <see cref="IBackingStore"/> interface.
/// </summary>
internal static class BackingStoreExtensions
{
    /// <summary>
    /// Gets the first available FeData file from the VIV file as raw data.
    /// </summary>
    /// <param name="store">Data store to get any available FeData from.</param>
    /// <returns>
    /// A <c><see cref="IFeData"/></c> object with the Front-End Data
    /// file that has been found backing store, or <see langword="null"/>
    /// if no FeData files could be found.
    /// </returns>
    public static async Task<IFeData?> GetAnyFeData(this IBackingStore store)
    {
        foreach (var ext in FeDataBase.KnownExtensions)
        {
            if ((await store.ReadAsync($"fedata{ext}")) is { } data)
            {
                IOutSerializer<IFeData> serializer = data[0] == 4
                    ? new Serializers.Fe.Nfs4.FeDataSerializer()
                    : new Serializers.Fe.Nfs3.FeDataSerializer();
                return serializer.Deserialize(data);
            }
        }
        return null;
    }
}
