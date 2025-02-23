using TheXDS.Vivianne.Models.Carp.Base;
using TheXDS.Vivianne.Models.Fe.Nfs3;

namespace TheXDS.Vivianne.Models.Carp.Nfs4;

/// <summary>
/// Carp data tailored specifically to NFS4.
/// </summary>
public class CarPerf : CarPerf<CarClass>
{
    public double UndersteerGradient { get; set; }
}
