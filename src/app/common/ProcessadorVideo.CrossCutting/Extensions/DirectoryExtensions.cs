using System;

namespace ProcessadorVideo.CrossCutting.Extensions;

public static class DirectoryExtensions
{
    public static void RemoveDirectory(string path, bool recursive = true)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive);
    }
}

