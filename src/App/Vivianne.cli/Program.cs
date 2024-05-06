using System.CommandLine;
using TheXDS.Vivianne.Commands;

namespace TheXDS.Vivianne;


internal static class Program
{
    private static Task<int> Main(string[] args)
    {
        var rootCmd = new RootCommand("Vivianne CLI - A NFS3 modding tool.");
        foreach (var j in VivianneCommand.GetCommands())
        {
            rootCmd.AddCommand(j);
        }
        return rootCmd.InvokeAsync(args);
    }
}