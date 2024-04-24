using System;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.Ganymede.Services;
using TheXDS.Vivianne.Serializers;
using System;
using TheXDS.Ganymede.Models;

namespace TheXDS.Vivianne.Tools;

public class FceTest : IVivianneTool
{
    public async Task Run(IDialogService dialogService, INavigationService navigationService)
    {
        var file = await dialogService.GetFileOpenPath("");
        if (!file.Success) return;

        var s = new FceSerializer();
        var fce = s.Deserialize(System.IO.File.OpenRead(file.Result));
        var names = fce.Header.PartNames.Select(p => p.ToString());
        await dialogService.Message(string.Join(Environment.NewLine, names));
    }
}
