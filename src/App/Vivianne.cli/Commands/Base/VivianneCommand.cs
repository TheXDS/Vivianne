using System.CommandLine;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Helpers.ReflectionHelpers;

namespace TheXDS.Vivianne.Commands.Base;

/// <summary>
/// Base class for all Vivianne CLI commands. It includes some shared
/// properties and parameters.
/// </summary>
public abstract class VivianneCommand
{
    /// <summary>
    /// Writes an error message to the standard error stream and exits the
    /// program with an exit code.
    /// </summary>
    /// <param name="message">
    /// Message to write to <c>stderr</c>.
    /// </param>
    /// <param name="exitCode">
    /// Exit code to return to the operating system. If ommitted, will default
    /// to <c>-1</c>.
    /// </param>
    protected static void Fail(string message, int exitCode = -1){
        using (var stderr = Console.OpenStandardError())
        using (var stderrw = new StreamWriter(stderr))
        {
            stderrw.WriteLine(message);
        }
        Environment.Exit(exitCode);
    }

    /// <summary>
    /// Gets a collection of common arguments to be used on any Commands that
    /// inherit from this class.
    /// </summary>
    protected static Dictionary<string, Argument> CommonArguments { get; } = [];

    /// <summary>
    /// Gets a collectino of common Options to be used on any Commands that
    /// inherit from this class.
    /// </summary>
    protected static Dictionary<string, Option> CommonOptions { get; } = [];

    /// <summary>
    /// Gets a common argument.
    /// </summary>
    /// <typeparam name="T">Type of argument to be returned.</typeparam>
    /// <param name="name">Name of the argument to return.</param>
    /// <returns>An option from <see cref="CommonArguments"/>.</returns>
    protected static Argument<T> GetArgument<T>(string name)
    {
        return (Argument<T>)CommonArguments[name];
    }

    /// <summary>
    /// Gets a common option.
    /// </summary>
    /// <typeparam name="T">Type of option to be returned.</typeparam>
    /// <param name="name">Name of the option to return.</param>
    /// <returns>An option from <see cref="CommonOptions"/>.</returns>
    protected static Option<T> GetOption<T>(string name)
    {
        return (Option<T>)CommonOptions[name];
    }

    /// <summary>
    /// Scans the current <see cref="AppDomain"/> and enumerates all Commands
    /// defined for Vivianne CLI.
    /// </summary>
    /// <returns>
    /// An enumeration of all defined Commands that inherit from
    /// <see cref="VivianneCommand"/>.
    /// </returns>
    public static IEnumerable<Command> GetCommands()
    {
        return GetTypes<VivianneCommand>(true)
            .Select(TypeExtensions.New<VivianneCommand>)
            .Select(p => p.GetCommand());
    }

    /// <summary>
    /// When overriden in a derivate class, defines the logic required to build
    /// the command to be exposed on Vivianne CLI.
    /// </summary>
    /// <returns>A command that can be invoked in Vivianne CLI.</returns>
    public abstract Command GetCommand();
}
