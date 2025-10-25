using System;
using System.Collections.Generic;

namespace TheXDS.Vivianne.Component;

/// <summary>
/// Static class that provides command line startup services and helper methods.
/// </summary>
public static class CommandLineStartup
{
    /// <summary>
    /// Gets a dictionary of command line handlers that can be used to
    /// process custom command line arguments.
    /// </summary>
    public static Dictionary<Guid, Action<string[]>> Handlers { get; } = [];

    /// <summary>
    /// Forces the application to exit if it is not running with elevated
    /// privileges.
    /// </summary>
    public static void FailIfNotElevated()
    {
        if (!PlatformServices.OperatingSystem.IsElevated)
        {
            PlatformServices.OperatingSystem.ShowNativeErrorBox("This operation requires elevated privileges.", "Operation not permitted");
            unchecked { Environment.Exit((int)0x80070005); }
        }
    }
}