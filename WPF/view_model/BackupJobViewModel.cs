using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Threading;

using WPF.commands;
using WPF.model;


using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WPF.Network;

namespace WPF.view_model
{
    class BackupJobViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private  ObservableCollection<BackupJobModel> backupJobList;
        private  ObservableCollection<BackupJobModel> backupJobListprog;
        private ObservableCollection<BackupJobModel> selectedBackupJobs; // Change here
        private List<ThreadControl> threadControls = new List<ThreadControl>();
        private List<BackupListManager> Backuplist = new List<BackupListManager>();
        

        private int threadIndexCounter = 0;
        private static ManualResetEvent pauseEvent = new ManualResetEvent(true);


        public ICommand ModifyBackupCommand { get; set; }
        public ICommand DeleteBackupCommand { get; set; }

        public ICommand PauseBackupCommand { get; set; }
        public ICommand StopBackupCommand { get; set; }

        public ICommand ResumeBackupCommand { get; set; }
        private TcpServer _tcpServer;



        //public ICommand RunBackupCommand { get; set; }

        public ICommand RunBackupCommand { get; set; }
        //public ICommand PauseBackupCommand { get; set; }
        //public ICommand StopBackupCommand { get; set; }


        public static BackupJobViewModel Instance { get; private set; }

        private double _progreess;
        public double Progreess
        {
            get { return _progreess; }
            set
            {
                if (_progreess != value)
                {
                    _progreess = value;
                    OnPropertyChanged(nameof(Progreess));
                    Debug.WriteLine("tous est en marche viewmodel" + Progreess);
                }
            }
        }
        public ObservableCollection<BackupJobModel> BackupJobList
        {
            get { return backupJobList; }
            set
            {
                backupJobList = value;
                OnPropertyChanged(nameof(BackupJobList));
            }
        }
        public ObservableCollection<BackupJobModel> BackupJobListprog
        {
            get { return backupJobListprog; }
            set
            {
                backupJobListprog = value;
                OnPropertyChanged(nameof(BackupJobListprog));
            }
        }
        public List<BackupListManager> _Backuplist
        {
            get => Backuplist;
            set
            {
                Backuplist = value;
                OnPropertyChanged(nameof(_Backuplist));
            }
        }



        public ObservableCollection<BackupJobModel> SelectedBackupJobs // Change here
        {
            get { return selectedBackupJobs; }
            set
            {
                selectedBackupJobs = value;
                OnPropertyChanged(nameof(SelectedBackupJobs));
            }
        }

        public BackupJobViewModel()
        {
           //Instance = this;
            //backupJobList = BackupListManager.GetBackupJobModels();
            //selectedBackupJobs = new ObservableCollection<BackupJobModel>(); // Change here
            //DeleteBackupCommand = new RelayCommand(DeleteBackup, CanDeleteBackup);
            //ModifyBackupCommand = new RelayCommand(ModifyBackup, CanModifyBackup);
            //RunBackupCommand = new RelayCommand(RunBackup, CanRunBackup);
            //AddBackupCommand = new RelayCommand(AddBackup, CanAddBackup);
            AddBackupCommand = new commands.RelayCommand(AddBackup, CanAddBackup);
            BackupJobListprog = BackupJobModel.GetBackupJobModels();
            BackupJobList = BackupJobModel.GetBackupJobModels();
            SelectedBackupJobs = new ObservableCollection<BackupJobModel>();
            RunBackupCommand = new commands.RelayCommand(RunBackup, CanRunBackup);
            //PauseBackupCommand = new commands.RelayCommand(PauseBackup, CanPauseBackup);
            //StopBackupCommand = new commands.RelayCommand(StopBackup, CanStopBackup);
            ///ResumeBackupCommand = new commands.RelayCommand(ResumeBackup, CanResumeBackup);
            //PauseBackupCommand = new commands.RelayCommand(PauseBackup, CanPauseBackup);
            //StopBackupCommand = new RelayCommand<BackupJobModel>(StopBackup, CanStopBackup);
            PauseBackupCommand = new commands.RelayCommand<BackupJobModel>(PauseBackup);
            StopBackupCommand = new commands.RelayCommand<BackupJobModel>(StopBackup);
            ResumeBackupCommand = new commands.RelayCommand<BackupJobModel>(ResumeBackup);
            //_tcpServer = new TcpServer(12345); // Choose an appropriate port number
            //_tcpServer.Start();




        }


        //private async void RunBackup(object obj)
        //{
        //foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
        //{
        //selectedBackupJob.CancellationTokenSource = new CancellationTokenSource();
        //await Task.Run(() => selectedBackupJob.RunBackupJob(selectedBackupJob, selectedBackupJob.CancellationTokenSource.Token));
        // }
        //}

        
        private void RunBackup(object obj)
        {

         //List<Thread> threads = new List<Thread>();

            //Parallel.ForEach(SelectedBackupJobs.ToList(), selectedBackupJob =>
            //{
            //Debug.WriteLine("trying to run in parallel");
            //BackupListManager.RunBackupJob(selectedBackupJob, selectedBackupJobs.Count);
            //});

            //foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
            //{

            //new Thread(() =>
            // {

            //Debug.WriteLine("trying to run in parallel");
            // BackupListManager.RunBackupJob(selectedBackupJob, selectedBackupJobs.Count);
            //}).Start();

            //}
            //foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
            //{
            // int currentIndex = threadIndexCounter++;
            // var BackList = new BackupListManager(selectedBackupJob, selectedBackupJobs.Count);
            // _Backuplist.Add(BackList);
            // _Backuplist = new List<BackupListManager>(_Backuplist);
            // Debug.WriteLine("trying to run in add a thread contorler"+ currentIndex);
            // BackList.Start();
            //}

            foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
            {
                Thread newThread = new Thread(() =>
                {
                    selectedBackupJob.TotalBackupJobs = SelectedBackupJobs.Count;
                    selectedBackupJob.RunBackupJob(selectedBackupJob);
                });
                newThread.Start();
               // var backupJobModel = new BackupJobModel(selectedBackupJob.sourceDirectory, selectedBackupJob.destinationDirectory, selectedBackupJob.name, selectedBackupJob.backupType, selectedBackupJob.logChoice);


                //BackupJobListprog.Add(backupJobModel);

                // Start the backup job in a new thread
                //backupJobModel.Thread = new Thread(() =>
                //{
                //    var backupListManager = new BackupListManager(selectedBackupJob, SelectedBackupJobs.Count, backupJobModel);
               ///     backupListManager.DoWork();
               ///// });
                //backupJobModel.Thread.Start();
            }
        }

        private bool CanRunBackup(object obj)
        {
            return true;
        }

        //private bool CanPauseBackup(BackupJobModel backupJobModel)
        //{
            //return true;
       // }

        //private void PauseBackup(object obj)
       // {
            // backupJobModel?.Thread?.Suspend();
            //backupJobModel?.PauseBackup();
            //var current = BackupJobListprog.FirstOrDefault();
            //if (current != null)
            //{
               // current.PauseBackup();
           // }
            //Debug.WriteLine("pauseviewmodeldddddddddddd");
        //}

        //private bool CanStopBackup(object obj)
       // {
           // return true;
        //}

        private void PauseBackup(BackupJobModel backupJob)
        {
            Debug.WriteLine("viewmodel" + backupJob.ToString());
            backupJob.PauseBackup();
            
        }

        private void StopBackup(BackupJobModel backupJob)
        {
            backupJob.StopBackup();
        }

        private void ResumeBackup(BackupJobModel backupJob)
        {
            backupJob.ResumeBackup();
        }



        //private void ResumeBackup(object obj)
        //{
        // var current = BackupJobListprog.FirstOrDefault();
        //if (current != null)
        //{
        // current.ResumeBackup();
        //}
        // }
        // private void StopBackup(object obj)
        //{
        //backupJobModel?.Thread?.Abort();
        //backupJobModel?.StopBackup();
        // var current = BackupJobListprog.FirstOrDefault();
        // Debug.WriteLine($"Stop backup: {current}");
        //if (current != null)
        // {
        //current.StopBackup();
        ///}

        //backupJobModel.StopBackup();
        // Debug.WriteLine("objstop");

        // Debug.WriteLine("stopviewmodeldddddddddddd");
        //}

        //Thread[] threads = new Thread[5];
        //static Dictionary<int, ManualResetEventSlim> threadControlEvents = new Dictionary<int, ManualResetEventSlim>();
        //private async void RunBackup(object obj)
        //{
        // Liste pour stocker toutes les tâches de sauvegarde
        //List<Task> backupTasks = new List<Task>();
        //List<BackupJobModel> backupJobs = new List<BackupJobModel>();

        // backupJobs = SelectedBackupJobs.ToList();
        // BackupListManager.backuplistview = backupJobs;
        // Parcourir chaque tâche de sauvegarde sélectionnée et les démarrer de manière asynchrone
        //foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
        //  {
        // int currentIndex = threadIndexCounter++;
        //var threadControl = new ThreadControl(selectedBackupJob, selectedBackupJobs.Count);
        // threadControls.Add(threadControl);
        // Debug.WriteLine("Trying to run and add a thread controller " + currentIndex);
        ///Debug.WriteLine("in1 ");
        //var resetEvent = new ManualResetEventSlim(true); // Commence en état non bloqué
        //int threadIndex = currentIndex;
        //threadControlEvents[threadIndex] = resetEvent;
        //threads[threadIndex] = new Thread(() => {
        // BackupListManager.RunBackupp();
        // Debug.WriteLine("Trying to run and add a thread controller " + currentIndex);
        //});
        // threads[threadIndex].Start();
        // Utiliser Task.Run pour démarrer chaque sauvegarde de manière asynchrone
        //Task backupTask = Task.Run(() =>
        //{
        // Appeler la méthode de sauvegarde de manière synchrone ici
        //BackupListManager.RunBackupJob(selectedBackupJob, selectedBackupJobs.Count);
        //});

        // Ajouter la tâche à la liste des tâches
        //backupTasks.Add(backupTask);
        //}

        // Attendre que toutes les tâches de sauvegarde soient terminées
        // await Task.WhenAll(backupTasks);

        //Debug.WriteLine("All backup jobs have completed.");
        // }

        public void PauseAll(object obj)
        {
            //foreach (var control in threadControls)
            //{
                Debug.WriteLine("trying  Pauseeeee");
                //control.Pause();
                //threadControls[1].Pause();
                Debug.WriteLine("-------------------------------------" + Thread.CurrentThread.ManagedThreadId);
               //
               //
               //threadControlEvents[1].Reset();
                // BackupListManager.PauseBackup();
                Debug.WriteLine("trying to Pauseeeee Thread ID:" + Thread.CurrentThread.ManagedThreadId);
           // }
        }

        public void ResumeAll(object obj)
        {
            foreach (var control in threadControls)
            {
                control.Resume();
                Debug.WriteLine("trying to resume");
            }
        }

        public void StopAll(object obj)
        {
            foreach (var control in threadControls)
            {
                control.Stop();
                Debug.WriteLine("trying to Stop");
            }
        }

        
        public void PauseThread(int index, object obj)
        {
            if (index >= 0 && index < threadControls.Count)
            {
                threadControls[index].Pause();
                Debug.WriteLine("trying to PauseThread");
            }
        }

        public void ResumeThread(int index)
        {
            if (index >= 0 && index < threadControls.Count)
            {
                threadControls[index].Resume();
                Debug.WriteLine("trying to ResumeThread");
            }
        }

        public void StopThread(int index)
        {
            if (index >= 0 && index < threadControls.Count)
            {
                threadControls[index].Stop();
                Debug.WriteLine("trying to StopThread");
            }
        }

        //private bool CanRunBackup(object obj)
        //{
            //return true; // Change here
        //}

        private bool CanModifyBackup(object obj)
        {
            return SelectedBackupJobs.Count == 1; // Change here
        }

        private void ModifyBackup(object obj)
        {
            // Implement modification logic for the first selected backup job (if any)
            if (SelectedBackupJobs.Count == 1)
            {
                var selectedBackupJob = SelectedBackupJobs[0];
                // Implement modification logic for selectedBackupJob
            }
        }

        private bool CanDeleteBackup(object obj)
        {
            return true; // Change here
        }

        private bool CanPauseBackup(object obj)
        {
            return true; // Change here
        }

        private bool CanResumeBackup(object obj)
        {
            return true; // Change here
        }
        private void DeleteBackup(object obj)
        {
            try
            {
                Debug.WriteLine(SelectedBackupJobs.Count);
                foreach (var selectedBackupJob in SelectedBackupJobs.ToList()) // Use ToList() to create a copy
                {
                    //BackupListManager.DeleteBackupJob(selectedBackupJob);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Couldn't delete");
            }
        }

        public ICommand AddBackupCommand { get; set; }
        
        private bool CanAddBackup(object obj)
        {
            return true;
        }

        /////////////addddddddddbaaaaaaaaackup
        ///

        public string Name { get; set; }
        public string SourceDirectoryy { get; set; }
        public string DestinationDirectory { get; set; }
        public string BackupType { get; set; }
        public int LogChoice { get; set; }
        public bool IsDifferentialBackup { get; set; }
        public bool IsFullBackup { get; set; }
        public bool IsXmlLogType { get; set; }
        public bool IsJsonLogType { get; set; }
        //public ICommand AddBackupCommand { get; set; }


        private void AddBackup(object obj)
        {
            Debug.WriteLine("Trying to add");
            try
            {
                if (IsDifferentialBackup)
                {
                    BackupType = "Diff";
                }
                else if (IsFullBackup)
                {
                    BackupType = "Full";
                };
                if (IsXmlLogType && IsJsonLogType)
                {
                    LogChoice = 3;
                }
                else if (IsXmlLogType)
                {
                    LogChoice = 1;
                }
                else if (IsJsonLogType)
                {
                    LogChoice = 2;
                }

                Debug.WriteLine(SourceDirectoryy);
                Debug.WriteLine("==============================");
                Debug.WriteLine(Name);
                BackupJobModel backupToBeAdded = new BackupJobModel(SourceDirectoryy, DestinationDirectory, Name, BackupType, LogChoice);
                BackupJobModel.DatabaseBackupJobs.Add(backupToBeAdded);
                Debug.WriteLine(BackupJobModel.DatabaseBackupJobs.Count);
                //BackupJobModel.RunBackup()
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }
       // private bool CanAddBackup(object obj)
        //{
          //  return true;
        //}


    }
}
