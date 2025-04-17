using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Nfs4;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Carp.Nfs4;
using TheXDS.Vivianne.Serializers.Fe.Nfs4;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Includes a set of methods used to sync changes between FeData files and
/// Carp.
/// </summary>
public static class FeData4SyncTool
{
    private static readonly string[] knownCarps = ["carp.txt", "carp1.txt", "carp2.txt", "carp3.txt", "carpsim.txt", "carpsim1.txt", "carpsim2.txt", "carpsim3.txt"];

    /// <summary>
    /// Syncs changes between all FeData files and Carp.
    /// </summary>
    /// <param name="source">
    /// Source <see cref="FeData"/> to sync all supported values from.
    /// </param>
    /// <param name="sourceExt">
    /// File extension of <paramref name="source"/>. Used to exclude it from
    /// sync destinations.
    /// </param>
    /// <param name="directory">
    /// Directory of the VIV file. Changes will be synced on all supported
    /// files inside the directory.
    /// </param>
    public static void Sync(FeData source, string sourceExt, IDictionary<string, byte[]> directory)
    {
        ISerializer<FeData> fs = new FeDataSerializer();
        ISerializer<CarPerf> cs = new CarpSerializer();
        foreach (var j in FeDataBase.KnownExtensions.ExceptFor(sourceExt))
        {
            if (directory.TryGetValue($"fedata{j}", out var content))
            {
                var f = fs.Deserialize(content);
                f.CarName = source.CarName;
                f.CarId = source.CarId;
                f.SerialNumber = source.SerialNumber;
                f.PoliceFlag = source.PoliceFlag;
                f.VehicleClass = source.VehicleClass;
                f.Upgradable = source.Upgradable;
                f.Roof = source.Roof;
                f.EngineLocation = source.EngineLocation;
                f.IsDlc = source.IsDlc;
                f.DefaultCompare = source.DefaultCompare;
                f.CompareUpg1 = source.CompareUpg1;
                f.CompareUpg2 = source.CompareUpg2;
                f.CompareUpg3 = source.CompareUpg3;
                f.IsBonus = source.IsBonus;
                directory[$"fedata{j}"] = fs.Serialize(f);
            }
        }
        foreach (var j in knownCarps)
        {
            if (directory.TryGetValue(j, out var carpContent))
            {
                var c = cs.Deserialize(carpContent);
                c.SerialNumber = source.SerialNumber;
                c.CarClass = source.VehicleClass;
                directory[j] = cs.Serialize(c);
            }
        }
    }

    /// <summary>
    /// Syncs all FeData files inside the specified <see cref="VivFile"/> with
    /// performance information from the specified <see cref="CarPerf"/>.
    /// </summary>
    /// <param name="source">Performance data source.</param>
    /// <param name="name">
    /// File name of <paramref name="source"/>. Used to exclude it from
    /// sync destinations.
    /// </param>
    /// <param name="directory">VIV Directory to modify.</param>
    public static void Sync(CarPerf source, string? name, IDictionary<string, byte[]> directory)
    {
        ISerializer<FeData> fs = new FeDataSerializer();
        ISerializer<CarPerf> cs = new CarpSerializer();
        foreach (var j in Mappings.FeDataToTextProvider)
        {
            if (directory.TryGetValue($"fedata{j.Key}", out var content))
            {
                var f = fs.Deserialize(content);
                f.SerialNumber = source.SerialNumber;
                f.VehicleClass = source.CarClass;

                // Only allow perf data sync from the stock carperf.
                if (name == "carp.txt")
                {
                    var perfDataSource = j.Value.Invoke(source);
                    f.Weight = perfDataSource.Weight;
                    f.TopSpeed = perfDataSource.TopSpeed;
                    f.Hp = perfDataSource.Power;
                    f.Torque = perfDataSource.Torque;
                    f.MaxEngineSpeed = perfDataSource.MaxRpm;
                    f.Tires = perfDataSource.Tires;
                    f.Gearbox = perfDataSource.Gearbox;
                    f.Accel0To60 = perfDataSource.Accel0To60;
                    f.Accel0To100 = perfDataSource.Accel0To100;
                }
                directory[$"fedata{j.Key}"] = fs.Serialize(f);
            }
        }
        foreach (string j in knownCarps.Except([name]).NotNull())
        {
            if (directory.TryGetValue(j, out var carpContent))
            {
                var c = cs.Deserialize(carpContent);
                c.SerialNumber = source.SerialNumber;
                c.CarClass = source.CarClass;
                directory[j] = cs.Serialize(c);
            }
        }
    }
}
