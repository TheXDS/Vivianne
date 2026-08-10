using System;
using TheXDS.Vivianne.Properties;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using static TheXDS.MCART.Resources.Strings.ExDumpOptions;
using static TheXDS.MCART.Resources.Strings.Composition;

namespace TheXDS.Vivianne.Component.Application;

/// <summary>
/// Provides initialization and startup routines for the Vivianne application.
/// </summary>
public static class Initialization
{
    /// <summary>
    /// Initializes application services using the specified platform configuration.
    /// </summary>
    /// <param name="configuration">The platform configuration containing proxy instances and other settings.</param>
    public static void InitServices(PlatformConfiguration configuration)
    {
        PlatformServices.SetKeyboardProxy(configuration.KeyboardProxy);
        PlatformServices.SetOperatingSystemProxy(configuration.OperatingSystemProxy);
        EnableUnhandledExceptionDialog();
    }

    /// <summary>
    /// Executes initial asynchronous operations required before the application becomes operational.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task RunInitialOperations()
    {
        await Settings.LoadAsync();
        ProcessSpecialCommandLineStartupCallbacks(Environment.GetCommandLineArgs());
    }

    /// <summary>
    /// Registers an unhandled exception handler that logs errors to the desktop and displays an error dialog.
    /// </summary>
    /// <remarks>
    /// This method is only active in RELEASE builds. It subscribes to <see cref="AppDomain.UnhandledException"/>
    /// to capture and report any unhandled exceptions.
    /// </remarks>
    [Conditional("RELEASE")]
    private static void EnableUnhandledExceptionDialog()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    /// <summary>
    /// Processes command-line arguments to detect and execute special startup callbacks.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <remarks>
    /// If the first argument starts with <c>--Callback-</c> followed by a GUID, the corresponding handler
    /// from <see cref="CommandLineStartup.Handlers"/> is invoked in windowless mode.
    /// </remarks>
    private static void ProcessSpecialCommandLineStartupCallbacks(string[] args)
    {
        if (args.Length >= 2 && args[1].StartsWith("--Callback-") && CommandLineStartup.Handlers.TryGetValue(Guid.Parse(args[0][11..]), out var handler))
        {
            try
            {
                PlatformServices.OperatingSystem.GoWindowless();
                handler.Invoke(args);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Environment.Exit(ex.HResult != 0 ? ex.HResult : -1);
            }
        }
    }

    /// <summary>
    /// Handles unhandled exceptions by writing a dump file to the desktop and displaying an error dialog.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The exception event data containing the unhandled exception.</param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        try
        {
            File.WriteAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"VivianneError_{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}.txt"),
                ExDump((Exception)e.ExceptionObject, All));

            PlatformServices.OperatingSystem.ShowNativeErrorBox("Unhandled exception", $"""
            An unhandled exception has occurred in Vivianne, and it cannot continue execution.

            {((Exception)e.ExceptionObject).Message}

            An exception dump has been generated onto your desktop.

            Please, submit a bug report in Vivianne's repo at https://github.com/TheXDS/Vivianne
            """);
        }
        finally
        {
            Environment.FailFast("Unhandled exception", (Exception)e.ExceptionObject);
        }
    }
}
