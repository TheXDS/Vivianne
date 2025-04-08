using TheXDS.MCART.Helpers;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes a set of miscellaneous extensions to a variety of classes.
/// </summary>
public static class MiscExtensions
{
    /// <summary>
    /// Gets a human-readable size for the amount of bytes specified.
    /// </summary>
    /// <param name="bytes">
    /// Size in bytes to turn into human-readable format.
    /// </param>
    /// <param name="humanReadable">
    /// If set to <see langword="true"/>, the string will be formatted. If set
    /// to <see langword="false"/>, the unaltered integer value will be
    /// returned as a string.</param>
    /// <returns>A string that contains a human-readable byte size amount.</returns>
    public static string GetSize(this int bytes, bool humanReadable = true) => humanReadable ? ((long)bytes).ByteUnits() : bytes.ToString();

    /// <summary>
    /// Gets a human-readable size for the amount of bytes specified.
    /// </summary>
    /// <param name="bytes">
    /// Size in bytes to turn into human-readable format.
    /// </param>
    /// <param name="humanReadable">
    /// If set to <see langword="true"/>, the string will be formatted. If set
    /// to <see langword="false"/>, the unaltered integer value will be
    /// returned as a string.</param>
    /// <returns>A string that contains a human-readable byte size amount.</returns>
    public static string GetSize(this long bytes, bool humanReadable = true) => humanReadable ? bytes.ByteUnits() : bytes.ToString();
}
