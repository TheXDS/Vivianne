using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Represents a delegate that will analyze an <see cref="FceFile"/> object and
/// return a result on the operation that needs to be performed to correct the
/// issue.
/// </summary>
/// <param name="file"><see cref="FceFile"/> to be analyzed.</param>
/// <returns>
/// An <see cref="FceCleanupResult"/> containing a message that describes the
/// issue, as well as a callback that can be executed on the specified
/// <see cref="FceFile"/> to resolve it. <see langword="null"/> may be returned
/// if the <see cref="FceFile"/> seems valid for the analyzer.
/// </returns>
public delegate FceCleanupResult? FceCleanupAnalyzerCallback(FceFile file);

/// <summary>
/// Represents a single FCE cleanup analysis function.
/// </summary>
/// <param name="Callback">
/// Delegate to be invoked when requesting information about the cleanup
/// operation to be performed.
/// </param>
public record class FceCleanupAnalyzer(FceCleanupAnalyzerCallback Callback);

/// <summary>
/// Represents the result of the analysis performed on an <see cref="FceFile"/>
/// that includes a message and an action to be performed to resolve the issue.
/// </summary>
/// <param name="Title">Short description of the issue that was found.</param>
/// <param name="Details">
/// Long description of the the issue that was found.
/// </param>
/// <param name="CleanupAction">
/// Delegate that can be invoked to resolve the issue that was found. Ideally,
/// this delegate shall be scoped to the issue described, and not perform any
/// other unwanted cleanup actions.
/// </param>
public record class FceCleanupResult(string Title, string Details, Action<FceFile> CleanupAction);

internal static class FceCleanupAnalyzers
{
    public static FceCleanupResult? StrayPartNames(FceFile fce)
    {
        var actualPartList = fce.Header.PartNames.Select(p => p.ToString()).Where(p => !p.IsEmpty()).ToArray();
        var strayPartNameCount = actualPartList.Length - fce.Header.CarPartCount;

        void RemoveBadNames(FceFile f) => _ = Enumerable.Range(strayPartNameCount, 64 - strayPartNameCount).Select<int, object>(i => f.Header.PartNames[i] = FceAsciiBlob.Empty);
        void NameUnnamed(FceFile f) => _ = Enumerable.Range(fce.Header.CarPartCount + strayPartNameCount, -strayPartNameCount).Select<int, object>(i => f.Header.PartNames[i] = Guid.NewGuid().ToString());


        return strayPartNameCount switch
        {
            _ when strayPartNameCount > 0 => new FceCleanupResult($"{strayPartNameCount} stray part name(s)", "", RemoveBadNames),
            _ when strayPartNameCount < 0 => new FceCleanupResult($"{-strayPartNameCount} unnamed part(s)", "", NameUnnamed),
            _ => null
        };
    }

    public static FceCleanupResult? OrphanVertices(FceFile fce)
    {
        return null;
        //var allVerts = fce.Triangles.SelectMany(p => (int?[])[p.I1, p.I2, p.I3]).ToList();
        //foreach (var j in fce.Values.SelectMany(p => p.Triangles))
        //{
        //    allVerts[j.I1] = null;
        //    allVerts[j.I2] = null;
        //    allVerts[j.I3] = null;
        //}
        //var orphanVerts = allVerts.NotNull().ToArray();



        //return orphanVerts.Length > 0
        //    ? new FceCleanupResult($"{orphanVerts} orphaned vertices", $"{orphanVerts.Length} vertices were found that did not belong to any triangle.")
        //    : null;
    }
}

/// <summary>
/// Includes tools that can be used to cleanup invalid data on a
/// <see cref="FceFile"/>.
/// </summary>
public static class FceCleanupTool
{
    private static readonly FceCleanupAnalyzer[] _analyzers = [
        new(FceCleanupAnalyzers.StrayPartNames)
    ];

    /// <summary>
    /// Runs a collection of analyzers on the specified <see cref="FceFile"/>
    /// and enumerates all the warnings for any issues found on it.
    /// </summary>
    /// <param name="fce"><see cref="FceFile"/> to analyze.</param>
    /// <returns>
    /// An enumeration of all problems found on the specified
    /// <see cref="FceFile"/>, or an empty enumeration if no issues we found.
    /// </returns>
    public static IEnumerable<FceCleanupResult> GetWarnings(FceFile fce)
    {
        return GetWarnings(fce, _analyzers);
    }

    /// <summary>
    /// Runs a collection of analyzers on the specified <see cref="FceFile"/>
    /// and enumerates all the warnings for any issues found on it.
    /// </summary>
    /// <param name="fce"><see cref="FceFile"/> to analyze.</param>
    /// <param name="analyzers">
    /// Collection of analysers to use when analyzing the specified
    /// <see cref="FceFile"/>.
    /// </param>
    /// <returns>
    /// An enumeration of all problems found on the specified
    /// <see cref="FceFile"/>, or an empty enumeration if no issues we found.
    /// </returns>
    public static IEnumerable<FceCleanupResult> GetWarnings(FceFile fce, params FceCleanupAnalyzer[] analyzers)
    {
        return analyzers.Select(p => p.Callback(fce)).NotNull();
    }
}
