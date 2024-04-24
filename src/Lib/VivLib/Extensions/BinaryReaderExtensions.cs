using System.IO;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes a set of extensions to the <see cref="BinaryReader"/> class.
/// </summary>
public static class BinaryReaderExtensions
{
    /// <summary>
    /// Reads an array of type <typeparamref name="T"/> by marshaling.
    /// </summary>
    /// <typeparam name="T">
    /// Type of elements to be read. Must be a <see langword="struct"/>
    /// readable by marshaling.
    /// </typeparam>
    /// <param name="br">
    /// <see cref="BinaryReader"/> to be used for reading.
    /// </param>
    /// <param name="count">Number of elements to be read.</param>
    /// <returns>
    /// An array of <typeparamref name="T"/> containing
    /// <paramref name="count"/> elements.
    /// </returns>
    public static T[] MarshalReadArray<T>(this BinaryReader br, in int count) where T : struct
    {
        return Enumerable.Range(0, count).Select(_ => br.MarshalReadStruct<T>()).ToArray();
    }

    /// <summary>
    /// Moves to a specific location in the <see cref="Stream"/> being read by
    /// the <see cref="BinaryReader"/> and reads an array of type
    /// <typeparamref name="T"/> by marshaling.
    /// </summary>
    /// <typeparam name="T">
    /// Type of elements to be read. Must be a <see langword="struct"/>
    /// readable by marshaling.
    /// </typeparam>
    /// <param name="br">
    /// <see cref="BinaryReader"/> to be used for reading.
    /// </param>
    /// <param name="offset">
    /// Offset from which to start reading the array.</param>
    /// <param name="count">Number of elements to be read.
    /// </param>
    /// <returns>
    /// An array of <typeparamref name="T"/> containing
    /// <paramref name="count"/> elements.
    /// </returns>
    public static T[] MarshalReadArray<T>(this BinaryReader br, in int offset, in int count) where T : struct
    {
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
        return MarshalReadArray<T>(br, count);
    }

    /// <summary>
    /// Reads an array of bytes starting on the specified offset.
    /// </summary>
    /// <param name="br">
    /// <see cref="BinaryReader"/> to be used for reading.
    /// </param>
    /// <param name="offset">
    /// Offset from which to start reading the array.
    /// </param>
    /// <param name="count">Number of bytes to be read.</param>
    /// <returns>
    /// An array with <paramref name="count"/> bytes that has been read
    /// starting from <paramref name="offset"/>.
    /// </returns>
    public static byte[] ReadBytesAt(this BinaryReader br, in int offset, in int count)
    {
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
        return br.ReadBytes(count);
    }
}
