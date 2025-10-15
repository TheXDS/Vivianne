namespace TheXDS.Vivianne.Models.Geo;

public class GeoFace
{
    public GeoMaterialFlags MaterialFlags { get; set; }
    public byte Vertex1 { get; set; }
    public byte Vertex2 { get; set; }
    public byte Vertex3 { get; set; }
    public byte Vertex4 { get; set; }
    public string TextureName { get; set; } = string.Empty;
}
