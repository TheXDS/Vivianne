namespace TheXDS.Vivianne.Component;

/// <summary>
/// Static class that provides keyboard data in a platform agnostic way.
/// </summary>
public static class KeyboardProxy
{
    static IKeyboardProxy? _proxy;

    /// <summary>
    /// Gets a value that indicates if the <c>Shift</c> key is being held down.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <c>Shift</c> key is being held down,
    /// <see langword="false"/> otherwise, or if the application did not set a
    /// keyboard proxy on initialization.
    /// </value>
    public static bool IsShiftKeyDown => _proxy?.IsShiftKeyDown ?? false;

    /// <summary>
    /// Sets the keyboard proxy to use on the application.
    /// </summary>
    /// <param name="proxy">Keyboard proxy instance to use.</param>
    public static void SetProxy(IKeyboardProxy proxy)
    {
        _proxy = proxy;
    }
}
