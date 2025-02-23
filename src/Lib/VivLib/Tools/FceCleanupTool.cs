//using TheXDS.MCART.Types.Extensions;
//using TheXDS.Vivianne.Models;

//namespace TheXDS.Vivianne.Tools;

///// <summary>
///// Represents a delegate that will analyze an <see cref="Fce3File"/> object and
///// return a result on the operation that needs to be performed to correct the
///// issue.
///// </summary>
///// <param name="file"><see cref="Fce3File"/> to be analyzed.</param>
///// <returns>
///// An <see cref="FceCleanupResult"/> containing a message that describes the
///// issue, as well as a callback that can be executed on the specified
///// <see cref="Fce3File"/> to resolve it. <see langword="null"/> may be returned
///// if the <see cref="Fce3File"/> seems valid for the analyzer.
///// </returns>
//public delegate FceCleanupResult? FceCleanupAnalyzerCallback(Fce3File file);

///// <summary>
///// Includes tools that can be used to cleanup invalid data on a
///// <see cref="Fce3File"/>.
///// </summary>
//public static class FceCleanupTool
//{
//    private static readonly FceCleanupAnalyzer[] _analyzers = [
//        new(FceCleanupAnalyzers.HeaderDamage_Arts),
//        new(FceCleanupAnalyzers.HeaderDamage_CarPartCount),
//        new(FceCleanupAnalyzers.HeaderDamage_DummyCount),
//        new(FceCleanupAnalyzers.HeaderDamage_ColorCount),
//        new(FceCleanupAnalyzers.HeaderDamage_ColorMismatch),
//        new(FceCleanupAnalyzers.StrayPartNames),
//        new(FceCleanupAnalyzers.BadTriangleFlags),
//    ];

//    /// <summary>
//    /// Runs a collection of analyzers on the specified <see cref="Fce3File"/>
//    /// and enumerates all the warnings for any issues found on it.
//    /// </summary>
//    /// <param name="fce"><see cref="Fce3File"/> to analyze.</param>
//    /// <returns>
//    /// An enumeration of all problems found on the specified
//    /// <see cref="Fce3File"/>, or an empty enumeration if no issues we found.
//    /// </returns>
//    public static IEnumerable<FceCleanupResult> GetWarnings(Fce3File fce)
//    {
//        return GetWarnings(fce, _analyzers);
//    }

//    /// <summary>
//    /// Runs a collection of analyzers on the specified <see cref="Fce3File"/>
//    /// and enumerates all the warnings for any issues found on it.
//    /// </summary>
//    /// <param name="fce"><see cref="Fce3File"/> to analyze.</param>
//    /// <param name="analyzers">
//    /// Collection of analysers to use when analyzing the specified
//    /// <see cref="Fce3File"/>.
//    /// </param>
//    /// <returns>
//    /// An enumeration of all problems found on the specified
//    /// <see cref="Fce3File"/>, or an empty enumeration if no issues we found.
//    /// </returns>
//    public static IEnumerable<FceCleanupResult> GetWarnings(Fce3File fce, params FceCleanupAnalyzer[] analyzers)
//    {
//        return analyzers.Select(p => p.Callback(fce)).NotNull();
//    }

//    /// <summary>
//    /// Asyncronously runs a collection of analyzers on the specified
//    /// <see cref="Fce3File"/> and enumerates all the warnings for any issues
//    /// found on it.
//    /// </summary>
//    /// <param name="fce"><see cref="Fce3File"/> to analyze.</param>
//    /// <param name="analyzers">
//    /// Collection of analysers to use when analyzing the specified
//    /// <see cref="Fce3File"/>.
//    /// </param>
//    /// <returns>
//    /// An async enumeration of all problems found on the specified
//    /// <see cref="Fce3File"/>, or an empty enumeration if no issues we found.
//    /// </returns>
//    public static async IAsyncEnumerable<FceCleanupResult?> GetWarningsAsync(Fce3File fce, params FceCleanupAnalyzer[] analyzers)
//    {
//        foreach (var j in analyzers)
//        {
//            yield return await Task.Run(() => j.Callback(fce));
//        }
//    }
//}
