using TheXDS.Vivianne.Models.Viv;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Static class that provides keyboard data in a platform agnostic way.
/// </summary>
public static class PlatformServices
{
    static IKeyboardProxy? _keyboardProxy;
    static IStaticFceRender? _staticFceRender;

    /// <summary>
    /// Gets a value that indicates if the <c>Shift</c> key is being held down.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <c>Shift</c> key is being held down,
    /// <see langword="false"/> otherwise, or if the application did not set a
    /// keyboard proxy on initialization.
    /// </value>
    public static bool IsShiftKeyDown => _keyboardProxy?.IsShiftKeyDown ?? false;

    /// <summary>
    /// Sets the keyboard proxy to use on the application.
    /// </summary>
    /// <param name="proxy">Keyboard proxy instance to use.</param>
    public static void SetKeyboardProxy(IKeyboardProxy proxy)
    {
        _keyboardProxy = proxy;
    }

    public static void SetFceRender(IStaticFceRender render)
    {
        _staticFceRender = render;
    }
}
