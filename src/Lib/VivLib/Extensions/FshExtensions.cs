﻿using SixLabors.ImageSharp;
using System.Diagnostics.CodeAnalysis;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Contains a series of extension methods for the <see cref="FshFile"/>
/// class.
/// </summary>
public static class FshExtensions
{
    private static readonly IEnumerable<(Func<string?, bool> FailsValidation, string ErrorMessage)> GimxIdValidationRules = [
        (p => p.IsEmpty(), "FSH requires blobs to include a 4 character ID."),
        (p => !p!.ToLowerInvariant().All("abcdefghijklmnopqrstuvwxyz1234567890".Contains), "FSH blob ID must include letters and/or digits only."),
        (p => p!.Length != 4, "FSH blob ID length mismatch. Please specify a 4 character long ID."),
    ];

    /// <summary>
    /// Returns a color palette from the FSH file if one is present.
    /// </summary>
    /// <param name="fsh">FSH file to read the color palette from.</param>
    /// <returns>
    /// An array of colors that represent the loaded color palette.
    /// </returns>
    public static Color[]? GetPalette(this FshFile fsh)
    {
        return fsh.Entries.Values.FirstOrDefault(p => p.Magic == FshBlobFormat.Palette32) is { } palette
            ? ReadColors(palette.PixelData, palette.Width).ToArray()
            : null;
    }

    /// <summary>
    /// Returns a value that indicates if the GIMX ID is valid.
    /// </summary>
    /// <param name="id"><see cref="string"/> to verify.</param>
    /// <param name="errorMessage">
    /// Output parameter. Error message produced by the validation. It will be
    /// <see langword="null"/> if all validations pass.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the string is a valid GIMX ID,
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsGimxIdInvalid([NotNullWhen(false)] string? id, [NotNullWhen(true)] out string? errorMessage)
    {
        foreach (var (FailsValidation, ErrorMessage) in GimxIdValidationRules)
        {
            if (FailsValidation(id))
            {
                errorMessage = ErrorMessage; return true;
            }
        }
        errorMessage = null;
        return false;
    }

    /// <summary>
    /// Returns a value that indicates if the GIMX ID can be used to identify a
    /// new GIMX texture in the FSH file.
    /// </summary>
    /// <param name="newId">
    /// <see cref="string"/> to use as the GIMX id for a new texture.
    /// </param>
    /// <param name="fsh">FSH file to check for duplicates.</param>
    /// <param name="errorMessage">
    /// Output parameter. Error message produced by the validation. It will be
    /// <see langword="null"/> if all validations pass.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the string is a valid GIMX ID and can be used
    /// to identify a new GIMX texture inside the FSH file,
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsNewGimxIdInvalid(string? newId, FshFile fsh, [NotNullWhen(true)] out string? errorMessage)
    {
        if (IsGimxIdInvalid(newId, out errorMessage)) return true;
        if (fsh.Entries.ContainsKey(newId!))
        {
            errorMessage = "The specified ID is already in use.";
        }
        return !errorMessage.IsEmpty();
    }

    public static IEnumerable<Color> ReadColors(byte[] data, int paletteSize)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        while (paletteSize-- > 0)
        {
            var b = reader.ReadByte();
            var g = reader.ReadByte();
            var r = reader.ReadByte();
            var a = reader.ReadByte();
            yield return Color.FromRgba(r, g, b, a);
        }
    }
}