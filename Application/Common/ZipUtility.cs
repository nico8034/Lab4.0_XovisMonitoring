using System.IO.Compression;

namespace Application.Common;

public static class ZipUtility
{
    public static void ZipFolder(string sourceFolder, string zipFilePath)
    {
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        ZipFile.CreateFromDirectory(sourceFolder, zipFilePath);
    }
}