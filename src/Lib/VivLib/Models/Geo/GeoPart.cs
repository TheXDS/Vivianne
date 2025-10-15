using System.Numerics;
using TheXDS.Vivianne.Models.Shared;

namespace TheXDS.Vivianne.Models.Geo;

public class GeoPart : TridimensionalObjectBase
{
    public GeoFace[] Faces { get; set; } = [];
    public int Unk_0x14 { get; set; }
    public int Unk_0x18 { get; set; }
    public long Unk_0x1C { get; set; }
    public long Unk_0x24 { get; set; }
    public long Unk_0x2C { get; set; }
}
