namespace TheXDS.Vivianne.Models.Fce.Nfs4;

/// <summary>
/// Represents an FCE car model.
/// </summary>
public class FceFile
{
    /// <summary>
    /// GEts or sets the current magic header of the FCE file.
    /// </summary>
    public int Magic { get; set; }

    /// <summary>
    /// Gets or sets the unknown 0x0004 value.
    /// </summary>
    public int Unk_0x0004 { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the number of arts contained in the
    /// FCE file.
    /// </summary>
    /// <remarks>
    /// For NFS3, this value should remain at <c>1</c>, unless the model
    /// supports non-zero texture pages (like, Cop models, certain special
    /// objects, etc.).
    /// </remarks>
    public int Arts { get; set; }

    /// <summary>
    /// Gets or sets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the X axis. 
    /// </summary>
    public float XHalfSize { get; set; }

    /// <summary>
    /// Gets or sets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the Y axis. 
    /// </summary>
    public float YHalfSize { get; set; }

    /// <summary>
    /// Gets or sets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the Z axis. 
    /// </summary>
    public float ZHalfSize { get; set; }

    /// <summary>
    /// Gets or sets the contents of the first reserved data table.
    /// </summary>
    public byte[] RsvdTable1 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the second reserved data table.
    /// </summary>
    public byte[] RsvdTable2 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the third reserved data table.
    /// </summary>
    public byte[] RsvdTable3 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the fourth reserved data table.
    /// </summary>
    public byte[] RsvdTable4 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the fifth reserved data table.
    /// </summary>
    public byte[] RsvdTable5 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the sixth reserved data table.
    /// </summary>
    public byte[] RsvdTable6 { get; set; } = [];

    /// <summary>
    /// Gets or sets the contents of the animation table.
    /// </summary>
    public byte[] AnimationTable { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined primary colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.
    /// </remarks>
    public IList<HsbColor> PrimaryColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined interior colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// </remarks>
    public IList<HsbColor> InteriorColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined secondary colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// </remarks>
    public IList<HsbColor> SecondaryColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the defined driver hair colors for the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> entries, and its recommended
    /// to avoid exceeding <c>10</c> due to limitations in the related FeData
    /// file format for the color names.<br/><br/>
    /// </remarks>
    public IList<HsbColor> DriverHairColors { get; set; } = [];

    /// <summary>
    /// Gets a table containing all defined Parts in the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>64</c> <see cref="FcePart"/>
    /// elements.
    /// </remarks>
    public IList<FcePart> Parts { get; set; } = [];

    /// <summary>
    /// Gets a table containing all the Dummies.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> <see cref="FceAsciiBlob"/>
    /// elements.
    /// </remarks>
    public IList<FceDummy> Dummies { get; set; } = [];

    /// <summary>
    /// Gets or sets an integer value whose purpose is currently unknown.
    /// </summary>
    public int Unk_0x0924 { get; set; }

    /// <summary>
    /// Gets or sets a 256-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x0928 { get; set; } = [];

    /// <summary>
    /// Gets or sets a 528-byte table with data whose purpose is currently
    /// unknown.
    /// </summary>
    public byte[] Unk_0x1e28 { get; set; } = [];

    /// <summary>
    /// Gets a part with the specified name.
    /// </summary>
    /// <param name="partName">Name of the part to get.</param>
    /// <returns>
    /// A part with the specified name, or <see langword="null"/> if no such
    /// part exists.
    /// </returns>
    public FcePart? GetPart(string partName) => Parts.FirstOrDefault(p => p.Name == partName);

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
