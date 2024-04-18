using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Component;

internal static class VivTemplates
{
    public static IEnumerable<KeyValuePair<string, Func<byte[]>>> Get()
    {
        yield return new("DASH.qfs", TemplateDashQfs);
    }

    private static byte[] TemplateDashQfs()
    {
        var cabinBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, GaugeData = new() };
        cabinBlob.ReplaceWith(new Image<Rgba32>(640, 480));
        cabinBlob.Footer = Mappings.FshFooterWriter[FshBlobFooterType.CarDashboard].Invoke(cabinBlob);
        var steerBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, XRotation = 128, YRotation = 128, XPosition = 192, YPosition = 352 };
        steerBlob.ReplaceWith(new Image<Rgba32>(256, 256));

        var fsh = new FshFile();
        fsh.Entries.Add("0000", cabinBlob);
        fsh.Entries.Add("0001", steerBlob);
        return QfsCodec.Compress(((ISerializer<FshFile>)new FshSerializer()).Serialize(fsh));
    }
}
