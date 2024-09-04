﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;
using WPF.model;
using WPF.view_model;

namespace WPF.model
{
    public class BackupJobModel : ViewModelBase
    {
        private string _name;
        private string _sourceDirectory;
        private string _destinationDirectory;
        private string _backupType;
        private int _logChoice;
        private string _lastTimeRuned;
        private double _progress1;
        private double _TotalFiles;
        private double _FileBackedUp;
        private ICommand _pauseCommand;
        private ICommand _stopCommand;
        private Thread _thread;

        private static string _path = "C://Log";
        private static string _extensionsToEncrypt = "";
        private static Mutex mutuale = new Mutex(false);
        private static Semaphore _semaphore = new Semaphore(1, 10);
        private static long size = 1024;
        private static string namess = "";

        private static ManualResetEventSlim pauseEvent ;

        private static BackupJobModel backupJobModel;
        private static int c;
        private static CancellationTokenSource cancellationTokenSource;


        public double Progress1
        {
            get { return _progress1; }
            set
            {
                if (_progress1 != value)
                {
                    _progress1 = value;
                    OnPropertyChanged(nameof(Progress1));
                    Debug.WriteLine("tous est en marche model" + Progress1);
                }
            }
        }

        public ICommand PauseBackupCommand { get; set; }
        public ICommand StopBackupCommand { get; set; }

        public ICommand ResumeBackupCommand { get; set; }
        public ICommand PauseCommand
        {
            get { return _pauseCommand; }
            set
            {
                _pauseCommand = value;
                OnPropertyChanged(nameof(PauseCommand));
                Debug.WriteLine("tous est en marche pause" + Progress1);
            }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
            set
            {
                _stopCommand = value;
                OnPropertyChanged(nameof(StopCommand));
            }
        }

        public Thread Thread
        {
            get { return _thread; }
            set {
                _thread = value;
                OnPropertyChanged(nameof(StopCommand)); 
                }
        }
        public double FileBackedUp
        {
            get { return _FileBackedUp; }
            set
            {
                if (_FileBackedUp != value)
                {
                    _FileBackedUp = value;
                    OnPropertyChanged(nameof(FileBackedUp));
                    Debug.WriteLine("tous est en marche filebackedup" + FileBackedUp);
                }
            }
        }

        public double TotalFiles
        {
            get { return _TotalFiles; }
            set
            {
                if (_TotalFiles != value)
                {
                    _TotalFiles = value;
                    OnPropertyChanged(nameof(TotalFiles));
                    Debug.WriteLine("tous est en marche totalfiles" + TotalFiles);
                }
            }
        }

        //public BackupJobModel BackupJobModel1
        //{
           // get { return backupJobModel; }
            //set { backupJobModel = value; }
       // }
        public int TotalBackupJobs
        {
            get { return c; }
            set { c = value; }
        }

        public string lastTimeRuned
        {
            get { return _lastTimeRuned; }
            set { _lastTimeRuned = value; }
        }
        public string sourceDirectory
        {
            get { return _sourceDirectory; }
            set { _sourceDirectory = value; }
        }
        public string destinationDirectory
        {
            get { return _destinationDirectory; }
            set { _destinationDirectory = value; }
        }
        public string backupType
        {
            get { return _backupType; }
            set { _backupType = value; }
        }
        public string name
        {
            get { return _name; }
            set { _name = value;}
        }
        public int logChoice
        {
            get { return _logChoice; }
            set { _logChoice = value;}
        }
        //private void BackupJobModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //  if (e.PropertyName == nameof(Progress1))
        // {
        // Notify the BackupJobViewModel about the change
        // BackupJobViewModel.Instance.Progreess = Progress1;
        //}
        // }

      
            public BackupJobModel(string sourceDirectory, string destinationDirectory, string name, string backupType, int logChoice)
        {
            this.sourceDirectory = sourceDirectory;
            this.destinationDirectory = destinationDirectory;
            this.name = name;
            this.backupType = backupType;
            this.logChoice = logChoice;
            this.lastTimeRuned = "Never";
            this.Progress1 = 0;
            //pauseEvent = new ManualResetEventSlim(true); // Initially not paused
            //CancellationTokenSource = new CancellationTokenSource();
            PauseBackupCommand = new commands.RelayCommand(PauseBackup, CanPauseBackup);
            StopBackupCommand = new commands.RelayCommand(StopBackup, CanStopBackup);
            ResumeBackupCommand = new commands.RelayCommand(ResumeBackup, CanResumeBackup);

            // PropertyChanged += BackupJobModel_PropertyChanged;
        }


        //private bool CanPauseBackup(l)
        // {
        //  return backupJobModel?.Thread != null && backupJobModel.Thread.ThreadState == System.Threading.ThreadState.Running;
        // }

        //public void PauseBackup(object obj)
        //{
        // backupJobModel?.Thread?.Suspend();
        //pauseEvent.Reset();
        // Debug.WriteLine("pausemodeldddddddddddd");
        // }

        //private bool CanStopBackup()
        //{
        //    return backupJobModel?.Thread != null && backupJobModel.Thread.ThreadState != System.Threading.ThreadState.Stopped;
        //}

        //// public void StopBackup(object obj)
        //{
        //backupJobModel?.Thread?.Abort();
        //pauseEvent.Set();
        // Debug.WriteLine("stopmodeldddddddddddd");
        // }


        public void PauseBackup(object obj)
        {
            //CancellationTokenSource?.Cancel();
            pauseEvent.Reset();
            
        }

        public void StopBackup(object obj)
        {
            //CancellationTokenSource?.Cancel();
            // Add any additional logic for stopping the backup job
            cancellationTokenSource.Cancel();

        }

        public void ResumeBackup(object obj)
        {
            pauseEvent.Set();
            //CancellationTokenSource = new CancellationTokenSource();
            // Add any additional logic for resuming the backup job
        }

        public static ObservableCollection<BackupJobModel> DatabaseBackupJobs = new ObservableCollection<BackupJobModel>();

        public static ObservableCollection<BackupJobModel> GetBackupJobModels()
        {
            return DatabaseBackupJobs;

        }

        ////manager
        /// 


       // public static List<BackupJobModel> backuplistview
        //{
            //get { return backuplistvieww; }
           // set
            ///{
              //  backuplistvieww = value;
            //}
        //}
        public static string names
         {
            get { return namess; }
            set
            {
               namess = value;

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

        public void RunBackupJob(BackupJobModel backupJobModel)
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
        private async static void sourceDifferentialCopy(BackupJobModel backupJob, int log_choice)
        {
            DirectoryInfo diSource = new DirectoryInfo(backupJob.sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(backupJob.destinationDirectory);
            if (diSource != diTarget)
            {
                cancellationTokenSource = new CancellationTokenSource();
                pauseEvent = new ManualResetEventSlim(true);
                await Task.Run(() => directoryDifferentialCopy2(diSource, diTarget, log_choice, backupJob, cancellationTokenSource.Token));
            }

        }
        public ManualResetEvent PauseEvent { get; private set; }
        //public CancellationTokenSource CancellationTokenSource { get; private set; }
        private static void directoryDifferentialCopy2(DirectoryInfo diSource, DirectoryInfo diTarget, int log_choice, BackupJobModel backupJob, CancellationToken cancellationToken)
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
                   
                     pauseEvent.Wait(cancellationToken);  // Attendre ici si la pause est activée.
                    cancellationToken.ThrowIfCancellationRequested();

                    Stopwatch stopwatch = new Stopwatch();
                    Console.Write("diff copy done for file : " + v.Name);
                    Thread.Sleep(1000);
                    stopwatch.Start();
                    bool isFileEndsWithExtension = checkIfFileEndsWithExtension(v.Name, ExtensionsToEncrypt);
                        if (isFileEndsWithExtension)
                        {
                            Thread.Sleep(1000);
                            string outputPath = System.IO.Path.Combine(diTarget.FullName, v.Name);
                            RunXorExecutable(v.FullName, outputPath + "_encrypted");
                            Thread.Sleep(1000);
                        }
                        else
                        {

                            foreach (var i in queryList1Only)
                            {
                                if (CalculSizeFile(size, i.Length) & i.Name == namess)
                                {
                                    Thread.Sleep(1000);
                                    i.CopyTo(System.IO.Path.Combine(diTarget.ToString(), i.Name), true);
                                    Thread.Sleep(1000);
                                }
                            }
                            //mutuale.WaitOne();
                            if (CalculSizeFile(size, v.Length) & v.Name != namess)
                            {
                                Thread.Sleep(1000);
                                v.CopyTo(System.IO.Path.Combine(diTarget.ToString(), v.Name), true);
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
                    backupJob.FileBackedUp = totalFiles - processedFiles;
                    backupJob.TotalBackupJobs = totalFiles;
                    Debug.WriteLine("in process" + processedFiles++);
                    Debug.WriteLine("in process" + backupJob.Progress1);
                }

                bool success = processedFiles == totalFiles;
                 NotifyBackupCompletion(success);

                foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        diTarget.CreateSubdirectory(diSourceSubDir.Name);
                    directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob, cancellationToken);
                }
                return;
            }

            if (Directory.Exists(diTarget.FullName) == false)
            {
                Directory.CreateDirectory(diTarget.FullName);
            }

            foreach (FileInfo fi in diSource.GetFiles())
            {
                pauseEvent.Wait(cancellationToken);  // Attendre ici si la pause est activée.
                cancellationToken.ThrowIfCancellationRequested();

                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine(@"Copying {0}\{1}", diTarget.FullName, fi.Name);
                Console.Write("diff copy done for file : " + fi.Name);
                stopwatch.Start();
                bool isFileEndsWithExtension = checkIfFileEndsWithExtension(fi.Name, ExtensionsToEncrypt);
                if (isFileEndsWithExtension)
                {
                    string inputPath = System.IO.Path.Combine(diSource.FullName, fi.Name);
                    string outputPath = System.IO.Path.Combine(diTarget.FullName, fi.Name);
                    RunXorExecutable(inputPath, outputPath + "_encrypted");
                }
                else
                {
                    fi.CopyTo(System.IO.Path.Combine(diTarget.ToString(), fi.Name), true);
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
                directoryDifferentialCopy2(diSourceSubDir, nextTargetSubDir, log_choice, backupJob, cancellationToken);
            }
        }

        ////enddiffback
        ///
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
            if (fileSizeInKB > size)
            {
                return false;
            }



            return true;
        }

        private static bool checkIfFileEndsWithExtension(string file, string extensions)
        {
            if (extensions == null)
            {
                return false;
            }
            else if (extensions == "")
            {
                return false;
            }
            string[] extensionsList = extensions.Split(','); ;
            if (extensionsList.Count() == 0)
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
        private static void NotifyBackupCompletion(bool success)
        {
            string message = success ? "Backup completed successfully." : "Backup failed.";
            Debug.WriteLine($"Notification: {message}");

            //mutuale.WaitOne();
            // Assurez-vous que l'application WPF est initialisée et qu'une fenêtre principale est définie
            Thread newThread = new Thread(() =>
            {
                // Ici, vous exécutez votre logique qui peut être en arrière-plan

                // Utiliser le Dispatcher pour s'assurer que la mise à jour de l'UI se fait sur le thread principal de l'UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Application.Current != null && Application.Current.MainWindow != null)
                    {
                        // Vérifiez si la fenêtre principale est de type `MainWindow` (ou le nom de votre fenêtre principale)
                        if (Application.Current.MainWindow is MainWindow mainWindow)
                        {
                            mainWindow.ShowNotification(message);
                        }
                    }
                });
            });

            // Démarrer le nouveau thread
            newThread.Start();
            //mutuale.ReleaseMutex();
        }
        private bool CanPauseBackup(object obj)
        {
            return true; // Change here
        }
        private bool CanStopBackup(object obj)
        {
            return true; // Change here
    
        }

        private bool CanResumeBackup(object obj) { return true; }





    }


}
