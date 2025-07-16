using System.Diagnostics;
using System.Reflection;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Attributes;
using TheXDS.Vivianne.Models.Fe;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Serializers.Fe;

/// <summary>
/// Base class for FeData serializers.
/// </summary>
/// <typeparam name="T">Type of FeData to write.</typeparam>
public class FeDataSerializerBase<T> where T : IFeData
{
    /// <summary>
    /// Reads all of the string table entries into the specified data object.
    /// </summary>
    /// <param name="reader">Reader to use when reading the strings.</param>
    /// <param name="data">
    /// FeData object in which to read the string values.
    /// </param>
    /// <param name="offsets">String offsets table to use.</param>
    protected static void ReadStrings(BinaryReader reader, T data, uint[] offsets)
    {
        foreach (var j in typeof(T).GetProperties())
        {
            if (j.GetAttribute<OffsetTableIndexAttribute>() is { Value: int offset })
            {
                j.SetValue(data, SeekAndRead(reader, offsets[offset]));
            }
        }
    }

    /// <summary>
    /// Writes all of the string values from the FeData object into the string
    /// data pool and the offset table.
    /// </summary>
    /// <param name="offsetsWriter">
    /// Writer to use when writing string table offsets.
    /// </param>
    /// <param name="feData">
    /// FeData object from which to get the string values to write.
    /// </param>
    /// <param name="stringTableOffset">Offset of the string offsets table.</param>
    /// <returns>The entire contents of the string pool.</returns>
    protected static byte[] WriteStrings(BinaryWriter offsetsWriter, T feData, int stringTableOffset)
    {
        uint lastOffset = (uint)(stringTableOffset + (feData.StringEntries * 4));
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms))
        {
            foreach (var j in typeof(T).GetProperties().Select<PropertyInfo,(PropertyInfo property, OffsetTableIndexAttribute? index)>(p => (p, p.GetAttribute<OffsetTableIndexAttribute>())).Where(p => p.index is not null).OrderBy(p => p.index!.Value))
            {
                string value = j.property.GetValue(feData)?.ToString() ?? string.Empty;
                offsetsWriter.Write(lastOffset);
                bw.WriteNullTerminatedString(value, Encoding.Latin1);
                lastOffset += (uint)value.Length + 1;
            }
        }
        return ms.ToArray();
    }

    /// <summary>
    /// Reads the offsets from the FeData offset table.
    /// </summary>
    /// <param name="reader">Reader to use when reading the offset table.</param>
    /// <param name="entries">Number of entries to read.</param>
    /// <returns>An array of all the offsets from the string offset table.</returns>
    protected static uint[] GetOffsetTable(BinaryReader reader, ushort entries)
    {
        uint[] offsets = new uint[entries];
        for (int i = 0; i < entries; i++)
        {
            offsets[i] = reader.ReadUInt32();
        }
        return offsets;
    }

    private static string SeekAndRead(BinaryReader reader, uint offset)
    {
        if (offset > reader.BaseStream.Length)
        {
            Debug.Print(string.Format(St.FeDataSerializer_StringOutOfBounds, offset));
            return string.Empty;
        }
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        return reader.ReadNullTerminatedString(Encoding.Latin1);
    }
}
