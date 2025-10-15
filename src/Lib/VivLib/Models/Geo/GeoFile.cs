namespace TheXDS.Vivianne.Models.Geo;

public class GeoFile
{
    public int MagicNumber { get; set; }
    public int[] Unk_0x04 { get; set; }
    public long Unk_0x84 { get; set; }
    public List<GeoPart> Parts { get; set; } = [];
}
