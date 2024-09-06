using EasySave.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EasySave
{
    internal class BackupJobModel
    {
        private string _sourceDirectory;
        private string _destinationDirectory;
        private string _backupType;
        private string _name;

        public string sourceDirectory { get => _sourceDirectory; set => _sourceDirectory = value; }
        public string destinationDirectory { get => _destinationDirectory; set => _destinationDirectory = value; }
        public string name { get => _name; set => _name = value; }
        public string backupType { get => _backupType; set => _backupType = value; }
        
        public void runBackupJob(int log_choice)
        {   
            if (this.backupType == "Full")
            {
                sourceDifferentialCopy(this,log_choice);  
            }
            else if (this.backupType == "Differential")
            {
                sourceDifferentialCopy(this,log_choice);
            }
        }

        class FileCompare : System.Collections.Generic.IEqualityComparer<FileInfo>
        {
            public FileCompare() { }

            public bool Equals(FileInfo f1, FileInfo f2)
            {
                return (f1.Name == f2.Name &&
                        f1.Length == f2.Length);
            }

            // Return a hash that reflects the comparison criteria. According to the
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(FileInfo fi)
            {
                string s = $"{fi.Name}{fi.Length}";
                return s.GetHashCode();
            }
        }

         bool IsSoftwareExecutable(string filePath)
        {

            string[] softwareExtensions = { ".exe", ".dll", ".zip" };

            foreach (string extension in softwareExtensions)
            {
                if (filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        private void sourceDifferentialCopy(BackupJobModel backupJob, int log_choice)
        {
            DirectoryInfo diSource = new DirectoryInfo(backupJob.sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(backupJob.destinationDirectory);
            if (diSource != diTarget)
            {
                directoryDifferentialCopy2(diSource, diTarget,log_choice,backupJob);
            }

        }


        void directoryDifferentialCopy2(DirectoryInfo diSource, DirectoryInfo diTarget,int log_choice, BackupJobModel backupJob)
        {

            if (diTarget.Exists)
            {
                // Take a snapshot of the file system.  
                IEnumerable<FileInfo> list1 = diSource.GetFiles("*.*", SearchOption.AllDirectories);
                IEnumerable<FileInfo> list2 = diTarget.GetFiles("*.*", SearchOption.AllDirectories);

                // A custom file comparer defined below
                FileCompare myFileCompare = new FileCompare();

                // Find the set difference between the two folders.  
                // For this example we only check one way.  
                var queryList1Only = (from file in list1
                                      where !IsSoftwareExecutable(file.FullName)
                                      select file).Except(list2, myFileCompare);

                foreach (var v in queryList1Only)
                {
                    // Copy the file

                    Stopwatch stopwatch = new Stopwatch();
                    Console.Write("diff copy done for file : " + v.Name);

                    stopwatch.Start();
                    v.CopyTo(Path.Combine(diTarget.ToString(), v.Name), true);
                    stopwatch.Stop();
                    long fileSize = v.Length;
                    if (log_choice == 1)
                    {
                        DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    }
                    else if (log_choice == 2)
                    {
                        DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    }
                    
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        diTarget.CreateSubdirectory(diSourceSubDir.Name);
                    directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob);
                }
                return;
            }



            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(diTarget.FullName) == false)
            {
                Directory.CreateDirectory(diTarget.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in diSource.GetFiles())
            {
                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine(@"Copying {0}\{1}", diTarget.FullName, fi.Name);
                Console.Write("diff copy done for file : " + fi.Name);
                stopwatch.Start();
                fi.CopyTo(Path.Combine(diTarget.ToString(), fi.Name), true); 
                stopwatch.Stop();
                long fileSize = fi.Length;
                if (log_choice == 1)
                {
                    DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                }
                else if (log_choice == 2)
                {
                    DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                }
                
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    diTarget.CreateSubdirectory(diSourceSubDir.Name);
                directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob);
            }
        }


        //get all files and subdirectories in a directory
        private string[] getAllFilesAndSubDirectories (string path)
        {
            string[] entries = Directory.GetFileSystemEntries(path);
            return entries;
        }
        // Function to create a complete copy of the source folder to the destination folder
        void sourceFullCopy(BackupJobModel backupJob, int log_choice)
        {
            // Create the destination folder if it does not exist
            FileHandling.CreateDirIfNotExist(backupJob.destinationDirectory);
            // Retrieve the list of files and subdirectories in the source folder
            string[] entries = getAllFilesAndSubDirectories(backupJob.sourceDirectory);
            // Loop through each file or subdirectory in the source folder
            foreach (string entry in entries)
            {                
                if (Directory.Exists(entry)) // It's a subdirectory
                {
                    //recursively call sourceFullCopy for the subdirectory
                    string subDirectoryName = Path.GetFileName(entry);
                    string subDestinationDirectory = Path.Combine(backupJob.destinationDirectory, subDirectoryName);

                    // Create a BackupJobModel for the subdirectory
                    BackupJobModel subBackupJob = new BackupJobModel(entry, subDestinationDirectory, backupJob.name, backupType);
                    {
                        sourceDirectory = entry;
                        destinationDirectory = subDestinationDirectory;
                    };
                    // Recursively call sourceFullCopy for the subdirectory
                    sourceFullCopy(subBackupJob, log_choice);
                }
                else //it's a file
                {
                    string fileName = Path.GetFileName(entry);
                    string sourceFile = Path.GetFullPath(entry);
                    string destFile = Path.Combine(backupJob.destinationDirectory, fileName);
                    if (!IsSoftwareExecutable(sourceFile))
                    {
                        // Copy the file
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        File.Copy(entry, destFile, true);
                        stopwatch.Stop();
                        long fileSize = entry.Length;
                        if (log_choice == 1)
                        {
                            DailyLogModel.XMLLogger(this.name, this.sourceDirectory + '\\' + fileName, this.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        } else if (log_choice == 2)
                        {
                            DailyLogModel.JsonLogger(this.name, this.sourceDirectory + '\\' + fileName, this.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        }
                    }

                }

            }

        }
        
        public BackupJobModel(string sourceDirectory, string destinationDirectory, string name, string backupType)
        {
            this.sourceDirectory = sourceDirectory;
            this.destinationDirectory = destinationDirectory;
            this.name = name;
            this.backupType = backupType;
            /*this._locationType = locationType;*/
        }

    }
    

}
