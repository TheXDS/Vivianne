using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TheXDS.Vivianne.Models.Viv;

/// <summary>
/// Represents a VIV file that can be edited in memory.
/// </summary>
public class VivFile : IDictionary<string, byte[]>
{
    /// <inheritdoc/>
    public byte[] this[string key]
    {
        get => Directory.First(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Value;
        set => ((IDictionary<string, byte[]>)Directory)[key] = value;
    }

    /// <summary>
    /// Collection of files contained in this VIV.
    /// </summary>
    public Dictionary<string, byte[]> Directory { get; } = [];

    /// <inheritdoc/>
    public ICollection<string> Keys => ((IDictionary<string, byte[]>)Directory).Keys;

    /// <inheritdoc/>
    public ICollection<byte[]> Values => ((IDictionary<string, byte[]>)Directory).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<string, byte[]>>)Directory).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<string, byte[]>>)Directory).IsReadOnly;

    /// <inheritdoc/>
    public void Add(string key, byte[] value)
    {
        ((IDictionary<string, byte[]>)Directory).Add(key, value);
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<string, byte[]> item)
    {
        ((ICollection<KeyValuePair<string, byte[]>>)Directory).Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        ((ICollection<KeyValuePair<string, byte[]>>)Directory).Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<string, byte[]> item)
    {
        return ((ICollection<KeyValuePair<string, byte[]>>)Directory).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return Directory.Keys.Any(p => p.Equals(key, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<string, byte[]>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, byte[]>>)Directory).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, byte[]>>)Directory).GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(string key)
    {
        return Directory.FirstOrDefault(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)) is { } kv && Remove(kv);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<string, byte[]> item)
    {
        return ((ICollection<KeyValuePair<string, byte[]>>)Directory).Remove(item);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out byte[] value)
    {
        return ((IDictionary<string, byte[]>)Directory).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Directory).GetEnumerator();
    }
}