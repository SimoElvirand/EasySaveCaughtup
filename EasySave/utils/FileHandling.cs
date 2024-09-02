using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasySave.utils
{
    public static class FileHandling
    {
        public static void CreateDirIfNotExist(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }
    
    }
}
