using TheXDS.Vivianne.Models.Carp.Nfs4;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Serializers.Carp.Nfs4;


public class CarpSerializer : CarpSerializerBase<CarClass, CarPerf>
{
    protected override void ReadProps(CarPerf carp, Dictionary<int, string> fields)
    {
       carp.UndersteerGradient = TryDoubleKey(fields, 80);
    }
    protected override string GetExtraProps(CarPerf entity)
    {
        return $"""
            understeer gradient(80)
            {entity.UndersteerGradient}
            """;
    }
}
