using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

using System.ComponentModel;
using System.Windows.Input;
using WPF.commands;

namespace WPF.model
{
    class BackupListManager
    {
        private static string _path = "C://Log";
        private static string _extensionsToEncrypt = "";
        private static Mutex mutuale = new Mutex(false);
        private static Semaphore _semaphore = new Semaphore(1, 10);
        private static long  size = 1024;
        private static string name = "";
        private static ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private static List<BackupJobModel> backuplistvieww = new List<BackupJobModel>();
        //private BackupJobModel backupJob;
        //private int jobCount;
        private bool stopRequested;
        private Thread thread;
        //static Thread[] threads = new Thread[5];
        //static Dictionary<int, ManualResetEventSlim> threadControlEvents = new Dictionary<int, ManualResetEventSlim>();
        public ICommand PauseBackupCommand { get; set; }
        public ICommand StopBackupCommand { get; set; }


        public static List<BackupJobModel> backuplistview
        {
            get { return backuplistvieww; }
            set 
            {
                backuplistvieww = value;
            }
        }
        public static string names
        {
            get { return name; }
            set
            {
               name = value;

                // Notify property changed if you implement INotifyPropertyChanged
            }
        }
        public static long sizes
        {
            get { return size; }
            set
            {
                if (value >= 0)
                {
                    size = value;
                }
                else
                {
                    throw new ArgumentException("La size doit être positive.");
                }

                // Notify property changed if you implement INotifyPropertyChanged
            }
        }
        public static string path
        {
            get { return _path; }
            set
            {
                _path = value;

                // Notify property changed if you implement INotifyPropertyChanged
            }
        }

        public static string ExtensionsToEncrypt
        {
            get { return _extensionsToEncrypt; }
            set
            {
                _extensionsToEncrypt = value;
                // Notify property changed if you implement INotifyPropertyChanged
            }
        }
        
        public static ObservableCollection<BackupJobModel> DatabaseBackupJobs = new ObservableCollection<BackupJobModel>();

        public static ObservableCollection<BackupJobModel> GetBackupJobModels()
        {
            return DatabaseBackupJobs;

        }
        //private static  int threadIndexCounter = 0;
        // public static void RunBackupp()
        //{

        //foreach (var selectedBackupJob in backuplistview)
        //{
        //int currentIndex = threadIndexCounter++;
        //Debug.WriteLine("in ");
        //Debug.WriteLine(backuplistview.Count);
        //var resetEvent = new ManualResetEventSlim(true); // Commence en état non bloqué
        //int threadIndex = currentIndex;
        //threadControlEvents[threadIndex] = resetEvent;
        //threads[threadIndex] = new Thread(() =>
        ///{
        //RunBackupJob(selectedBackupJob, backuplistview.Count);
        //Debug.WriteLine("Trying to run and add a thread controller " + currentIndex);
        //});
        //threads[threadIndex].Start();
        //}
        //}

        //public BackupListManager(BackupJobModel backupJob, int jobCount)
        //{
        //  this.backupJob = backupJob;
        /// this.jobCount = jobCount;
        // pauseEvent = new ManualResetEvent(true);
        // PauseBackupCommand = new RelayCommand(PauseBackup, CanPauseBackup);
        // StopBackupCommand = new RelayCommand(Stop, CanStopBackup);
        // thread = new Thread(DoWork);
        // stopRequested = false;
        // }

        private BackupJobModel _backupJobModel;
        private BackupJobModel _backupJob;
        private int _totalBackupJobs;

        public BackupListManager(BackupJobModel backupJob, int totalBackupJobs, BackupJobModel backupJobModel)
        {
            _backupJob = backupJob;
            _totalBackupJobs = totalBackupJobs;
            _backupJobModel = backupJobModel;
        }
        private static string RunXorExecutable(string inputPath, string outputPath)
        {
            string xorExePath = @"C:\Users\simog\source\repos\EasySave\WPF\utils\CryptoSoft\Xor.exe";

            // Set up process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = xorExePath,
                Arguments = $"--input \"{inputPath}\" --output \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
        
        private static bool checkIfFileEndsWithExtension(string file,string extensions)
        {
            if (extensions == null)
            {
                return false;
            } else if (extensions == "")
            {
                return false;
            }
            string[] extensionsList = extensions.Split(',');;
            if (extensionsList.Count() ==0)
            {
                return false;
            }
            foreach (string extension in extensionsList)
            {
                if (file.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
            
        }

        public static void AddBackupJob(BackupJobModel backupJobModel)
        {
            DatabaseBackupJobs.Add(backupJobModel);
        }
        public static void DeleteBackupJob(BackupJobModel backupJobModel)
        {
            var backupJobsCopy = new List<BackupJobModel>(DatabaseBackupJobs);

            DatabaseBackupJobs.Remove(backupJobModel);
        }
        public static void RunBackupJob(BackupJobModel backupJobModel, int c)
        {
            

            backupJobModel.lastTimeRuned = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            Debug.WriteLine("trying to run backup");

            //_semaphore.WaitOne();
            //barrier.SignalAndWait();

            if (backupJobModel.backupType == "Full")
            {
                
                
                Debug.WriteLine("trying to run full backup");
                sourceDifferentialCopy(backupJobModel, backupJobModel.logChoice);
            }
            else if (backupJobModel.backupType == "Diff")
            {
                
                Debug.WriteLine("trying to run diff backup");
                sourceDifferentialCopy(backupJobModel, backupJobModel.logChoice);
            }
           // _semaphore.Release();
        }
        private static void sourceDifferentialCopy(BackupJobModel backupJob, int log_choice)
        {
            DirectoryInfo diSource = new DirectoryInfo(backupJob.sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(backupJob.destinationDirectory);
            if (diSource != diTarget)
            {
                directoryDifferentialCopy2(diSource, diTarget, log_choice, backupJob);
            }

        }
        public void DoWork()
        {
            //while (!stopRequested)
            //{
                pauseEvent.WaitOne();  // Attendre que le thread soit repris

                // Exécution de la sauvegarde
                Debug.WriteLine($"Travail de sauvegarde pour {_backupJobModel.name} en cours...");
                Thread.Sleep(10);
                RunBackupJob(_backupJobModel, _totalBackupJobs);

                Thread.Sleep(10);  // Simule un travail qui prend du temps
            //}

            Debug.WriteLine($"Thread pour {_backupJobModel.name} terminé.");
        }
        private static void directoryDifferentialCopy2(DirectoryInfo diSource, DirectoryInfo diTarget, int log_choice, BackupJobModel backupJob)
        {
            if (diTarget.Exists)
            {
                IEnumerable<FileInfo> list1 = diSource.GetFiles("*.*", SearchOption.AllDirectories);
                IEnumerable<FileInfo> list2 = diTarget.GetFiles("*.*", SearchOption.AllDirectories);

                FileCompare myFileCompare = new FileCompare();

                var queryList1Only = (from file in list1
                                      where !IsSoftwareExecutable(file.FullName)
                                      select file).Except(list2, myFileCompare);

                int totalFiles = queryList1Only.Count();
                int processedFiles = 0;

                foreach (var v in queryList1Only)
                {
                    pauseEvent.WaitOne();  // Attendre ici si la pause est activée.

                    Stopwatch stopwatch = new Stopwatch();
                    Console.Write("diff copy done for file : " + v.Name);
                    Thread.Sleep(1000);
                    stopwatch.Start();
                    bool isFileEndsWithExtension = checkIfFileEndsWithExtension(v.Name, ExtensionsToEncrypt);
                    if (isFileEndsWithExtension)
                    {
                        Thread.Sleep(1000);
                        string outputPath = Path.Combine(diTarget.FullName, v.Name);
                        RunXorExecutable(v.FullName, outputPath + "_encrypted");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        
                        foreach (var i in queryList1Only)
                        {
                            if (CalculSizeFile(size, i.Length) & i.Name == name) 
                            {
                                Thread.Sleep(1000);
                                i.CopyTo(Path.Combine(diTarget.ToString(), i.Name), true);
                                Thread.Sleep(1000);
                            }
                        }
                            //mutuale.WaitOne();
                        if (CalculSizeFile(size, v.Length) & v.Name != name)
                        {
                            Thread.Sleep(1000);
                            v.CopyTo(Path.Combine(diTarget.ToString(), v.Name), true);
                            Thread.Sleep(1000);
                           // mutuale.ReleaseMutex();
                        }
                        
                    }
                    stopwatch.Stop();
                    long fileSize = v.Length;
                    if (log_choice == 1)
                    {
                        mutuale.WaitOne();
                        DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        mutuale.ReleaseMutex();
                    }
                    else if (log_choice == 2)
                    {
                        mutuale.WaitOne();
                        DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        mutuale.ReleaseMutex();
                    }
                    else if (log_choice == 3)
                    {
                        mutuale.WaitOne();
                        DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        mutuale.ReleaseMutex();
                        mutuale.WaitOne();
                        DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + v.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                        mutuale.ReleaseMutex();
                    }

                    processedFiles++;
                    backupJob.Progress1 = (double)processedFiles / totalFiles * 100;
                    Debug.WriteLine("in process" + processedFiles++);
                    Debug.WriteLine("in process" + backupJob.Progress1);
                }

               // bool success = processedFiles == totalFiles;
               // NotifyBackupCompletion(success);

                foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        diTarget.CreateSubdirectory(diSourceSubDir.Name);
                    directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob);
                }
                return;
            }

            if (Directory.Exists(diTarget.FullName) == false)
            {
                Directory.CreateDirectory(diTarget.FullName);
            }

            foreach (FileInfo fi in diSource.GetFiles())
            {
                pauseEvent.WaitOne();  // Attendre ici si la pause est activée.

                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine(@"Copying {0}\{1}", diTarget.FullName, fi.Name);
                Console.Write("diff copy done for file : " + fi.Name);
                stopwatch.Start();
                bool isFileEndsWithExtension = checkIfFileEndsWithExtension(fi.Name, ExtensionsToEncrypt);
                if (isFileEndsWithExtension)
                {
                    string inputPath = Path.Combine(diSource.FullName, fi.Name);
                    string outputPath = Path.Combine(diTarget.FullName, fi.Name);
                    RunXorExecutable(inputPath, outputPath + "_encrypted");
                }
                else
                {
                    fi.CopyTo(Path.Combine(diTarget.ToString(), fi.Name), true);
                }
                stopwatch.Stop();
                long fileSize = fi.Length;
                if (log_choice == 1)
                {
                    mutuale.WaitOne();
                    DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    mutuale.ReleaseMutex();
                }
                else if (log_choice == 2)
                {
                    mutuale.WaitOne();
                    DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    mutuale.ReleaseMutex();
                }
                else if (log_choice == 3)
                {
                    mutuale.WaitOne();
                    DailyLogModel.JsonLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    mutuale.ReleaseMutex();
                    mutuale.WaitOne();
                    DailyLogModel.XMLLogger(backupJob.name, backupJob.sourceDirectory + '\\' + fi.Name, backupJob.destinationDirectory, DateTime.Now, stopwatch.Elapsed.TotalSeconds, fileSize);
                    mutuale.ReleaseMutex();
                }
            }

            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    diTarget.CreateSubdirectory(diSourceSubDir.Name);
                directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob);
            }
        }


        public void Stop(object obj)
        {
            stopRequested = true;  // Arrêter le thread
            pauseEvent.Set();  // S'assurer que le thread n'est pas en pause lorsqu'il est arrêté
          //  Debug.WriteLine($"Thread pour {backupJob.name} arrêté.");
        }

        public void Start()
        {
            thread.Start();
        }
        public static void PauseBackup(object obj)
        {
            pauseEvent.Reset();
            //Thread.Sleep(70000000);
            Debug.WriteLine($"Thread pour backupManager pause Thread ID:" + Thread.CurrentThread.ManagedThreadId);
        }

        public static void ResumeBackup()
        {
           pauseEvent.Set();  // Reprendre
            Debug.WriteLine($"Thread pour backupManager repris Thread ID:" + Thread.CurrentThread.ManagedThreadId);
        }
        private bool CanPauseBackup(object obj)
        {
            return true; // Change here
        }

        private bool CanStopBackup(object obj)
        {
            return true; // Change here
        }
        class FileCompare : System.Collections.Generic.IEqualityComparer<FileInfo>
        {
            public FileCompare() { }

            public bool Equals(FileInfo f1, FileInfo f2)
            {
                return (f1.Name == f2.Name &&
                        f1.Length == f2.Length);
            }

            public int GetHashCode(FileInfo fi)
            {
                string s = $"{fi.Name}{fi.Length}";
                return s.GetHashCode();
            }
        }
        static bool IsSoftwareExecutable(string filePath)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(filePath);

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    return true; // Include hidden files
                }

                string[] softwareExtensions = { ".exe", ".dll", ".zip", ".RAR", ".rar", ".ZIP", "tgz" };

                foreach (string extension in softwareExtensions)
                {
                    if (filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                // Handle exceptions if necessary
                return false;
            }
        }

        public static void CopyFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                // Vérifie si le fichier source existe
                if (File.Exists(sourceFilePath))
                {
                    // Copie le fichier à l'emplacement de destination
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    Debug.WriteLine($"Le fichier a été copié de {sourceFilePath} vers {destinationFilePath}.");
                }
                else
                {
                    Debug.WriteLine($"Le fichier source n'existe pas : {sourceFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Une erreur est survenue lors de la copie du fichier : {ex.Message}");
            }
        }
        static bool CalculSizeFile(long size, long lengthbite)
        {
          
               
                    long fileSizeInBytes = lengthbite;
                    double fileSizeInKB = fileSizeInBytes / 1024.0;
                    if(fileSizeInKB > size)
                    {
                        return false;
                    }
                    
                

           return true;
        }

        private static  void NotifyBackupCompletion(bool success)
        {
            string message = success ? "Backup completed successfully." : "Backup failed.";
            Debug.WriteLine($"Notification: {message}");

            //mutuale.WaitOne();
            // Assurez-vous que l'application WPF est initialisée et qu'une fenêtre principale est définie
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                // Vérifiez si la fenêtre principale est de type `MainWindow` (ou le nom de votre fenêtre principale)
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.ShowNotification(message);
                }
            }
            //mutuale.ReleaseMutex();
        }



    }
}
