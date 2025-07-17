using TheXDS.Vivianne.Info;

namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type that represents the
/// essential contents of an FCE file.
/// </summary>
/// <typeparam name="TPart">Type of part stored in this FCE file.</typeparam>
public interface IFceFile<TPart> : IFceFile where TPart : FcePart
{
    /// <summary>
    /// Gets a table containing all defined Parts in the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>64</c>
    /// <typeparamref name="TPart"/> elements.
    /// elements.
    /// </remarks>
    IList<TPart> Parts { get; }
}
