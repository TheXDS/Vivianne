using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Tools;

public record class FceCleanupAnalyzer(Func<FceFile, FceCleanupResult?> Details);

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

public static class FceCleanupTool
{
    private static FceCleanupAnalyzer[] analyzers = [
        new(FceCleanupAnalyzers.StrayPartNames)
    ];



    public static IEnumerable<FceCleanupResult> GetWarnings(FceFile fce)
    {
        return analyzers.Select(p => p.Details(fce)).NotNull();
    }
}
