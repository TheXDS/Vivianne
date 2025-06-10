using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Info.Bnk;

/// <summary>
/// Implements an information extractor for <see cref="BnkFile"/> entities.
/// </summary>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
public class BnkFileInfoExtractor(bool humanSize) : IEntityInfoExtractor<BnkFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(BnkFile entity)
    {
        return [
            string.Format("BNK format version: {0}", entity.FileVersion),
            string.Format("Declared streams: {0}", entity.Streams.Count),
            string.Format("Streams with PT headers: {0}", entity.Streams.NotNull().Count()),
            string.Format("Usable audio payload: {0}", entity.Streams.NotNull().Sum(p => p.SampleData.Length + (p.AltStream?.SampleData.Length ?? 0)).GetSize(humanSize)),
            string.Format("Total payload size: {0}", entity.PayloadSize.GetSize(humanSize)),
        ];
    }
}
