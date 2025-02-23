//using TheXDS.MCART.Helpers;
//using TheXDS.MCART.Types.Extensions;
//using TheXDS.Vivianne.Models;
//using St = TheXDS.Vivianne.Resources.Strings.Tools.FceCleanupTool;

//namespace TheXDS.Vivianne.Tools;

//internal static class FceCleanupAnalyzers
//{
//    public static FceCleanupResult? StrayPartNames(Fce3File fce)
//    {
//        var actualPartList = fce.Header.PartNames.Select(p => p.ToString()).Where(p => !p.IsEmpty()).ToArray();
//        var strayPartNameCount = actualPartList.Length - fce.Header.CarPartCount;
//        void RemoveBadNames(Fce3File f)
//        {
//            var h = f.Header;
//            _ = Enumerable.Range(fce.Header.CarPartCount, strayPartNameCount).Select<int, object>(i => h.PartNames[i] = FceAsciiBlob.Empty).ToArray();
//            f.Header = h;
//        }

//        void NameUnnamed(Fce3File f)
//        {
//            var h = f.Header;
//            for (var j = 0; j < fce.Header.CarPartCount; j++)
//            {
//                if (h.PartNames[j].ToString().IsEmpty())
//                {
//                    h.PartNames[j] = j < FceAsciiBlob.CommonPartNames.Length ? FceAsciiBlob.CommonPartNames[j] : Guid.NewGuid().ToString();
//                }
//            }
//            f.Header = h;
//        }

//        return strayPartNameCount switch
//        {
//            _ when strayPartNameCount > 0 => new FceCleanupResult(string.Format(St.StrayPartNames_1_Title, strayPartNameCount), St.StrayPartNames_1_Details, RemoveBadNames),
//            _ when strayPartNameCount < 0 => new FceCleanupResult(string.Format(St.StrayPartNames_2_Title, -strayPartNameCount), St.StrayPartNames_2_Details, NameUnnamed),
//            _ => null
//        };
//    }

//    public static FceCleanupResult? BadTriangleFlags(Fce3File fce)
//    {
//        static void CorrectFlags(Fce3File file)
//        {
//            for(var j = 0; j < file.Triangles.Length; j++)
//            {
//                file.Triangles[j].Flags = file.Triangles[j].Flags & (TriangleFlags)15;
//                if (file.Triangles[j].Flags.HasFlag((TriangleFlags)3))
//                {
//                    file.Triangles[j].Flags = file.Triangles[j].Flags & (TriangleFlags)12;
//                }
//            }
//        }

//        List<string> badPartNames = [];
//        int badTrisCount = 0;
//        foreach ((var part, var name) in fce.Values.Zip(fce.Keys))
//        {
//            var badFlagsCount = part.Triangles.Select(t => t.Flags).Count(p => p != (p & (TriangleFlags)15) || p.HasFlag((TriangleFlags)3));
//            badTrisCount += badFlagsCount;
//            if (badFlagsCount > 0)
//            {
//                badPartNames.Add(string.Format(St.BadTriangleFlags_comp,name, badFlagsCount));
//            }
//        }
//        return badPartNames.Count != 0 ? new FceCleanupResult(
//            string.Format(St.BadTriangleFlags_Title, badTrisCount),
//            string.Format(St.BadTriangleFlags_Details,string.Join(Environment.NewLine, badPartNames)), CorrectFlags) : null;
//    }

//    public static FceCleanupResult? HeaderDamage_Arts(Fce3File f)
//    {
//        return f.Header.Arts != 1 ? new FceCleanupResult(
//            St.HeaderDamage_Arts_Title,
//            St.HeaderDamage_Arts_Details,
//            f =>
//            {
//                var h = f.Header;
//                h.Arts = 1;
//                f.Header = h;
//            }) : null;
//    }

//    public static FceCleanupResult? HeaderDamage_DummyCount(Fce3File f)
//    {
//        return f.Header.DummyCount.IsBetween(0, 16) ? null : new FceCleanupResult(
//            St.HeaderDamage_DummyCount_Title,
//            St.HeaderDamage_DummyCount_Details, f =>
//            {
//                var h = f.Header;
//                h.DummyCount = 16;
//                f.Header = h;
//            });
//    }

//    public static FceCleanupResult? HeaderDamage_CarPartCount(Fce3File f)
//    {
//        return f.Header.CarPartCount.IsBetween(0, 64) ? null : new FceCleanupResult(
//            St.HeaderDamage_CarPartCount_Title,
//            St.HeaderDamage_CarPartCount_Details, f =>
//            {
//                var h = f.Header;
//                h.CarPartCount = 64;
//                f.Header = h;
//            });
//    }

//    public static FceCleanupResult? HeaderDamage_ColorCount(Fce3File f)
//    {
//        return f.Header.PrimaryColors.IsBetween(0, 16) ? null : new FceCleanupResult(
//            St.HeaderDamage_ColorCount_Title,
//            St.HeaderDamage_ColorCount_Details, f =>
//            {
//                var h = f.Header;
//                h.PrimaryColors = 16;
//                f.Header = h;
//            });
//    }

//    public static FceCleanupResult? HeaderDamage_ColorMismatch(Fce3File f)
//    {
//        return f.Header.PrimaryColors == f.Header.SecondaryColors ? null : new FceCleanupResult(
//            St.HeaderDamage_ColorMismatch_Title,
//            St.HeaderDamage_ColorMismatch_Details, f =>
//            {
//                var h = f.Header;
//                h.SecondaryColors = h.PrimaryColors;
//                f.Header = h;
//            });
//    }

//    public static FceCleanupResult? OrphanVertices(Fce3File fce)
//    {
//        return null;
//        //var allVerts = fce.Triangles.SelectMany(p => (int?[])[p.I1, p.I2, p.I3]).ToList();
//        //foreach (var j in fce.Values.SelectMany(p => p.Triangles))
//        //{
//        //    allVerts[j.I1] = null;
//        //    allVerts[j.I2] = null;
//        //    allVerts[j.I3] = null;
//        //}
//        //var orphanVerts = allVerts.NotNull().ToArray();



//        //return orphanVerts.Length > 0
//        //    ? new FceCleanupResult($"{orphanVerts} orphaned vertices", $"{orphanVerts.Length} vertices were found that did not belong to any triangle.")
//        //    : null;
//    }
//}
