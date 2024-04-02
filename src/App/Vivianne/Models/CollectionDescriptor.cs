namespace TheXDS.Vivianne.Models;

public class CollectionDescriptor
{
    public string Collection { get; set; }
    public double Maximum { get; set; }
    public string Message { get; set; }
    public double Minimum { get; set; }
    public double Step { get; set; }
    public double BarWidth { get; set; } = 40;
}
