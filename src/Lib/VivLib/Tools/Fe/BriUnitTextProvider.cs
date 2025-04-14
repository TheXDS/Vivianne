using System.Globalization;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Implements a <see cref="FeDataTextProvider"/> for BRI fedata files.
/// </summary>
/// <param name="carp">Performance data source.</param>
public class BriUnitTextProvider(ICarPerf carp) : FeDataTextProvider(carp, CultureInfo.GetCultureInfo("en-GB"))
{
    /// <inheritdoc/>
    public override string TopSpeed => $"{(CarpData.TopSpeed * 2.236936).ToString("0", Culture)} MPH";

    /// <inheritdoc/>
    public override string Weight => $"{(CarpData.Mass / 0.4535924).ToString("0", Culture)} lbs";

    /// <inheritdoc/>
    public override string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{(hp * 0.9862).ToString("0", Culture)} bhp @ {rpm.ToString(Culture)} RPM";
        }
    }
}
