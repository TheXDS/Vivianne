using System;
using System.Threading.Tasks;
using TheXDS.MCART.Exceptions;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a backing store that will automatically serialize and
/// deserialize entities to be written to the raw physical underlying store.
/// </summary>
/// <typeparam name="TFile">Type of entities to be written.</typeparam>
/// <typeparam name="TSerializer">
/// Concrete type of serializer to use.
/// </typeparam>
public class BackingStore<TFile, TSerializer> : IBackingStore<TFile>
    where TFile : notnull
    where TSerializer : notnull, ISerializer<TFile>, new()
{
    private readonly TSerializer Serializer;
    private readonly IBackingStore _backingStore;

    /// <param name="backingStore">
    /// Underlying physical raw backing store tu use when reading or writing
    /// entities.
    /// </param>
    /// <param name="serializerConfigCallback"></param>
    public BackingStore(IBackingStore backingStore, Action<TSerializer>? serializerConfigCallback)
    {
        _backingStore = backingStore;
        Serializer = new TSerializer();
        serializerConfigCallback?.Invoke(Serializer);
    }

    /// <inheritdoc/>
    public string? FileName { get; set; }

    /// <inheritdoc/>
    public IBackingStore Store => _backingStore;

    /// <inheritdoc/>
    public async Task<TFile?> ReadAsync()
    {
        return (await _backingStore.ReadAsync(FileName ?? throw new InvalidOperationException())) is { } contents
            ? await Serializer.DeserializeAsync(contents)
            : default;
    }

    /// <inheritdoc/>
    public Task<bool> WriteAsync(TFile file)
    {
        return FileName is not null ? PerformSave(file) : WriteNewAsync(file);
    }

    /// <inheritdoc/>
    public async Task<bool> WriteNewAsync(TFile file)
    {
        var result = await _backingStore.GetNewFileName(FileName);
        if (!result.Success) return false;
        FileName = result.Result;
        return await PerformSave(file);
    }

    private async Task<bool> PerformSave(TFile file)
    {
        return await _backingStore.WriteAsync(FileName ?? throw new TamperException(), await Serializer.SerializeAsync(file));
    }
}
