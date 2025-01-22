using System.IO;

namespace TheXDS.Vivianne.Helpers;

public static class FileBackup
{
    public static void Create(string originalFile)
    {
        if (!File.Exists(originalFile)) return;
        var backupFileName = Path.Combine(Path.GetDirectoryName(originalFile)!, Path.GetFileNameWithoutExtension(originalFile));
        var c = 0;
        while (File.Exists($"{backupFileName}-{c:0000}.bak")) c++;
        File.Move(originalFile, $"{backupFileName}-{c:0000}.bak");
    }
}