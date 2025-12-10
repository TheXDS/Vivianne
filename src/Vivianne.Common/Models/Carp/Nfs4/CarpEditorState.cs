using TheXDS.Vivianne.Models.Carp.Base;

namespace TheXDS.Vivianne.Models.Carp.Nfs4;

/// <summary>
/// Implements a <see cref="CarpEditorState{TCarPerf, TCarClass}"/> for NFS4
/// Carp files.
/// </summary>
public class CarpEditorState : CarpEditorState<CarPerf, Fe.Nfs4.CarClass>
{
    /// <inheritdoc/>
    public double UndersteerGradient
    {
        get => File.UndersteerGradient;
        set => Change(p => p.UndersteerGradient, value);
    }
}