using System.Globalization;
using System.Windows.Media.Media3D;
using TheXDS.Vivianne.ValueConverters.Base;

namespace TheXDS.Vivianne.ValueConverters;

/// <summary>
/// Implements a value converter that calculates an opposite coordinate to the
/// input value for camera orientation.
/// </summary>
public class LookBackConverter : IOneWayValueConverter<Point3D, Point3D>
{
    /// <inheritdoc/>
    public Point3D Convert(Point3D value, object? parameter, CultureInfo? culture)
    {
        return (Point3D)(new Point3D(0, 0, 0) - value);
    }
}
