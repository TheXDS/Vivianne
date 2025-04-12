namespace TheXDS.Vivianne.Misc;

/// <summary>
/// Includes common helpers functions to convert between different array types.
/// </summary>
public static class CommonHelpers
{
    /// <summary>
    /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="byte"/>[]</c>  array mapping the entire span of the
    /// input array.
    /// </returns>
    public static byte[] MapToByte(short[] inputArray)
    {
        var result = new byte[inputArray.Length * 2];
        Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
        return result;
    }

    /// <summary>
    /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="byte"/>[]</c>  array mapping the entire span of the
    /// input array.
    /// </returns>
    public static byte[] MapToByte(int[] inputArray)
    {
        var result = new byte[inputArray.Length * 4];
        Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
        return result;
    }

    /// <summary>
    /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="byte"/>[]</c>  array mapping the entire span of the
    /// input array.
    /// </returns>
    public static byte[] MapToByte(sbyte[] inputArray)
    {
        return [.. inputArray.Select(p => (byte)(p + 128))];
    }

    /// <summary>
    /// Maps the data in the <c><see cref="byte"/>[]</c> array as a
    /// <c><see cref="short"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="short"/>[]</c> array mapping the entire span of the
    /// original <c><see cref="byte"/>[]</c> array.
    /// </returns>
    public static short[] MapToInt16(byte[] inputArray)
    {
        var result = new short[inputArray.Length / 2];
        Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
        return result;
    }


    /// <summary>
    /// Maps the data in the <c><see cref="byte"/>[]</c> array as an
    /// <c><see cref="int"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="int"/>[]</c> array mapping the entire span of the
    /// original <c><see cref="byte"/>[]</c> array.
    /// </returns>
    public static int[] MapToInt32(byte[] inputArray)
    {
        var result = new int[inputArray.Length / 4];
        Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
        return result;
    }

    /// <summary>
    /// Maps the data in the <c><see cref="byte"/>[]</c> array as an
    /// <c><see cref="sbyte"/>[]</c> array.
    /// </summary>
    /// <param name="inputArray">Array to be mapped.</param>
    /// <returns>
    /// A <c><see cref="sbyte"/>[]</c> array mapping the span of the original
    /// <c><see cref="byte"/>[]</c> array.
    /// </returns>
    public static sbyte[] MaptoSByte(byte[] inputArray)
    {
        return [.. inputArray.Select(p => (sbyte)(p - 128))];
    }
}
