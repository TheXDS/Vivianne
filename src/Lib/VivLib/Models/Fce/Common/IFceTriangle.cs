namespace TheXDS.Vivianne.Models.Fce.Common;

public interface IFceTriangle
{
    int I1 { get; set; }
    int I2 { get; set; }
    int I3 { get; set; }
    float U1 { get; }
    float U2 { get; }
    float U3 { get; }
    float V1 { get; }
    float V2 { get; }
    float V3 { get; }
}