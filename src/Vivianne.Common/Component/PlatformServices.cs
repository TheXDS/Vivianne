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

            result |= Keyboard.IsShiftKeyDown ? ModifierKey.Shift : ModifierKey.None;
            result |= Keyboard.IsAltKeyDown ? ModifierKey.Alt : ModifierKey.None;
            result |= Keyboard.IsCtrlKeyDown ? ModifierKey.Ctrl : ModifierKey.None;

            return result;
        }
    }

    /// <summary>
    /// Exposes an instance that provides FCE rendering services.
    /// </summary>
    public static IKeyboardProxy Keyboard => _keyboardProxy ??= new NullKeyboardProxy();

    /// <summary>
    /// Exposes an instance that provides FCE rendering services.
    /// </summary>
    public static IStaticFceRender FceRender => _staticFceRender ??= new NullFceRender();

    /// <summary>
    /// Exposes an instance that provides operating system information and
    ///  services.
    /// </summary>
    public static IOperatingSystemProxy OperatingSystem => _operatingSystemProxy ??= new NullOperatingSystemProxy();

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
