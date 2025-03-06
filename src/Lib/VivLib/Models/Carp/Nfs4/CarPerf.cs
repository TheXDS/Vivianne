using TheXDS.Vivianne.Models.Carp.Base;
using TheXDS.Vivianne.Models.Fe.Nfs4;

namespace TheXDS.Vivianne.Models.Carp.Nfs4;

/// <summary>
/// Carp data tailored specifically to NFS4.
/// </summary>
public class CarPerf : CarPerf<CarClass>
{
    /// <summary>
    /// Gets or sets the "Understeer gradient" value.
    /// </summary>
    public double UndersteerGradient { get; set; }
}
