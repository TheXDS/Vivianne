using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Fsh.Nfs3;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.Serializers.Fsh.Blobs;
using TheXDS.Vivianne.Tools;

namespace TheXDS.Vivianne.Data;

internal static class VivTemplates
{
    public static IEnumerable<KeyValuePair<string, Func<byte[]>>> Get()
    {
        yield return new("DASH.qfs", TemplateDashQfs);
    }

    private static byte[] TemplateDashQfs()
    {
        var cabinBlob = new FshBlob()
        {
            Magic = FshBlobFormat.Argb32, Footer = ((ISerializer<GaugeData>)new GaugeDataSerializer()).Serialize(new())
        };
        cabinBlob.ReplaceWith(new Image<Rgba32>(640, 480));
        var steerBlob = new FshBlob() { Magic = FshBlobFormat.Argb32, XRotation = 128, YRotation = 128, XPosition = 192, YPosition = 352 };
        steerBlob.ReplaceWith(new Image<Rgba32>(256, 256));

        var fsh = new FshFile();
        fsh.Entries.Add("0000", cabinBlob);
        fsh.Entries.Add("0001", steerBlob);
        return QfsCodec.Compress(((ISerializer<FshFile>)new FshSerializer()).Serialize(fsh));
    }
}
