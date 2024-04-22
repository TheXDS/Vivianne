using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Models;

[StructLayout(LayoutKind.Sequential)]
public struct FceAsciiBlob
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] public byte[] Value;

    public FceAsciiBlob(string x) : this()
    {
        Value = [.. Encoding.Latin1.GetBytes(x), .. new byte[64 - x.Length]];
    }

    public static FceAsciiBlob Empty { get; } = new FceAsciiBlob() { Value = new byte[64] };

    public string ToString()
    {
        var end = Value.FindIndexOf<byte>(0);
        if (end == -1) end = 64;
        return Encoding.Latin1.GetString(Value[0..end]);
    }

    public static implicit operator string(FceAsciiBlob x) => x.ToString();
    public static implicit operator FceAsciiBlob(string x) => new FceAsciiBlob(x);
}
