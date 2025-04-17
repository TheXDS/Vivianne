using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools.Fce;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Implements a tool that allows the conversion of FCE4/FCE4M files to FCE3.
/// </summary>
public class Fce4ToFce3Converter : IVivianneTool
{
    string IVivianneTool.ToolName => "FCE4/FCE4M to FCE3 converter";

    /// <inheritdoc/>
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        if (await dialogService.GetFileOpenPath(FileFilters.FceFileFilter) is not { Success: true, Result: { } fce4RawFile }) return;
        if (await dialogService.GetFileSavePath(FileFilters.FceFileFilter) is not { Success: true, Result: { } fce3RawFile }) return;
        try
        {
            ISerializer<Models.Fce.Nfs4.FceFile> fce4Serializer = new Serializers.Fce.Nfs4.FceSerializer();
            using Stream Fce4Stream = File.OpenRead(fce4RawFile);
            Models.Fce.Nfs4.FceFile fce4File = await fce4Serializer.DeserializeAsync(Fce4Stream);
            Models.Fce.Nfs3.FceFile fce3File = FceConverter.ToNfs3(fce4File);
            ISerializer<Models.Fce.Nfs3.FceFile> fce3Serializer = new Serializers.Fce.Nfs3.FceSerializer();
            using Stream fce3Stream = File.OpenWrite(fce3RawFile);
            await fce3Serializer.SerializeToAsync(fce3File, fce3Stream);
            await dialogService.Message("Conversion performed successfully.", $"""
                The conversion from {Mappings.FceMagicToString(fce4File)} file format has been performed successfully.
                """);
        }
        catch (Exception ex)
        {
            await dialogService.Error(ex);
        }
    }

}
