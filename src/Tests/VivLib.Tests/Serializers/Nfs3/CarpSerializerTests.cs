#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Carp.Nfs3;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Serializers.Base;
using TheXDS.Vivianne.Serializers.Carp.Nfs3;

namespace TheXDS.Vivianne.Serializers.Nfs3;

public class CarpSerializerTests() : CarpSerializerTestsBase<CarpSerializer, CarPerf>("Nfs3.test.txt", SetAdditionalProps)
{
    private static void SetAdditionalProps(CarPerf carp)
    {
        carp.CarClass = CarClass.A;
    }
}
