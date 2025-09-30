namespace TheXDS.Vivianne.Component;

/// <summary>
/// Implements a dummy, null keyboard proxy for when the proxy is not
///  registered on the target platform.
/// </summary>
public class NullKeyboardProxy : IKeyboardProxy
{
    /// <inheritdoc/>
    public bool IsAltKeyDown => false;

    /// <inheritdoc/>
    public bool IsCtrlKeyDown => false;

    /// <inheritdoc/>
    public bool IsShiftKeyDown => false;
}