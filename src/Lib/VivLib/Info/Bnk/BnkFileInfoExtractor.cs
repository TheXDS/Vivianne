using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.Info.Bnk.BnkStreamInfoExtractor;

namespace TheXDS.Vivianne.Info.Bnk;

/// <summary>
/// Implements an information extractor for <see cref="BnkFile"/> entities.
/// </summary>
public class BnkFileInfoExtractor : IEntityInfoExtractor<BnkFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(BnkFile entity, bool humanSize = true)
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

/// <summary>
/// Implements an information extractor for <see cref="BnkStream"/> entities.
/// </summary>
public class BnkStreamInfoExtractor : IEntityInfoExtractor<BnkStream>
{
    /// <inheritdoc/>
    public string[] GetInfo(BnkStream value, bool humanSize = true)
    {
        return [.. new string?[]{
            string.Format(St.BnkNfo_Duration, TimeSpan.FromSeconds((double)value.SampleData.Length / (value.SampleRate * value.BytesPerSample))),
            string.Format(St.BnkNfo_Samples, value.SampleData.Length / value.BytesPerSample),
            string.Format(St.BnkNfo_Channels, value.Channels),
            string.Format(St.BnkNfo_Format, value.BytesPerSample * 8, value.Compression ? "?" : "PCM"),
            string.Format(St.BnkNfo_SampleRate, value.SampleRate),
            string.Format(St.BnkNfo_Size, value.SampleData.Length.GetSize(humanSize)),
            value.AltStream is null ? null : St.BnkNfo_AltStream
        }.NotNull()];
    }
}
