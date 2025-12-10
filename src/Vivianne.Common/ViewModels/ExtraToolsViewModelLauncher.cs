using System.Collections.Generic;
using System.Linq;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;
using TheXDS.MCART.Component;
using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Tools;
using TheXDS.Vivianne.ViewModels.Base;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.ViewModels;

/// <summary>
/// Implements a ViewModel that creates a list of extra tools that can be run from the startup page of Vivianne.
/// </summary>
public class ExtraToolsViewModelLauncher : ViewModel, IViewModelLauncher
{
    /// <inheritdoc/>
    public string PageName => St.ExtraTools;

    /// <summary>
    /// Gets a collection of custom tools that can be launched from the startup
    /// view.
    /// </summary>
    public IEnumerable<ButtonInteraction> AdditionalInteractions { get; }
    
    /// <inheritdoc/>
    public bool IsActive { get; set; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExtraToolsViewModelLauncher"/> class.
    /// </summary>
    public ExtraToolsViewModelLauncher()
    {
        AdditionalInteractions = CreateToolsList();
    }

    private IEnumerable<ButtonInteraction> CreateToolsList()
    {
        return ReflectionHelpers
            .FindAllObjects<IVivianneTool>()
            .Select(x => new ButtonInteraction(new SimpleCommand(() => x.Run(DialogService!, NavigationService!)), x.ToolName));
    }
}
