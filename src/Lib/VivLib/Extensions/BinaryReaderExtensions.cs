using System.IO;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.Extensions;

public static class BinaryReaderExtensions
{
    public static T[] MarshalReadArray<T>(this BinaryReader br, in int count) where T : struct
    {
        return Enumerable.Range(0, count).Select(_ => br.MarshalReadStruct<T>()).ToArray();
    }

    public static T[] MarshalReadArray<T>(this BinaryReader br, in int offset, in int count) where T : struct
    {
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
        return MarshalReadArray<T>(br, count);
    }

    public static byte[] ReadBytesAt(this BinaryReader br, in int offset, in int count)
    {
        br.BaseStream.Seek(offset, SeekOrigin.Begin);
        return br.ReadBytes(count);
    }
}
