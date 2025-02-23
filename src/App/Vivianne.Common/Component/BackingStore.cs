using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Ganymede.Models;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Component;

public class BackingStore<TFile, TSerializer>(IBackingStore backingStore) : IFileBackingStore<TFile>
    where TFile : new()
    where TSerializer : ISerializer<TFile>, new()
{
    private static readonly TSerializer Serializer = new();
    private readonly IBackingStore backingStore = backingStore;

    public string FileName { get; set; }

    public async Task<TFile?> ReadAsync()
    {
        return (await backingStore.ReadAsync(FileName)) is { } contents
            ? await Serializer.DeserializeAsync(contents)
            : default;
    }

    public Task<bool> WriteAsync(TFile file)
    {
        return FileName is not null ? PerformSave(file) : WriteNewAsync(file);
    }

    public async Task<bool> WriteNewAsync(TFile file)
    {
        var result = await backingStore.GetNewFileName(FileName);
        if (!result.Success) return false;
        return await PerformSave(file);
    }

    private async Task<bool> PerformSave(TFile file)
    {
        return await backingStore.WriteAsync(FileName, await Serializer.SerializeAsync(file));
    }
}