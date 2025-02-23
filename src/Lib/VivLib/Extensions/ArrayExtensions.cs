namespace TheXDS.Vivianne.Extensions;

public static class ArrayExtensions
{
    public static IEnumerable<T> Wrapping<T>(this T[] array, int count)
    {
        var c = array.Length;
        for (var i = 0; i < count; i++)
        {
            yield return array[i % c];
        }
    }

    public static IEnumerable<T> SkipIfMore<T>(this ICollection<T> source, int itemsToSkip)
    {
        return source.Skip(source.Count > itemsToSkip ? itemsToSkip : 0);
    }
}
