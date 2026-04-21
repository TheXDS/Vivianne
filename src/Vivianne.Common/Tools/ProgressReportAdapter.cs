using System;
using Gn = TheXDS.Ganymede.Models.ProgressReport;
using Vv = TheXDS.Vivianne.Tools.Base.ProgressReport;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Adapts a <see cref="IProgress{Vv}" /> to an <see cref="IProgress{Gn}" />.
/// </summary>
/// <param name="progress">The progress reporter to adapt.</param>
public class ProgressReportAdapter(IProgress<Gn> progress) : IProgress<Vv>
{
    private readonly IProgress<Gn> _progress = progress;

    /// <summary>
    /// Reports the specified progress value.
    /// </summary>
    /// <param name="value">The progress value to report.</param>
    public void Report(Vv value)
    {
        _progress.Report(new Gn(value.Progress, value.Status));
    }
}
