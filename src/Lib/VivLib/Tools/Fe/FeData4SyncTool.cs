﻿using TheXDS.MCART.Types.Extensions;
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
    /// <param name="vivDirectory">
    /// Directory of the VIV file. Changes will be synced on all supported
    /// files inside the directory.
    /// </param>
    public static void Sync(FeData source, string sourceExt, IDictionary<string, byte[]> vivDirectory)
    {
        ISerializer<FeData> fs = new FeDataSerializer();
        ISerializer<CarPerf> cs = new CarpSerializer();
        foreach (var j in FeDataBase.KnownExtensions.ExceptFor(sourceExt))
        {
            if (vivDirectory.TryGetValue($"fedata{j}", out var content))
            {
                var f = fs.Deserialize(content);
                f.CarName = source.CarName;
                f.CarId = source.CarId;
                f.SerialNumber = source.SerialNumber;
                f.PoliceFlag = source.PoliceFlag;
                f.VehicleClass = source.VehicleClass;
                f.Upgradable = source.Upgradable;
                f.Roof = source.Roof;
                f.DefaultCompare = source.DefaultCompare;
                f.CompareUpg1 = source.CompareUpg1;
                f.CompareUpg2 = source.CompareUpg2;
                f.CompareUpg3 = source.CompareUpg3;
                f.IsBonus = source.IsBonus;
                vivDirectory[$"fedata{j}"] = fs.Serialize(f);
            }
        }
        if (vivDirectory.TryGetValue("carp.txt", out var carpContent))
        {
            var c = cs.Deserialize(carpContent);
            c.SerialNumber = source.SerialNumber;
            c.CarClass = source.VehicleClass;
            vivDirectory["carp.txt"] = cs.Serialize(c);
        }
    }

    /// <summary>
    /// Syncs all FeData files inside the specified <see cref="VivFile"/> with
    /// performance information from the specified <see cref="CarPerf"/>.
    /// </summary>
    /// <param name="source">Performance data source.</param>
    /// <param name="vivDirectory">VIV Directory to modify.</param>
    public static void Sync(CarPerf source, IDictionary<string, byte[]> vivDirectory)
    {
        ISerializer<FeData> serializer = new FeDataSerializer();
        foreach (var j in Mappings.FeDataToTextProvider)
        {
            if (vivDirectory.TryGetValue($"fedata{j.Key}", out var content))
            {
                var f = serializer.Deserialize(content);
                f.SerialNumber = source.SerialNumber;
                f.VehicleClass = source.CarClass;
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
                vivDirectory[$"fedata{j.Key}"] = serializer.Serialize(f);
            }
        }
    }
}
