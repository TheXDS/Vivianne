using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Extensions;

namespace TheXDS.Vivianne.Info.Fce;

/// <summary>
/// Implements an information extractor for <see cref="FceFile"/> entities.
/// </summary>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
/// <param name="showRsvdContents">
/// Indicates if the contents of the reserved tables shoudl be read and dumped.
/// </param>
public class Fce3InfoExtractor(bool humanSize, bool showRsvdContents) : IEntityInfoExtractor<FceFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(FceFile entity)
    {
        return [.. (string[])[
            string.Format("File signature: 0x{0:x8}", entity.Magic),
            string.Format("Number of arts: {0}", entity.Arts),
            string.Format("Bounding box size: X={0}, Y={1}, Z={2}", entity.XHalfSize * 2, entity.YHalfSize * 2, entity.ZHalfSize * 2),
            DumpTable(entity.RsvdTable1, "Reserved table 1"),
            DumpTable(entity.RsvdTable2, "Reserved table 2"),
            DumpTable(entity.RsvdTable3, "Reserved table 3"),
            string.Format("Primary colors: {0}", entity.PrimaryColors.Count),
            string.Format("Secondary colors: {0}", entity.SecondaryColors.Count),
            string.Format("Parts: {0}", entity.Parts.Count),
            string.Format("Dummies: {0}", entity.Dummies.Count),
            DumpTable(entity.Unk_0x1e04, "Unk table at 0x1e04"),
            ]];
    }

    private string DumpTable(byte[] table, string tableName)
    {
        return showRsvdContents
            ? string.Join(Environment.NewLine, ((string[])[string.Format("{0} contents:", tableName)]).Concat(ChunkUp(table, 40).Select(ToHex)))
            : string.Format("{0} size: {1}", tableName, table.Length.GetSize(humanSize));
    }

    private static byte[][] ChunkUp(byte[] data, int chunkSize)
    {
        int numberOfChunks = (data.Length + chunkSize - 1) / chunkSize;
        byte[][] chunks = new byte[numberOfChunks][];
        for (int i = 0; i < numberOfChunks; i++)
        {
            int currentChunkSize = Math.Min(chunkSize, data.Length - i * chunkSize);
            chunks[i] = new byte[currentChunkSize];
            Array.Copy(data, i * chunkSize, chunks[i], 0, currentChunkSize);
        }
        return chunks;
    }

    private static string ToHex(byte[] data)
    {
        return string.Join(" ", data.Select(p => p.ToString("X2")));
    }
}
