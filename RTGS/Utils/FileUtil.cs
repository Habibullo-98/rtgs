using Serilog;

namespace RTGS.Utils;

public class FileUtil
{
    public static async Task Save(string content, string path)
    {
        Log.Debug("[FILE UTIL] Saving file {Path}", path);
        await File.WriteAllTextAsync(path, content);
    }
    
    public static void Move(string source, string folder)
    {
        var fileName = Path.GetFileName(source);
        File.Move(source, Path.Combine(folder, fileName));
        Log.Information("[FILE UTIL] Moved file from {Source} to {Destination}", source, folder);
    }

    
    public static bool IsFileLocked(string filePath)
    {
        try
        {
            using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return false;
            }
        }
        catch (IOException)
        {
            return true;
        }
    }
}