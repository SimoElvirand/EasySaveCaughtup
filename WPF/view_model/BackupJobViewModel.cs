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
          
            AddBackupCommand = new commands.RelayCommand(AddBackup, CanAddBackup);
            BackupJobListprog = BackupJobModel.GetBackupJobModels();
            BackupJobList = BackupJobModel.GetBackupJobModels();
            SelectedBackupJobs = new ObservableCollection<BackupJobModel>();
            RunBackupCommand = new commands.RelayCommand(RunBackup, CanRunBackup);
            PauseBackupCommand = new commands.RelayCommand<BackupJobModel>(PauseBackup);
            StopBackupCommand = new commands.RelayCommand<BackupJobModel>(StopBackup);
            ResumeBackupCommand = new commands.RelayCommand<BackupJobModel>(ResumeBackup);
           
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

         

            foreach (var selectedBackupJob in SelectedBackupJobs.ToList())
            {
                Thread newThread = new Thread(() =>
                {
                    selectedBackupJob.TotalBackupJobs = SelectedBackupJobs.Count;
                    selectedBackupJob.RunBackupJob(selectedBackupJob);
                });
                newThread.Start();
               
            }
        }

        private bool CanRunBackup(object obj)
        {
            return true;
        }

        

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
