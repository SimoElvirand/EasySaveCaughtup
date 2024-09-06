using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Printing.IndexedProperties;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using WPF.commands;
using WPF.model;

namespace WPF.view_model
{
    class AddBackupJobViewModel
    {

        public string Name { get; set; }
        public string SourceDirectoryy { get; set; }
        public string DestinationDirectory { get; set; }
        public string BackupType { get; set; }
        public int LogChoice { get; set; }
        public bool IsDifferentialBackup { get; set; }
        public bool IsFullBackup { get; set; }
        public bool IsXmlLogType { get; set; }
        public bool IsJsonLogType { get; set; }
        public ICommand AddBackupCommand { get; set; }

        public AddBackupJobViewModel()
        {
            AddBackupCommand = new RelayCommand(AddBackup, CanAddBackup);
        }

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
                //BackupJobModel.DatabaseBackupJobs.Add(backupToBeAdded);
                //Debug.WriteLine(BackupJobModel.DatabaseBackupJobs.Count);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }
        private bool CanAddBackup(object obj)
        {
            return true;
        }

    }  
}
