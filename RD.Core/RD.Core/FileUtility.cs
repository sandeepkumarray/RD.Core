using System;
using System.Collections.Generic;
using System.IO;

namespace RDCore
{
    public class FileUtility
    {
        public bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public IEnumerable<FileInfo> GetFileList(string searchPattern, string path)
        {
            if (!string.IsNullOrEmpty(searchPattern))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        return new DirectoryInfo(path).EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
                    }
                    catch
                    {
                    }
                }
            }
            return (IEnumerable<FileInfo>)new List<FileInfo>();
        }
    }
}
