using System.IO;

namespace TheXDS.Vivianne.Helpers;

/// <summary>
/// Contains methods used to create and manage file backups for any file that
/// can be edited by Vivianne.
/// </summary>
public static class FileBackup
{
    /// <summary>
    /// Creates a backup of the specified file.
    /// </summary>
    /// <param name="originalFile">Full path of the original file.</param>
    public static void Create(string originalFile)
    {
        if (!File.Exists(originalFile)) return;
        var backupFileName = Path.Combine(Path.GetDirectoryName(originalFile)!, Path.GetFileNameWithoutExtension(originalFile));
        var c = 0;
        while (File.Exists($"{backupFileName}-{c:0000}.bak")) c++;
        File.Move(originalFile, $"{backupFileName}-{c:0000}.bak");
    }
}