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
/// <param name="backingStore">
/// Underlying physical raw backing store tu use when reading or writing
/// entities.
/// </param>
public class BackingStore<TFile, TSerializer>(IBackingStore backingStore) : IBackingStore<TFile>
    where TFile : notnull
    where TSerializer : notnull, ISerializer<TFile>, new()
{
    private static readonly TSerializer Serializer = new();
    private readonly IBackingStore backingStore = backingStore;

    /// <inheritdoc/>
    public string? FileName { get; set; }

    /// <inheritdoc/>
    public IBackingStore Store => backingStore;

    /// <inheritdoc/>
    public async Task<TFile?> ReadAsync()
    {
        return (await backingStore.ReadAsync(FileName ?? throw new InvalidOperationException())) is { } contents
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
        var result = await backingStore.GetNewFileName(FileName);
        if (!result.Success) return false;
        FileName = result.Result;
        return await PerformSave(file);
    }

    private async Task<bool> PerformSave(TFile file)
    {
        return await backingStore.WriteAsync(FileName ?? throw new TamperException(), await Serializer.SerializeAsync(file));
    }
}