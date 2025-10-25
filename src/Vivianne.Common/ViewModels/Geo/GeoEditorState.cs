using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using TheXDS.MCART.Types.Extensions;
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
        private WheelsState _wheelsState;
        private bool _spoilerDeployed;
        private string _fshName = "<none>";

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
        /// be rendered as being on (changes the texture defining the
        /// brakelights to one showing them as being on).
        /// </summary>
        public bool BrakelightsOn
        {
            get => _brakelightsOn;
            set => Change(ref _brakelightsOn, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates the state in which to render
        /// the wheels.
        /// </summary>
        public WheelsState WheelsState
        {
            get => _wheelsState;
            set => Change(ref _wheelsState, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the active spoiler
        /// should be rendered as deployed or not.
        /// </summary>
        public bool SpoilerDeployed
        {
            get => _spoilerDeployed;
            set => Change(ref _spoilerDeployed, value);
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
        /// Gets or sets the name of the FSH file that has been loaded into the
        /// editor.
        /// </summary>
        public string FshName
        {
            get => _fshName;
            set => Change(ref _fshName, value);
        }

        /// <summary>
        /// Gets a collection of all available elements from the FCE file.
        /// </summary>
        public ObservableCollection<GeoPartListItem> Parts => _parts ??= [.. GetListItem(File.Parts.NotNull())];

        /// <summary>
        /// Calculates a <see cref="Vector3"/> that can be used to center the entire model.
        /// </summary>
        public Vector3 GetCenteringVector()
        {
            var verts = Parts.SelectMany(p => p.Part.TransformedVertices).ToArray();
            var minX = verts.Min(p => p.X);
            var minY = verts.Min(p => p.Y);
            var minZ = verts.Min(p => p.Z);
            float sizeX = verts.Max(p => p.X) - minX;
            float sizeY = verts.Max(p => p.Y) - minY;
            float sizeZ = verts.Max(p => p.Z) - minZ;
            var xDiff = minX + (sizeX / 2);
            var yDiff = minY + (sizeY / 2);
            var zDiff = minZ + (sizeZ / 2);
            return new Vector3(xDiff, yDiff, zDiff);
        }

        private static IEnumerable<GeoPartListItem> GetListItem(IEnumerable<GeoPart> elements)
        {
            return elements.Select((p, i) => new GeoPartListItem(p, i));
        }
    }
}
