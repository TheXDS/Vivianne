using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TheXDS.Vivianne.Misc;

public static class CommonHelpers
{
    public static byte[] MapToByte(short[] inputArray)
    {
        var result = new byte[inputArray.Length * 2];
        Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
        return result;
    }

    public static byte[] MapToByte(int[] inputArray)
    {
        var result = new byte[inputArray.Length * 4];
        Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
        return result;
    }

    public static byte[] MapToByte(sbyte[] inputArray)
    {
        return [.. inputArray.Select(p => (byte)(p + 128))];
    }

    public static short[] MapToInt16(byte[] inputArray)
    {
        var result = new short[inputArray.Length / 2];
        Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
        return result;
    }

    public static int[] MapToInt32(byte[] inputArray)
    {
        var result = new int[inputArray.Length / 4];
        Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
        return result;
    }
    public static sbyte[] MaptoSByte(byte[] inputArray)
    {
        return [.. inputArray.Select(p => (sbyte)(p - 128))];
    }
}
