using TheXDS.Vivianne.Info;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type that represents the
/// essential contents of an FCE file.
/// </summary>
public interface IFceFile
{
    /// <summary>
    /// Gets a value that indicates the game version that this FCE4 file is for.
    /// </summary>
    NfsVersion Version { get; }

    /// <summary>
    /// Gets the current magic header of the FCE file.
    /// </summary>
    int Magic { get; }

    /// <summary>
    /// Gets a value that indicates the number of arts contained in the
    /// FCE file.
    /// </summary>
    /// <remarks>
    /// For NFS3/4, this value should remain at <c>1</c>, unless the model
    /// supports non-zero texture pages (like, Cop models, certain special
    /// objects, etc.).
    /// </remarks>
    int Arts { get; }

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the X axis. 
    /// </summary>
    float XHalfSize { get; }

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the Y axis. 
    /// </summary>
    float YHalfSize { get; }

    /// <summary>
    /// Gets a value that, when multiplied by <c>2.0</c>, indicates the
    /// size of the model in the Z axis. 
    /// </summary>
    float ZHalfSize { get; }

    /// <summary>
    /// Gets a table containing all the Dummies.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>16</c> <see cref="FceAsciiBlob"/>
    /// elements.
    /// </remarks>
    IList<FceDummy> Dummies { get; }

    /// <summary>
    /// Gets the entire color table for the FCE file.
    /// </summary>
    IEnumerable<IHsbColor[]> Colors { get; }

    /// <summary>
    /// Gets or sets the contents of the first reserved data table.
    /// </summary>
    byte[] RsvdTable1 { get; set; }

    /// <summary>
    /// Gets or sets the contents of the second reserved data table.
    /// </summary>
    byte[] RsvdTable2 { get; set; }

    /// <summary>
    /// Gets or sets the contents of the third reserved data table.
    /// </summary>
    byte[] RsvdTable3 { get; set; }
}