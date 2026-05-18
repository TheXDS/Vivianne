using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Base;

namespace TheXDS.Vivianne.Models.Bnk;

/// <summary>
/// Represents the editor state for a PT header within a BNK stream, providing
/// access to custom header properties.
/// </summary>
/// <remarks>
/// This class exposes dictionaries for both general and audio-specific custom
/// properties found in the PT header of the associated BNK stream. Use the
/// provided properties to view or modify header values as needed during
/// editing operations.
/// </remarks>
public class BnkPtHeaderEditorState : FileStateBase<BnkStream>
{
    private ObservableCollection<EditablePtHeaderItem>? _properties;
    private ObservableCollection<EditablePtHeaderItem>? _customAudioProperties;

    /// <summary>
    /// Gets or sets a dictionary with all custom properties on the PT
    /// header for this stream.
    /// </summary>
    public ObservableCollection<EditablePtHeaderItem> Properties => _properties ??= Create(File.Properties);

    /// <summary>
    /// Gets or sets a dictionary with all custom properties on the audio PT
    /// header for this stream.
    /// </summary>
    public ObservableCollection<EditablePtHeaderItem> CustomAudioProperties => _customAudioProperties ??= Create(File.CustomAudioProperties);

    private ObservableCollection<EditablePtHeaderItem> Create(IDictionary<byte, PtHeaderValue> dictionary)
    {
        var collection = new ObservableCollection<EditablePtHeaderItem>();
        void SetUnsavedChanges(object? o, PropertyInfo p, PropertyChangeNotificationType t) => UnsavedChanges = true;
        collection.CollectionChanged += (o, p) =>
        {
            UnsavedChanges = true;
            p.OldItems?.Cast<EditablePtHeaderItem>().ToList().ForEach(i => i.Unsubscribe(SetUnsavedChanges));
            p.NewItems?.Cast<EditablePtHeaderItem>().ToList().ForEach(i => i.Subscribe(SetUnsavedChanges));
        };
        foreach (var kvp in dictionary)
        {
            collection.Add(new EditablePtHeaderItem { Key = kvp.Key, Value = kvp.Value });
        }
        return collection;
    }
}
