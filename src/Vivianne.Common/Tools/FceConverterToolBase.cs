using System;
using System.IO;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Base class for all FCE file converter tools.
/// </summary>
public abstract class FceConverterToolBase<TSource, TSourceSerializer, TDest, TDestSerializer>(string toolName, Func<TSource, TDest> converterCallback) : IVivianneTool
    where TSource : class, Models.Fce.Common.IFceFile, new()
    where TSourceSerializer : ISerializer<TSource>, new()
    where TDest : class, Models.Fce.Common.IFceFile, new()
    where TDestSerializer : IInSerializer<TDest>, new()
{
    string IVivianneTool.ToolName => toolName;

    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        if (await dialogService.GetFileOpenPath(FileFilters.FceFileFilter) is not { Success: true, Result: { } sourceFile }) return;
        if (await dialogService.GetFileSavePath(FileFilters.FceFileFilter) is not { Success: true, Result: { } destFile }) return;
        try
        {
            TSourceSerializer sourceSerializer = new();
            TDestSerializer destSerializer = new();
            using Stream sourceStream = File.OpenRead(sourceFile);
            TSource source = await sourceSerializer.DeserializeAsync(sourceStream);
            TDest dest = converterCallback.Invoke(source);
            using Stream destStream = File.OpenWrite(destFile);
            await destSerializer.SerializeToAsync(dest, destStream);

            await dialogService.Message("Conversion performed successfully.", $"""
                The conversion from {Mappings.FceMagicToString(source)} to {Mappings.FceMagicToString(dest)} has been performed successfully.
                """);
        }
        catch (Exception ex)
        {
            await dialogService.Error(ex);
        }
    }
}