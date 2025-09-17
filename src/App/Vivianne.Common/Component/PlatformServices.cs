namespace TheXDS.Vivianne.Component;

/// <summary>
/// Static class that provides keyboard data in a platform agnostic way.
/// </summary>
public static class PlatformServices
{
    static IKeyboardProxy? _keyboardProxy;
    static IStaticFceRender? _staticFceRender;
    static IOperatingSystemProxy? _operatingSystemProxy;

    /// <summary>
    /// Gets a value that indicates the current state of keyboard modifier keys.
    /// </summary>
    public static ModifierKey ModifierKey
    {
        get
        {
            ModifierKey result = ModifierKey.None;

            result |= IsShiftKeyDown ? ModifierKey.Shift : ModifierKey.None;
            result |= IsAltKeyDown ? ModifierKey.Alt : ModifierKey.None;
            result |= IsCtrlKeyDown ? ModifierKey.Ctrl : ModifierKey.None;

            return result;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the current process is running with
    /// elevated privileges.
    /// </summary>
    public static bool IsElevated => _operatingSystemProxy?.IsElevated ?? false;

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
    /// Gets a value that indicates if the <c>Alt</c> key is being held down.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <c>Alt</c> key is being held down,
    /// <see langword="false"/> otherwise, or if the application did not set a
    /// keyboard proxy on initialization.
    /// </value>
    public static bool IsAltKeyDown => _keyboardProxy?.IsAltKeyDown ?? false;

    /// <summary>
    /// Gets a value that indicates if the <c>Ctrl</c> key is being held down.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <c>Ctrl</c> key is being held down,
    /// <see langword="false"/> otherwise, or if the application did not set a
    /// keyboard proxy on initialization.
    /// </value>
    public static bool IsCtrlKeyDown => _keyboardProxy?.IsCtrlKeyDown ?? false;

    /// <summary>
    /// Sets the keyboard proxy to use on the application.
    /// </summary>
    /// <param name="proxy">Keyboard proxy instance to use.</param>
    public static void SetKeyboardProxy(IKeyboardProxy proxy)
    {
        _keyboardProxy = proxy;
    }

    /// <summary>
    /// Sets the operating system proxy to use on the application.
    /// </summary>
    /// <param name="proxy">Operating system proxy instance to use.</param>
    public static void SetOperatingSystemProxy(IOperatingSystemProxy proxy)
    {
        _operatingSystemProxy = proxy;
    }

    /// <summary>
    /// Sets the FCE render service instance to use when a request to render a
    /// specific model is made.
    /// </summary>
    /// <param name="render">FCE render service to expose.</param>
    public static void SetFceRender(IStaticFceRender render)
    {
        _staticFceRender = render;
    }
}
