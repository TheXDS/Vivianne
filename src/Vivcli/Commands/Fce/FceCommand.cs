using TheXDS.Vivianne.Commands.Base;
using TheXDS.Vivianne.Info;
using M3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using M4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using S3 = TheXDS.Vivianne.Serializers.Fce.Nfs3;
using S4 = TheXDS.Vivianne.Serializers.Fce.Nfs4;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;

namespace TheXDS.Vivianne.Commands.Fce;

/// <summary>
/// Defines a command that allows the user to interact with an FCE file.
/// </summary>
public partial class FceCommand() : FileCommandBase(
    "fce",
    St.Help,
    St.Fce_FileArg,
    St.Fce_FileArgHelp,
    [
        BuildInfoCommand,
        BuildLsCommand,
        BuildConvertCommand,
        BuildRenameCommand,
    ])
{
    [Flags]
    private enum FceObjectType : byte
    {
        Part = 1,
        Dummy = 2,
        Any = 3,
    }

    private enum SpecificFceObjectType
    {
        Part,
        Dummy,
    }

    private static Task FceTransaction(FileInfo fceFile, Action<M3.FceFile> fce3Callback, Action<M4.FceFile> fce4Callback)
    {
        try
        {
            var rawContents = File.ReadAllBytes(fceFile.FullName);
            return VersionIdentifier.FceVersion(rawContents) switch
            {
                NfsVersion.Nfs3 => FileTransaction<M3.FceFile, S3.FceSerializer>(fceFile, fce3Callback),
                NfsVersion.Nfs4 or NfsVersion.Mco => FileTransaction<M4.FceFile, S4.FceSerializer>(fceFile, fce4Callback),
                _ => throw new Exception(St.UnknownFCEVersion)
            };
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
            return Task.CompletedTask;
        }
    }

    private static Task FceTransaction(FileInfo fceFile, Func<M3.FceFile, Task> fce3Callback, Func<M4.FceFile, Task> fce4Callback)
    {
        try
        {
            var rawContents = File.ReadAllBytes(fceFile.FullName);
            return VersionIdentifier.FceVersion(rawContents) switch
            {
                NfsVersion.Nfs3 => FileTransaction<M3.FceFile, S3.FceSerializer>(fceFile, fce3Callback),
                NfsVersion.Nfs4 or NfsVersion.Mco => FileTransaction<M4.FceFile, S4.FceSerializer>(fceFile, fce4Callback),
                _ => throw new Exception(St.UnknownFCEVersion)
            };
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
            return Task.CompletedTask;
        }
    }

    private static Task ReadOnlyFceTransaction(FileInfo fceFile, Action<M3.FceFile> fce3Callback, Action<M4.FceFile> fce4Callback)
    {
        try
        {
            var rawContents = File.ReadAllBytes(fceFile.FullName);
            return VersionIdentifier.FceVersion(rawContents) switch
            {
                NfsVersion.Nfs3 => ReadOnlyFileTransaction<M3.FceFile, S3.FceSerializer>(fceFile, fce3Callback),
                NfsVersion.Nfs4 or NfsVersion.Mco => ReadOnlyFileTransaction<M4.FceFile, S4.FceSerializer>(fceFile, fce4Callback),
                _ => throw new Exception(St.UnknownFCEVersion)
            };
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
            return Task.CompletedTask;
        }
    }

    private static Task ReadOnlyFceTransaction(FileInfo fceFile, Func<M3.FceFile, Task> fce3Callback, Func<M4.FceFile, Task> fce4Callback)
    {
        try
        {
            var rawContents = File.ReadAllBytes(fceFile.FullName);
            return VersionIdentifier.FceVersion(rawContents) switch
            {
                NfsVersion.Nfs3 => ReadOnlyFileTransaction<M3.FceFile, S3.FceSerializer>(fceFile, fce3Callback),
                NfsVersion.Nfs4 or NfsVersion.Mco => ReadOnlyFileTransaction<M4.FceFile, S4.FceSerializer>(fceFile, fce4Callback),
                _ => throw new Exception(St.UnknownFCEVersion)
            };
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
            return Task.CompletedTask;
        }
    }
}