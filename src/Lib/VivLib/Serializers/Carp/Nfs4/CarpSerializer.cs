using System.Globalization;
using TheXDS.Vivianne.Models.Carp.Nfs4;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Serializers.Carp.Nfs4;


/// <summary>
/// Implements a Carp serializer for NFS4 cars.
/// </summary>
public class CarpSerializer : CarpSerializerBase<CarClass, CarPerf>
{
    /// <inheritdoc/>
    protected override void ReadProps(CarPerf carp, Dictionary<int, string> fields)
    {
       carp.UndersteerGradient = TryDoubleKey(fields, 80);
    }

    /// <inheritdoc/>
    protected override string GetExtraProps(CarPerf entity)
    {
        return $"""
            understeer gradient(80)
            {entity.UndersteerGradient.ToString(CultureInfo.InvariantCulture)}
            """;
    }
}
