using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fsh;
using TheXDS.Vivianne.Models.Geo;

namespace TheXDS.Vivianne.ViewModels.Geo
{
    /// <summary>
    /// Represents the current state of the <see cref="GeoEditorViewModel"/>.
    /// </summary>
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
}
