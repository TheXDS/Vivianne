using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Ganymede.Types.Extensions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Base;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Resources;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Fsh;
using TheXDS.Vivianne.ViewModels.Base;

namespace TheXDS.Vivianne.ViewModels.Geo
{
    public class GeoEditorState : FileStateBase<GeoFile>
    {
        private ObservableCollection<GeoPartListItem>? _parts;
        private RenderState? _renderTree;
        private bool _brakelightsOn;
        private FshFile? _fshFile;

        /// <summary>
        /// Gets a reference to an object that describes the rendered scene.
        /// </summary>
        public RenderState? RenderTree
        {
            get => _renderTree;
            set => Change(ref _renderTree, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the brakelights should
        /// be rendered as being on (changes the object defining the
        /// brakelights to one referencing them as being on).
        /// </summary>
        public bool BrakelightsOn
        { 
            get => _brakelightsOn;
            set => Change(ref _brakelightsOn, value);
        }

        /// <summary>
        /// Gets or sets a reference to the FSH file that contains the textures
        /// used by this GEO model.
        /// </summary>
        public FshFile? FshFile
        { 
            get => _fshFile;
            set => Change(ref _fshFile, value);
        }

        /// <summary>
        /// Gets a collection of all available elements from the FCE file.
        /// </summary>
        public ObservableCollection<GeoPartListItem> Parts => _parts ??= [.. GetListItem(File.Parts)];

        private static IEnumerable<GeoPartListItem> GetListItem(IList<GeoPart> elements)
        {
            return elements.Select(p => new GeoPartListItem(p));
        }
    }

    public class GeoEditorViewModel : StatefulFileEditorViewModelBase<GeoEditorState, GeoFile>
    {
        private bool _refreshEnabled;
        private static readonly GeoRenderStateBuilder _render = new();
        private TheXDS.Vivianne.ViewModels.Fsh.FshEditorViewModel? _fshViewModel;

        public ICommand LoadExtenralFshCommand { get; }

        public GeoEditorViewModel()
        {
            LoadExtenralFshCommand = new SimpleCommand(OnLoadExternalFsh);
        }

        public TheXDS.Vivianne.ViewModels.Fsh.FshEditorViewModel? FshPreviewViewModel
        { 
            get => _fshViewModel;
            private set => Change(ref _fshViewModel, value);
        }

        private async Task OnLoadExternalFsh()
        {
            if (DialogService is not null && await DialogService.GetFileOpenPath(FileFilters.FshQfsOpenFileFilter) is { Success:true, Result: { } fshFile })
            {
                try
                {
                    ISerializer<FshFile> serializer = new FshSerializer();
                    State.FshFile = await serializer.DeserializeAsync(System.IO.File.OpenRead(fshFile));
                    FshPreviewViewModel = new TheXDS.Vivianne.ViewModels.Fsh.FshEditorViewModel
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
            State.RenderTree = _render.Build(State);
            _refreshEnabled = true;
        }
    }
}
