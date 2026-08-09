namespace TheXDS.Vivianne.Component.Application;

/// <summary>
/// Represents the platform configuration containing proxies for platform-specific services.
/// </summary>
/// <param name="KeyboardProxy">
/// The keyboard proxy used for input handling.
/// </param>
/// <param name="OperatingSystemProxy">
/// The operating system proxy used for OS-level operations.
/// </param>
public readonly record struct PlatformConfiguration(
    IKeyboardProxy KeyboardProxy,
    IOperatingSystemProxy OperatingSystemProxy
);
