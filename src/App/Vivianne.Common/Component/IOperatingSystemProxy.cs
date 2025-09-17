namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes operating
/// system related information.
/// </summary>
public interface IOperatingSystemProxy
{
    /// <summary>
    /// Gets a value that indicates whether the current process is running with
    /// elevated privileges.
    /// </summary>
    bool IsElevated { get; }
}