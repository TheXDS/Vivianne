namespace TheXDS.Vivianne.Extensions;

/// <summary>
/// Includes a set of collection specific extensions
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Creates an array from the specified collection, ensuring that the new
    /// array will contain at least the specified number of elements.
    /// </summary>
    /// <typeparam name="T">Type of array to generate.</typeparam>
    /// <param name="collection">
    /// Elements to include starting at the beginning of the array.</param>
    /// <param name="size">Desired size of the array.</param>
    /// <param name="defaultValue">
    /// Value to set on each empty entry of the array.
    /// </param>
    /// <returns>
    /// An array of at least <paramref name="size"/> elements.
    /// </returns>
    public static T[] ArrayOfSize<T>(this IEnumerable<T> collection, int size, T defaultValue = default) where T : struct
    {
        var arr = collection.ToArray();
        var appendSize = size - arr.Length;
        return appendSize <= size
            ? [.. arr, .. Enumerable.Repeat(defaultValue, appendSize)]
            : [.. arr.Take(size)];
    }

    /// <summary>
    /// Enumerates the array wrapping the index around to the number of
    /// elements available.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to enumerate.</param>
    /// <param name="count">Number of elements to return.</param>
    /// <returns>
    /// An enumeration of all eleents of the array, wrapping around the index
    /// if the count is larger than the number of elements in the array.
    /// </returns>
    public static IEnumerable<T> Wrapping<T>(this T[] array, int count)
    {
        var c = array.Length;
        for (var i = 0; i < count; i++)
        {
            yield return array[i % c];
        }
    }

    /// <summary>
    /// Enumerates the elements of the collection in such a way that skips a
    /// number of elements only if the collection has enough elements to
    /// complete the skip successfully.
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection.</typeparam>
    /// <param name="source">Source collection.</param>
    /// <param name="itemsToSkip">
    /// Number of items to skip. If the collectino has less than this number of
    /// items, then no skip operation is performed.
    /// </param>
    /// <returns>
    /// An enumeration of the remaining items in the collection after skipping
    /// <paramref name="itemsToSkip"/> elements if the collection has enough
    /// items to perform the skip operation.
    /// </returns>
    public static IEnumerable<T> SkipIfMore<T>(this ICollection<T> source, int itemsToSkip)
    {
        return source.Skip(source.Count > itemsToSkip ? itemsToSkip : 0);
    }
}
