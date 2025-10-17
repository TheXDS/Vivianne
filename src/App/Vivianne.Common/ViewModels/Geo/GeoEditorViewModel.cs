using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.ViewModels.Base;
using FshVm = TheXDS.Vivianne.ViewModels.Fsh.FshEditorViewModel;

namespace TheXDS.Vivianne.ViewModels.Geo
{
    /// <summary>
    /// ViewModel that allows the user to preview and interact with a .GEO 3D
    /// model.
    /// </summary>
    public class GeoEditorViewModel : StatefulFileEditorViewModelBase<GeoEditorState, GeoFile>
    {
        private bool _refreshEnabled;
        private FshVm? _fshViewModel;

        /// <summary>
        /// Gets a reference to the command used to load an external FSH/QFS
        /// file as the texture dictionary for this .GEO model.
        /// </summary>
        public ICommand LoadExtenralFshCommand { get; }

        /// <summary>
        /// Gets a reference to the children ViewModel used to preview a loaded
        /// FSH/QFS file.
        /// </summary>
        public FshVm? FshPreviewViewModel
        { 
            get => _fshViewModel;
            private set => Change(ref _fshViewModel, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoEditorViewModel"/>
        /// class.
        /// </summary>
        public GeoEditorViewModel()
        {
            LoadExtenralFshCommand = new SimpleCommand(OnLoadExternalFsh);
        }

        private async Task OnLoadExternalFsh()
        {
            if (DialogService is not null && await DialogService.GetFileOpenPath(FileFilters.FshQfsOpenFileFilter) is { Success:true, Result: { } fshFile })
            {
                try
                {
                    State.FshFile = await ((ISerializer<FshFile>)new FshSerializer()).DeserializeAsync(System.IO.File.OpenRead(fshFile));
                    FshPreviewViewModel = new FshVm
                    {
                        State = new FshEditorState() { File = State.FshFile },
                        DialogService = DialogService,
                        NavigationService = NavigationService
                    };
                }
                catch (Exception ex)
                {
                    await DialogService.Error(ex);
                }
            }
        }

        /// <inheritdoc/>
        protected override Task OnCreated()
        {
            ObserveCollection(State.Parts);
            OnVisibleChanged(null, null, default);
            State.UnsavedChanges = false;
            State.Subscribe(OnVisibleChanged);
            _refreshEnabled = true;
            return base.OnCreated();
        }

        private void ObserveCollection(ObservableCollection<GeoPartListItem> collection)
        {
            foreach (var j in collection)
            {
                j.Subscribe(() => j.IsVisible, OnVisibleChanged);
            }
            collection.CollectionChanged += (_, e) =>
            {
                if (e.OldItems is not null) foreach (var j in e.OldItems.Cast<GeoPartListItem>()) j.Unsubscribe(() => j.IsVisible);
                if (e.NewItems is not null) foreach (var j in e.NewItems.Cast<GeoPartListItem>()) j.Subscribe(() => j.IsVisible, OnVisibleChanged);
            };
        }

        private void OnVisibleChanged(object? instance, PropertyInfo? property, PropertyChangeNotificationType notificationType)
        {
            if (!_refreshEnabled) return;
            _refreshEnabled = false;
            State.RenderTree = GeoRenderStateBuilder.Build(State);
            _refreshEnabled = true;
        }
    }
}
