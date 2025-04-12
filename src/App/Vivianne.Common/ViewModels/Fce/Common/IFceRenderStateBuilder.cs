namespace TheXDS.Vivianne.ViewModels.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type used to create
/// <see cref="RenderState"/> objects from an FCE editor state context.
/// </summary>
/// <typeparam name="TState">
/// Type of state from which to extract the scene to be rendered.
/// </typeparam>
public interface IFceRenderStateBuilder<in TState>
{
    /// <summary>
    /// Builds a <see cref="RenderState"/> from the specified FCE editor state.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    RenderState Build(TState state);
}

