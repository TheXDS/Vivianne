using TheXDS.Ganymede.Types.Base;
using TheXDS.Vivianne.Component;

namespace TheXDS.Vivianne.ViewModels.Base;

/// <summary>
/// Represents a view model that provides access to a backing store for reading
/// and writing files.
/// </summary>
/// <typeparam name="TFile">
/// The type of file managed by the backing store. This type must be
/// non-nullable.
/// </typeparam>
public interface IBackingStoreViewModel<TFile> : IViewModel where TFile : notnull
{
    /// <summary>
    /// Gets or initializes a reference to the backing store used to read and
    /// write files.
    /// </summary>
    IBackingStore<TFile>? BackingStore { get; init; }
}