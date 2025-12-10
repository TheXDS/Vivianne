using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Models.Fe;

/// <summary>
/// Represents a mutable copy of the car performance tables present in FeData4 files.
/// </summary>
public class MutableCompareTable : NotifyPropertyChanged
{
    private byte _Acceleration;
    private byte _TopSpeed;
    private byte _Handling;
    private byte _Braking;
    private byte _Overall;
    private int _Price;

    /// <summary>
    /// Creates a new instance of the <see cref="MutableCompareTable"/> class.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public static MutableCompareTable From(CompareTable table)
    {
        return new MutableCompareTable
        {
            Acceleration = table.Acceleration,
            TopSpeed = table.TopSpeed,
            Handling = table.Handling,
            Braking = table.Braking,
            Overall = table.Overall,
            Price = table.Price
        };
    }

    /// <summary>
    /// Gets or sets the value of the Acceleration property.
    /// </summary>
    /// <value>The value of the Acceleration property.</value>
    public byte Acceleration
    {
        get => _Acceleration;
        set => Change(ref _Acceleration, value);
    }

    /// <summary>
    /// Gets or sets the value of the TopSpeed property.
    /// </summary>
    /// <value>The value of the TopSpeed property.</value>
    public byte TopSpeed
    {
        get => _TopSpeed;
        set => Change(ref _TopSpeed, value);
    }

    /// <summary>
    /// Gets or sets the value of the Handling property.
    /// </summary>
    /// <value>The value of the Handling property.</value>
    public byte Handling
    {
        get => _Handling;
        set => Change(ref _Handling, value);
    }

    /// <summary>
    /// Gets or sets the value of the Braking property.
    /// </summary>
    /// <value>The value of the Braking property.</value>
    public byte Braking
    {
        get => _Braking;
        set => Change(ref _Braking, value);
    }

    /// <summary>
    /// Gets or sets the value of the Overall property.
    /// </summary>
    /// <value>The value of the Overall property.</value>
    public byte Overall
    {
        get => _Overall;
        set => Change(ref _Overall, value);
    }

    /// <summary>
    /// Gets or sets the value of the Price property.
    /// </summary>
    /// <value>The value of the Price property.</value>
    public int Price
    {
        get => _Price;
        set => Change(ref _Price, value);
    }

    /// <summary>
    /// Converts this instance into a <see cref="CompareTable"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="CompareTable"/> instance with the same values as this
    /// object.
    /// </returns>
    public CompareTable ToCompare()
    {
        return new CompareTable
        {
            Acceleration = Acceleration,
            TopSpeed = TopSpeed,
            Handling = Handling,
            Braking = Braking,
            Overall = Overall,
            Price = Price
        };
    }
}
