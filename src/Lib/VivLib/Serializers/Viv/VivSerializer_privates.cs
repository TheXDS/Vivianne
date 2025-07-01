using System.Text.RegularExpressions;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;

namespace TheXDS.Vivianne.Serializers.Viv;

public partial class VivSerializer : ISerializer<VivFile>
{
    private static readonly byte[] Header = "BIGF"u8.ToArray();

    private static int GetDirectoryOffset(Dictionary<string, byte[]> directory)
    {
        var sum = 16;
        foreach (var j in directory)
        {
            sum += j.Key.Length + 9;
        }
        return sum;
    }

    private IEnumerable<KeyValuePair<string, (int offset, int length)>> ApplySort(IEnumerable<KeyValuePair<string, (int offset, int length)>> dir)
    {
        return Sort?.Invoke() switch
        {
            SortType.FileName => dir.OrderBy(p => p.Key, StringComparer.InvariantCultureIgnoreCase),
            SortType.FileType => dir.OrderBy(p => Path.GetExtension(p.Key), StringComparer.InvariantCultureIgnoreCase).ThenBy(p => p.Key, StringComparer.InvariantCultureIgnoreCase),
            SortType.FileSize => dir.OrderByDescending(p => p.Value.length),
            SortType.FileKind => dir.OrderBy(GetFileKindOrdinal).ThenBy(p => p.Key, StringComparer.InvariantCultureIgnoreCase),
            SortType.FileOffset => dir.OrderBy(p => p.Value.offset),
            _ => dir
        };
    }

    private int GetFileKindOrdinal(KeyValuePair<string, (int offset, int length)> element)
    {
        string[][] kinds =
        [
            [".txt"],
            FeDataBase.KnownExtensions,
            [".fsh", ".qfs"],
            [".tga"],
            [".fce"],
            [".bnk"],
        ];
        var extension = Path.GetExtension(element.Key).ToLowerInvariant();
        var kind = kinds.WithIndex().FirstOrDefault(p => p.element.Contains(extension));
        return kind.element is not null ? kind.index : int.MaxValue;
    }

    private static string DedupName(string name)
    {
        var nameNoExt = Path.GetFileNameWithoutExtension(name);
        if (DupNameRegex().Match(nameNoExt) is { Success: true, Value: string toRemove })
        {
            return $"{nameNoExt.ChopEnd(toRemove)}{Path.GetExtension(name)}";
        }
        return name;
    }

    [GeneratedRegex(@" \([0-9]+\)$")]
    private static partial Regex DupNameRegex();
}
