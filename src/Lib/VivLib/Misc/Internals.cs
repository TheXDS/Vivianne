namespace TheXDS.Vivianne.Misc;

internal class Internals
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
}
