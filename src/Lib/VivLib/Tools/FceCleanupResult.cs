using TheXDS.Vivianne.Models.Fce.Nfs3;

namespace TheXDS.Vivianne.Tools;

/// <summary>
/// Represents the result of the analysis performed on an <see cref="FceFile"/>
/// that includes a message and an action to be performed to resolve the issue.
/// </summary>
/// <param name="Title">Short description of the issue that was found.</param>
/// <param name="Details">
/// Long description of the the issue that was found.
/// </param>
/// <param name="CleanupAction">
/// Delegate that can be invoked to resolve the issue that was found. Ideally,
/// this delegate shall be scoped to the issue described, and not perform any
/// other unwanted cleanup actions.
/// </param>
public record class FceCleanupResult(string Title, string Details, Action<FceFile> CleanupAction);
