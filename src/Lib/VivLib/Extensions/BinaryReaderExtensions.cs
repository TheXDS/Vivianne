using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes a set of extensions to the <see cref="BinaryReader"/> class.
/// </summary>
public static class BinaryReaderExtensions
{
    /// <summary>
    /// Reads a struct using Marshaling, allowing for endianness
    /// transformations to be applied.
    /// </summary>
    /// <typeparam name="T">Type of structure to read.</typeparam>
    /// <param name="reader">Reader instance to read the struct from.</param>
    /// <returns>A new struct of type <typeparamref name="T"/>.</returns>
    public static T MarshalReadStructExt<T>(this BinaryReader reader) where T : struct
    {
        var raw = reader.ReadBytes(Marshal.SizeOf<T>());
        using var ms = new MemoryStream(raw);
        foreach (var j in typeof(T).GetFields())
        {
            if (j.GetAttribute<EndiannessAttribute>() is { Value: var e })
            {
                switch (e)
                {
                    case Endianness.BigEndian when BitConverter.IsLittleEndian:
                    case Endianness.LittleEndian when !BitConverter.IsLittleEndian:
                        Array.Reverse(raw, (int)Marshal.OffsetOf<T>(j.Name), Marshal.SizeOf(j.FieldType));
                        break;
                }
            }
        }
        using var br = new BinaryReader(ms);
        return br.MarshalReadStruct<T>();
    }
}
