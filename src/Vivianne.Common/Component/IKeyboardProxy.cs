namespace TheXDS.Vivianne.Component;

/// <summary>
/// Defines a set of members to be implemented by a type that exposes keyboard
/// data for modifier keys that can be used to open editors and dialogs in
/// alternate modes or with extra options.
/// </summary>
public interface IKeyboardProxy
{
    /// <summary>
    /// Gets a value that indicates if the <c>Shift</c> key is being held down.
    /// </summary>
    bool IsShiftKeyDown { get; }

    /// <summary>
    /// Gets a value that indicates if the <c>Alt</c> key is being held down.
    /// </summary>
    bool IsAltKeyDown { get; }

    /// <summary>
    /// Gets a value that indicates if the <c>Ctrl</c> key is being held down.
    /// </summary>
    bool IsCtrlKeyDown { get; }
}
