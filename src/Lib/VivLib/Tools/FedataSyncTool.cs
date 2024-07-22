using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Includes a set of methods used to sync changes between FeData files and
/// Carp.
/// </summary>
public static class FedataSyncTool
{
    /// <summary>
    /// Syncs changes between FeData files and Carp.
    /// </summary>
    /// <param name="source">
    /// Source <see cref="FeData"/> to sync all supported values from.
    /// </param>
    /// <param name="sourceExt">
    /// File extension of <paramref name="source"/>. Used to exclude it from
    /// sync destinations.
    /// </param>
    /// <param name="vivDirectory">
    /// Directory of the VIV file. Changes will be synced on all supported
    /// files inside the directory.
    /// </param>
    public static void Sync(FeData source, string sourceExt, IDictionary<string, byte[]> vivDirectory)
    {
        ISerializer<FeData> fs = new FeDataSerializer();
        ISerializer<Carp> cs = new CarpSerializer();
        foreach (var j in FeData.KnownExtensions.ExceptFor(sourceExt))
        {
            if (vivDirectory.TryGetValue($"fedata{j}", out var content))
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
                f.IsDlcCar = source.IsDlcCar;
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
                vivDirectory[$"fedata{j}"] = fs.Serialize(f);
            }
        }
        if (vivDirectory.TryGetValue("source.txt", out var carpContent))
        {
            var c = cs.Deserialize(carpContent);
            c.SerialNumber = source.SerialNumber;
            c.CarClass = source.VehicleClass;
            vivDirectory["source.txt"] = cs.Serialize(c);
        }
    }

    /// <summary>
    /// Syncs all FeData files inside the specified <see cref="VivFile"/> with
    /// performance information from the specified <see cref="Carp"/>.
    /// </summary>
    /// <param name="source">Performance data source.</param>
    /// <param name="vivDirectory">VIV Directory to modify.</param>
    public static void Sync(Carp source, IDictionary<string, byte[]> vivDirectory)
    {
        ISerializer<FeData> fedataSerializer = new FeDataSerializer();
        ISerializer<Carp> carpSerializer = new CarpSerializer();
        foreach (var j in Mappings.FeDataToTextProvider)
        {
            if (vivDirectory.TryGetValue($"fedata{j.Key}", out var content))
            {
                var f = fedataSerializer.Deserialize(content);
                f.SerialNumber = (ushort)source.SerialNumber;
                f.VehicleClass = source.CarClass;
                var perfDataSource = j.Value(source);
                f.Weight = perfDataSource.Weight;
                f.TopSpeed = perfDataSource.TopSpeed;
                f.Hp = perfDataSource.Power;
                f.Torque = perfDataSource.Torque;
                f.MaxEngineSpeed = perfDataSource.MaxRpm;
                f.Tires = perfDataSource.Tires;
                f.Gearbox = perfDataSource.Gearbox;
                f.Accel0To60 = perfDataSource.Accel0To60;
                f.Accel0To100 = perfDataSource.Accel0To100;

                vivDirectory[$"fedata{j.Key}"] = fedataSerializer.Serialize(f);
            }
        }
    }
}
