using System;
using System.Collections.Generic;
using St = TheXDS.Vivianne.Resources.Strings.Component.Application.CommandLineStartup;

namespace TheXDS.Vivianne.Component.Application;

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
            PlatformServices.OperatingSystem.ShowNativeErrorBox(St.OperationRequiresElevation, St.OperationNotPermitted);
            unchecked { Environment.Exit((int)0x80070005); }
        }
    }
}
