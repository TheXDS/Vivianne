using System.Numerics;
using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs4;
using Fce3 = TheXDS.Vivianne.Models.Fce.Nfs3.FceFile;
using Fce4 = TheXDS.Vivianne.Models.Fce.Nfs4.FceFile;
using H3 = TheXDS.Vivianne.Models.Fce.Nfs3.HsbColor;
using H4 = TheXDS.Vivianne.Models.Fce.Nfs4.HsbColor;

namespace TheXDS.Vivianne.Tools.Fce;

/// <summary>
/// Includes functions to convert between different FCE file formats.
/// </summary>
public static class FceConverter
{
    private static readonly string[] Nfs3toNfs4PartNames = [
        ":HB",
        ":HLFW",
        ":HRFW",
        ":HLRW",
        ":HRRW",
        ":MB",
        ":MRFW", // Yes, this is supposed to be backwards ¯\_(ツ)_/¯
        ":MLFW", // <---+
        ":MLRW",
        ":MRRW",
        ":LB",
        ":TB",
        ":OL"
    ];

    /// <summary>
    /// Converts a <see cref="Fce4"/> object to <see cref="Fce3"/>.
    /// </summary>
    /// <param name="fce">FCE file to convert.</param>
    /// <returns>
    /// A new <see cref="Fce3"/> with the same mesh data as the original
    /// <see cref="Fce4"/>, excluding the damaged car mesh, interior/driver
    /// hair color tables, animation table and reserved table contents.
    /// </returns>
    public static Fce3 ToNfs3(Fce4 fce)
    {
        var vertsCount = fce.Parts.Sum(p => p.Vertices.Length);
        return new()
        {
            Magic = 0,
            Arts = fce.Arts,
            Dummies = fce.Dummies,
            PrimaryColors = [.. fce.PrimaryColors.Select(ToNfs3Color)],
            SecondaryColors = [.. fce.SecondaryColors.Select(ToNfs3Color)],
            Parts = [.. fce.Parts.OrderBy(PartIndex).Cast<FcePart>()],
            RsvdTable1 = new byte[vertsCount * 32],
            RsvdTable2 = new byte[vertsCount * Marshal.SizeOf<Vector3>()],
            RsvdTable3 = new byte[vertsCount * Marshal.SizeOf<Vector3>()],
            Unk_0x1e04 = new byte[256],
            XHalfSize = fce.XHalfSize,
            YHalfSize = fce.YHalfSize,
            ZHalfSize = fce.ZHalfSize
        };
    }

    /// <summary>
    /// Converts a <see cref="Fce3"/> object to <see cref="Fce4"/> for Need For
    /// Speed 4.
    /// </summary>
    /// <param name="fce">FCE file to convert.</param>
    /// <returns>
    /// A new <see cref="Fce4"/> with the same mesh data as the original
    /// <see cref="Fce3"/>, mirroring the undamaged mesh data into the damaged
    /// mesh data, setting a sane default color table for interior and driver
    /// hair colors, excluding the reserved table contents, and setting the
    /// contents of the animation table to default values.
    /// </returns>
    /// <remarks>
    /// Reserved tables for Motor City Online FCE models have different lengths
    /// from what Need For Speed 4 uses. Therefore, a separate method to
    /// convert to MCO is required.
    /// </remarks>
    public static Fce4 ToNfs4(Fce3 fce)
    {
        var vertsCount = fce.Parts.Sum(p => p.Vertices.Length);
        return new()
        {
            Magic = BitConverter.ToInt32([0x14, 0x10, 0x10, 0x00]),
            Arts = fce.Arts,
            Dummies = fce.Dummies,
            PrimaryColors = [.. fce.PrimaryColors.Select(ToNfs4Color)],
            InteriorColors = [.. Enumerable.Range(0, fce.PrimaryColors.Count).Select(_ => new H4(0, 0, 64, 127))],
            SecondaryColors = [.. fce.SecondaryColors.Select(ToNfs4Color)],
            DriverHairColors = [.. Enumerable.Range(0, fce.PrimaryColors.Count).Select(_ => new H4(20, 16, 24, 127))],
            Parts = [.. fce.Parts.Select(ToFce4Part)],
            RsvdTable1 = new byte[vertsCount * 32],
            RsvdTable2 = new byte[vertsCount * Marshal.SizeOf<Vector3>()],
            RsvdTable3 = new byte[vertsCount * Marshal.SizeOf<Vector3>()],
            RsvdTable4 = new byte[vertsCount * 4],
            AnimationTable = new byte[vertsCount * 4],
            RsvdTable5 = new byte[vertsCount * 4],
            RsvdTable6 = new byte[fce.Parts.Sum(p => p.Triangles.Length) * 12],
            XHalfSize = fce.XHalfSize,
            YHalfSize = fce.YHalfSize,
            ZHalfSize = fce.ZHalfSize,
            Unk_0x0004 = 0,
            Unk_0x0924 = 0,
            Unk_0x0928 = new byte[256],
            Unk_0x1e28 = new byte[528],
        };
    }

    private static Fce4Part ToFce4Part(FcePart part, int index)
    {
        return new Fce4Part()
        {
            Name = Nfs3toNfs4PartNames[index],
            Origin = part.Origin,
            Normals = part.Normals,
            DamagedNormals = part.Normals,
            Vertices = part.Vertices,
            DamagedVertices = part.Vertices,
            Triangles = part.Triangles,
        };
    }

    private static int PartIndex(Fce4Part part)
    {
        var index = Nfs3toNfs4PartNames.FindIndexOf(part.Name);
        return index >= 0 ? index : Nfs3toNfs4PartNames.Length;
    }

    private static H3 ToNfs3Color(H4 color)
    {
        return new H3(color.Hue, color.Saturation, color.Brightness, color.Alpha);
    }

    private static H4 ToNfs4Color(H3 color)
    {
        return new H4((byte)color.Hue, (byte)color.Saturation, (byte)color.Brightness, (byte)color.Alpha);
    }
}