using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Info.Bnk;

/// <summary>
/// Implements an information extractor for <see cref="MusFile"/> entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AsfFileInfoExtractor"/>
/// class.
/// </remarks>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
public class MusFileInfoExtractor(bool humanSize) : AsfFileInfoExtractor(humanSize), IEntityInfoExtractor<MusFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(MusFile value)
    {
        var jointSubStreams = AudioRender.JoinStreams(value.AsfSubStreams.Values);
        return GetInfo(jointSubStreams);
    }
}