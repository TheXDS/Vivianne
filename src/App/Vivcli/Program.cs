﻿using System.CommandLine;
using TheXDS.Vivianne.Commands.Base;

namespace TheXDS.Vivianne;

internal static class Program
{
    private static Task<int> Main(string[] args)
    {
        var rootCmd = new RootCommand(Resources.Strings.VivCli.RootHelp);
        foreach (var j in VivianneCommand.GetCommands())
        {
            rootCmd.AddCommand(j);
        }
        return rootCmd.InvokeAsync(args);
    }
}