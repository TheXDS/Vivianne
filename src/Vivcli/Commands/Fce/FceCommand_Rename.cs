using System.CommandLine;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Base;
using TheXDS.Vivianne.Models.Fce.Common;
using M3 = TheXDS.Vivianne.Models.Fce.Nfs3;
using M4 = TheXDS.Vivianne.Models.Fce.Nfs4;
using St = TheXDS.Vivianne.Resources.Strings.FceCommand;

namespace TheXDS.Vivianne.Commands.Fce;

public partial class FceCommand
{
    private static Command BuildRenameCommand(Argument<FileInfo> fileArg)
    {
        var cmd = new Command("rename", St.Rename_Help);
        var typeOption = new Option<SpecificFceObjectType>(["--type", "-t"], St.SpecificFceObjectType_Help);
        var indexOption = new Option<int>(["--index", "-i"], St.ObjIndex_Help);
        var nameOption = new Option<string>(["--name", "-n"], St.NewName_Help);
        indexOption.AddValidator(v => { if (v.GetValueOrDefault<int>() < 0) v.ErrorMessage = St.Index_error; });
        nameOption.AddValidator(v => { if (v.GetValueOrDefault<string?>().IsEmpty() && v.GetValueForOption(typeOption) == SpecificFceObjectType.Part) v.ErrorMessage = St.Name_empty; });
        cmd.AddOption(typeOption);
        cmd.AddOption(indexOption);
        cmd.AddOption(nameOption);
        cmd.AddAlias("ren");
        cmd.AddAlias("mv");
        cmd.SetHandler(RenameCommand, fileArg, typeOption, indexOption, nameOption);
        return cmd;
    }

    private static Task RenameCommand(FileInfo fceFile, SpecificFceObjectType type, int index, string name)
    {
        return FceTransaction(fceFile, p => DoRename<M3.FceFile, FcePart>(p, type, index, name), p => DoRename<M4.FceFile, M4.Fce4Part>(p, type, index, name));
    }

    private static void DoRename<TInFce, TPart>(TInFce inFce, SpecificFceObjectType type, int index, string name) where TPart : FcePart where TInFce : IFceFile<TPart>
    {
        if (type.HasFlag(SpecificFceObjectType.Part))
        {
            DoRename(inFce.Parts, index, name);
        }
        else if (type.HasFlag(SpecificFceObjectType.Dummy))
        {
            DoRename(inFce.Dummies, index, name);
        }
        else
        {
            throw new ArgumentException(St.DoRename_InvalidTypeSpecified, nameof(type));
        }
    }

    private static void DoRename(IEnumerable<INameable> collection, int index, string name)
    {
        IList<INameable> list = [.. collection];
        if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index), St.IndexIsOutOfRange);
        if (list.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) throw new ArgumentException(St.Name_duplicated, nameof(name));
        list[index].Name = name;
    }
}
