using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

/// <summary>
/// Represents a null-terminated string with up to 64 bytes in length.
/// </summary>
/// <remarks>
/// For NFS3, car part names have no special meaning at all. However, it's
/// recommended to follow a certain standard established and used on NFS4:
/// <list type="table">
/// <listheader>
/// <term>Part name</term>
/// <description>Description</description>
/// </listheader>
/// <item>
/// <term>:HB</term>
/// <description>Car body (high detail)</description>
/// </item>
/// <item>
/// <term>:HLFW</term>
/// <description>Left front wheel (high detail)</description>
/// </item>
/// <item>
/// <term>:HRFW</term>
/// <description>Right front wheel (high detail)</description>
/// </item>
/// <item>
/// <term>:HLRW</term>
/// <description>Left rear wheel (high detail)</description>
/// </item>
/// <item>
/// <term>:HRRW</term>
/// <description>Right rear wheel (high detail)</description>
/// </item>
/// <item>
/// <term>:MB</term>
/// <description>Car body (medium detail)</description>
/// </item>
/// <item>
/// <term>:MLFW</term>
/// <description>Left front wheel (medium detail)</description>
/// </item>
/// <item>
/// <term>:MRFW</term>
/// <description>Right front wheel (medium detail)</description>
/// </item>
/// <item>
/// <term>:MLRW</term>
/// <description>Left rear wheel (medium detail)</description>
/// </item>
/// <item>
/// <term>:MRRW</term>
/// <description>Right rear wheel (medium detail)</description>
/// </item>
/// <item>
/// <term>:LB</term>
/// <description>Car with wheels (low detail)</description>
/// </item>
/// <item>
/// <term>:TB</term>
/// <description>Car with wheels (super low detail, used for long distance and model collision)</description>
/// </item>
/// <item>
/// <term>:HH</term>
/// <description>Pop-up headlights (high detail)</description>
/// </item>
/// <item>
/// <term>:MH</term>
/// <description>Pop-up headlights (medium detail)</description>
/// </item>
/// </list>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public struct FceAsciiBlob
{
    /// <summary>
    /// Enumerates common part names, ordered in such a way that NFS3 would map them accordingly.
    /// </summary>
    public static readonly string[] CommonPartNames = [
        ":HB",
        ":HLFW",
        ":HRFW",
        ":HLRW",
        ":HRRW",
        ":MB",
        ":MLFW",
        ":MRFW",
        ":MLRW",
        ":MRRW",
        ":LB",
        ":TB",
        ":HH",
        ":MH"
        ];

    /// <summary>
    /// Gets the Raw 64-byte array from where to extract the name.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="FceAsciiBlob"/> struct.
    /// </summary>
    /// <param name="x">
    /// <see cref="string"/> to build the new <see cref="FceAsciiBlob"/> from.
    /// </param>
    public FceAsciiBlob(string x) : this()
    {
        Value = [.. Encoding.Latin1.GetBytes(x), .. new byte[64 - x.Length]];
    }

    /// <summary>
    /// Represents an empty <see cref="FceAsciiBlob"/> string.
    /// </summary>
    public static FceAsciiBlob Empty { get; } = new FceAsciiBlob() { Value = new byte[64] };

    /// <summary>
    /// Gets the <see cref="string"/> represented by this instance.
    /// </summary>
    /// <returns>
    /// A string with the car part name. Its length will be less than or equal
    /// to 64 bytes.
    /// </returns>
    public override readonly string ToString()
    {
        var end = Value.FindIndexOf<byte>(0);
        if (end == -1) end = 64;
        return Encoding.Latin1.GetString(Value[0..end]);
    }

    /// <summary>
    /// Implicitly converts a <see cref="FceAsciiBlob"/> instance into a
    /// <see cref="string"/>.
    /// </summary>
    /// <param name="x"><see cref="FceAsciiBlob"/> to convert.</param>
    public static implicit operator string(FceAsciiBlob x) => x.ToString();

    /// <summary>
    /// Implicitly converts a <see cref="string"/> instance into a
    /// <see cref="FceAsciiBlob"/>.
    /// </summary>
    /// <param name="x"><see cref="string"/> to convert.</param>
    public static implicit operator FceAsciiBlob(string x) => new(x);
}
