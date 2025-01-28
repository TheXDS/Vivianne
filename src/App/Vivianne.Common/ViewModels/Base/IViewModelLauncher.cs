using System.Collections.Generic;
using TheXDS.Ganymede.Types;
using TheXDS.Ganymede.Types.Base;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// fines a set of members to be implemented by a type that serves as the
/// launchpad for ViewModels from the startup page.
/// </summary>
public interface IViewModelLauncher : IViewModel
{
    /// <summary>
    /// Gets the name for this page on the startup ViewModel.
    /// </summary>
    string PageName { get; }

    /// <summary>
    /// Gets a collection of additional interactions to be made available on
    /// the editor's launcher page.
    /// </summary>
    IEnumerable<ButtonInteraction> AdditionalInteractions { get; }
}
