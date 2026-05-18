using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Resources;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Bnk;
using TheXDS.Vivianne.Serializers.Audio;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Bnk;

/// <summary>
/// ViewModel for the BNK PT header editor.
/// </summary>
public class BnkPtHeaderEditorViewModel : EditorViewModelBase<BnkPtHeaderEditorState>
{
    /// <summary>
    /// Gets a reference to the command used to add a custom PT header property.
    /// </summary>
    public ICommand AddCustomPtHeader { get; }

    /// <summary>
    /// Gets a reference to the command used to add a custom audio PT header
    /// property.
    /// </summary>
    public ICommand AddCustomAudioPtHeader { get; }

    /// <summary>
    /// Gets the command that removes the custom PT header from the associated
    /// BNK stream.
    /// </summary>
    public ICommand RemoveCustomPtHeader { get; }

    /// <summary>
    /// Gets the command that removes the custom audio PT header from the
    /// associated BNK stream.
    /// </summary>
    public ICommand RemoveCustomAudioPtHeader { get; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="BnkPtHeaderEditorViewModel"/> class with the specified
    /// editor state.
    /// </summary>
    /// <param name="state">
    /// The current state of the bank point header editor. Cannot be
    /// <see langword="null"/>.
    /// </param>
    public BnkPtHeaderEditorViewModel(BnkPtHeaderEditorState state) : base(state)
    {
        AddCustomPtHeader = new SimpleCommand(OnAddCustomPtHeaderAsync);
        AddCustomAudioPtHeader = new SimpleCommand(OnAddCustomAudioPtHeaderAsync);
    }

    private Task OnAddCustomPtHeaderAsync()
    {
        return OnAddPtHeaderAsync<PtHeaderField>("PT Header Key", State.Properties);
    }

    private Task OnAddCustomAudioPtHeaderAsync()
    {
        return OnAddPtHeaderAsync<PtAudioHeaderField>("Custom audio PT Header Key", State.CustomAudioProperties);
    }

    private async Task OnAddPtHeaderAsync<T>(string prompt, ObservableCollection<EditablePtHeaderItem> collection) where T : Enum
    {
        if (DialogService is not null && await DialogService.GetInputValue<byte>(CommonDialogTemplates.Input with { Title = prompt }, 1, 254) is { Success: true, Result: { } result })
        {
            if (collection.Any(p => p.Key == result))
            {
                await DialogService.Error("A PT Header with the specified key already exists.");
                return;
            }
            if (Enum.IsDefined(typeof(T), result))
            {
                await DialogService.Error("The specified key is a reserved or standard PT header, cannot add it.");
                return;
            }
            collection.Add(new EditablePtHeaderItem { Key = result, Value = 0 });
        }
    }

    /// <inheritdoc/>
    protected override Task OnSaveChanges()
    {
        State.File.Properties.Clear();
        State.File.CustomAudioProperties.Clear();
        State.File.Properties.AddRange(State.Properties.Select(p => p.ToKeyValuePair()));
        State.File.CustomAudioProperties.AddRange(State.CustomAudioProperties.Select(p => p.ToKeyValuePair()));
        return Task.CompletedTask;
    }
}
