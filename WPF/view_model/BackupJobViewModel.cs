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

namespace WPF.view_model
{
    class BackupJobViewModel : ViewModelBase
    {
        private ObservableCollection<BackupJobModel> backupJobList;
        private ObservableCollection<BackupJobModel> backupJobListprog;
        private ObservableCollection<BackupJobModel> selectedBackupJobs; // Change here
        private List<ThreadControl> threadControls = new List<ThreadControl>();
        private List<BackupListManager> Backuplist = new List<BackupListManager>();
        
        private int threadIndexCounter = 0;
        private static ManualResetEvent pauseEvent = new ManualResetEvent(true);


        public ICommand ModifyBackupCommand { get; set; }
        public ICommand DeleteBackupCommand { get; set; }

       
        //public ICommand RunBackupCommand { get; set; }

        public ICommand RunBackupCommand { get; set; }
        public ICommand PauseBackupCommand { get; set; }
        public ICommand StopBackupCommand { get; set; }


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
           Instance = this;
            //backupJobList = BackupListManager.GetBackupJobModels();
            //selectedBackupJobs = new ObservableCollection<BackupJobModel>(); // Change here
            //DeleteBackupCommand = new RelayCommand(DeleteBackup, CanDeleteBackup);
            //ModifyBackupCommand = new RelayCommand(ModifyBackup, CanModifyBackup);
            //RunBackupCommand = new RelayCommand(RunBackup, CanRunBackup);
            BackupJobListprog = BackupListManager.GetBackupJobModels();
            BackupJobList = BackupListManager.GetBackupJobModels();
            SelectedBackupJobs = new ObservableCollection<BackupJobModel>();
            RunBackupCommand = new RelayCommand(RunBackup, CanRunBackup);
            PauseBackupCommand = new RelayCommand(PauseBackup, CanPauseBackup);
            StopBackupCommand = new RelayCommand(StopBackup, CanStopBackup);

        }

         private void RunBackup(object obj)
        {

         List<Thread> threads = new List<Thread>();

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
                var backupJobModel = new BackupJobModel(selectedBackupJob.sourceDirectory, selectedBackupJob.destinationDirectory, selectedBackupJob.name, selectedBackupJob.backupType, selectedBackupJob.logChoice)
                {
                    Progress1 = 0,
                    PauseCommand = PauseBackupCommand,
                    StopCommand = StopBackupCommand
                };

                BackupJobListprog.Add(backupJobModel);

                // Start the backup job in a new thread
                backupJobModel.Thread = new Thread(() =>
                {
                    var backupListManager = new BackupListManager(selectedBackupJob, SelectedBackupJobs.Count, backupJobModel);
                    backupListManager.DoWork();
                });
                backupJobModel.Thread.Start();
            }
        }

        private bool CanRunBackup(object obj)
        {
            return true;
        }

        private bool CanPauseBackup(BackupJobModel backupJobModel)
        {
            return backupJobModel?.Thread != null && backupJobModel.Thread.ThreadState == System.Threading.ThreadState.Running;
        }

        private void PauseBackup(object obj)
        {
            // backupJobModel?.Thread?.Suspend();
            pauseEvent.Reset();
        }

        private bool CanStopBackup(BackupJobModel backupJobModel)
        {
            return backupJobModel?.Thread != null && backupJobModel.Thread.ThreadState != System.Threading.ThreadState.Stopped;
        }

        private void StopBackup(object obj)
        {
            //backupJobModel?.Thread?.Abort();
            pauseEvent.Set();
        }

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

        private bool CanStopBackup(object obj)
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
                    BackupListManager.DeleteBackupJob(selectedBackupJob);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Couldn't delete");
            }
        }

    }
}
