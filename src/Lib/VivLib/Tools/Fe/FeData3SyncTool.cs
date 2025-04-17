using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Carp.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Carp.Nfs3;
using TheXDS.Vivianne.Serializers.Fe.Nfs3;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Includes a set of methods used to sync changes between FeData files and
/// Carp.
/// </summary>
public static class FeData3SyncTool
{
    private static readonly string[] knownCarps = ["carp.txt", "carpsim.txt"];

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
                f.VehicleClass = source.VehicleClass;
                f.Seat = source.Seat;
                f.IsPolice = source.IsPolice;
                f.IsBonus = source.IsBonus;
                f.AvailableToAi = source.AvailableToAi;
                f.IsDlc = source.IsDlc;
                f.CarAccel = source.CarAccel;
                f.CarTopSpeed = source.CarTopSpeed;
                f.CarHandling = source.CarHandling;
                f.CarBraking = source.CarBraking;
                f.Unk_0x0c = source.Unk_0x0c;
                f.Unk_0x14 = source.Unk_0x14;
                f.Unk_0x16 = source.Unk_0x16;
                f.Unk_0x1a = source.Unk_0x1a;
                f.Unk_0x1c = source.Unk_0x1c;
                f.Unk_0x1e = source.Unk_0x1e;
                f.Unk_0x20 = source.Unk_0x20;
                f.Unk_0x22 = source.Unk_0x22;
                f.Unk_0x24 = source.Unk_0x24;
                f.Unk_0x26 = source.Unk_0x26;
                f.Unk_0x2c = source.Unk_0x2c;
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

                // Only allow perf data sync from the standard carperf.
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
