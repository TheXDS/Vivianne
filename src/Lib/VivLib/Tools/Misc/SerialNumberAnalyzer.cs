/*
using TheXDS.MCART.Math;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using Carp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Carp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using CarpS3 = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using CarpS4 = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;
using FeS3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using FeS4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;
*/

namespace TheXDS.Vivianne.Tools.Misc;

/*
public class SerialNumberAnalyzer
{
}

public abstract class SerialNumberAnalyzerBase
{
    protected static IFeData? ReadFeData(byte[] rawFeData)
    {
        return (VersionIdentifier.FeDataVersion(rawFeData) switch
        {
            NfsVersion.Nfs3 => new FeS3(),
            NfsVersion.Nfs4 => new FeS4(),
            _ => (IOutSerializer<IFeData>?)null
        })?.Deserialize(rawFeData);
    }

    protected static ICarPerf? ReadCarp(byte[] rawCarp)
    {
        return (VersionIdentifier.CarpVersion(rawCarp) switch
        {
            NfsVersion.Nfs3 => new CarpS3(),
            NfsVersion.Nfs4 => new CarpS4(),
            _ => (IOutSerializer<ICarPerf>?)null
        })?.Deserialize(rawCarp);
    }

    protected static ISerialNumberModel? Read(byte[] rawData)
    {
        return ReadFeData(rawData) ?? ReadCarp(rawData) as ISerialNumberModel;
    }
}

public class SerialNumberGetter : SerialNumberAnalyzerBase
{
    private SerialNumberGetter() { }

    public static ushort? GetSerialNumber(VivFile viv)
    {
        ArgumentNullException.ThrowIfNull(viv);
        ushort? serialNumber = null;
        foreach (var j in FeDataBase.KnownExtensions.Select(j => $"fedata{j}").Concat(((string[])["", "sim", "1", "2", "3"]).Select(p => $"carp{p}.txt")))
        {
            if (viv.Directory.TryGetValue(j, out var rawBytes) && Read(rawBytes) is { } entity)
            {
                serialNumber ??= entity.SerialNumber;
                if (entity.SerialNumber != serialNumber)
                {
                    return null;
                }
            }
        }
        return (serialNumber is not null && serialNumber > 0) ? serialNumber: null;
    }
}

/// <summary>
/// Implements a tool that will set the serialNumber number of all internal files in a VIV file.
/// </summary>
public class SerialNumberSetter : SerialNumberAnalyzerBase
{
    private SerialNumberSetter() { }

    /// <summary>
    /// Sets the serialNumber number for specific entries in the provided <see cref="VivFile"/> directory.
    /// </summary>
    /// <remarks>This method updates the serialNumber number for entries in the <paramref name="viv"/> directory
    /// that match known extensions or specific file names. If an entry is found and successfully parsed, its serialNumber
    /// number is updated, and the modified data is written back to the directory.</remarks>
    /// <param name="viv">The <see cref="VivFile"/> containing the directory to update. Cannot be <see langword="null"/>.</param>
    /// <param name="serialNumber">The serialNumber number to assign to the relevant entries.</param>
    public static void SetSerialNumber(VivFile viv, ushort serialNumber)
    {
        ArgumentNullException.ThrowIfNull(viv);
        ProcessEntries(viv, serialNumber, FeDataBase.KnownExtensions.Select(j => $"fedata{j}"), ReadFeData);
        ProcessEntries(viv, serialNumber, ["", "sim", "1", "2", "3"], ReadCarp);
    }

    private static void ProcessEntries<T>(VivFile viv, ushort serialNumber, IEnumerable<string> fileNames, Func<byte[], T?> parser) where T : notnull, ISerialNumberModel
    {
        foreach (var j in fileNames)
        {
            if (viv.Directory.TryGetValue(j, out var rawBytes) && parser.Invoke(rawBytes) is { } entity)
            {
                entity.SerialNumber = serialNumber;
                viv.Directory[j] = Write(entity);
            }
        }
    }

    private static byte[] Write(ISerialNumberModel entity)
    {
        return entity switch
        {
            Fe3 f => ((IInSerializer<Fe3>)new FeS3()).Serialize(f),
            Fe4 f => ((IInSerializer<Fe4>)new FeS4()).Serialize(f),
            Carp3 f => ((IInSerializer<Carp3>)new CarpS3()).Serialize(f),
            Carp4 f => ((IInSerializer<Carp4>)new CarpS4()).Serialize(f),
            _ => [],
        };
    }
}
*/