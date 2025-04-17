using TheXDS.Vivianne.Info;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Base class for all FCE files independently of the game version.
/// </summary>
/// <typeparam name="TColor">
/// Type definition for the colors inside the file.
/// </typeparam>
/// <typeparam name="TPart">Type definition for the FCE parts contained in the FCE file.</typeparam>
public abstract class FceFileBase<TColor, TPart> : IFceFile<TPart> where TColor : IHsbColor where TPart : FcePart
{
    /// <inheritdoc/>
    public NfsVersion Version => VersionIdentifier.FceVersion(Magic);

    /// <inheritdoc/>
    public int Magic { get; set; }

    /// <inheritdoc/>
    public int Arts { get; set; }

    /// <inheritdoc/>
    public float XHalfSize { get; set; }

    /// <inheritdoc/>
    public float YHalfSize { get; set; }

    /// <inheritdoc/>
    public float ZHalfSize { get; set; }

    /// <inheritdoc/>
    public byte[] RsvdTable1 { get; set; } = [];

    /// <inheritdoc/>
    public byte[] RsvdTable2 { get; set; } = [];

    /// <inheritdoc/>
    public byte[] RsvdTable3 { get; set; } = [];

    /// <inheritdoc/>
    public IList<TPart> Parts { get; set; } = [];

    /// <inheritdoc/>
    public IList<FceDummy> Dummies { get; set; } = [];

    /// <inheritdoc/>
    public abstract IEnumerable<IHsbColor[]> Colors { get; }

    /// <summary>
    /// Gets a table containing all the defined primary colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.
    /// </remarks>
    public IList<TColor> PrimaryColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined secondary colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// If this table contains no elements, FCE renderers must use the primary
    /// color table for all secondary colors and should not add them to this
    /// table when saving the FCE file, unless the user edits this copy.
    /// <br/><br/>
    /// Also, when this table contains a single
    /// element, FCE renderers should use the single color defined in this
    /// table for all color combinations. 
    /// </remarks>
    public IList<TColor> SecondaryColors { get; set; } = [];

    /// <summary>
    /// Gets a part with the specified name.
    /// </summary>
    /// <param name="partName">Name of the part to get.</param>
    /// <returns>
    /// A part with the specified name, or <see langword="null"/> if no such
    /// part exists.
    /// </returns>
    public TPart? GetPart(string partName) => Parts.FirstOrDefault(p => p.Name == partName);

    /// <summary>
    /// Gets a dummy with the specified name.
    /// </summary>
    /// <param name="dummyName">Name of the dummy to get.</param>
    /// <returns>
    /// A dummy with the specified name, or <see langword="null"/> if no such
    /// part exists.
    /// </returns>
    public FceDummy? GetDummy(string dummyName) => Dummies.FirstOrDefault(p => p.Name == dummyName);
}