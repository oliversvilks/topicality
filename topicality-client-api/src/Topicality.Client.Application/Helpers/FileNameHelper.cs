using System.Text.RegularExpressions;

namespace Topicality.Client.Application.Helpers;

public static class FileNameHelper
{
    public static string NormalizeFileName(string fileName)
    {
        // Replace spaces with underscores
        string normalizedFileName = fileName.Replace(" ", "_");

        // Remove or replace other special characters
        normalizedFileName = Regex.Replace(normalizedFileName, @"[^a-zA-Z0-9_.-]", string.Empty);

        return normalizedFileName;
    }
}