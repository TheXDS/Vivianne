using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Represents a null-terminated string with up to 64 bytes in length.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FceAsciiBlob
{
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
        if (x.Length >= 64)
        {
            throw new ArgumentException("The maximum allowable string length for this blob is 63 bytes.", nameof(x));
        }
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
        if (end == -1) end = 63;
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
