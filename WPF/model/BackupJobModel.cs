using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using WPF.view_model;

namespace WPF
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
        private void BackupJobModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Progress1))
            {
                // Notify the BackupJobViewModel about the change
                BackupJobViewModel.Instance.Progreess = Progress1;
            }
        }

        public BackupJobModel(string sourceDirectory, string destinationDirectory, string name, string backupType, int logChoice)
        {
            this.sourceDirectory = sourceDirectory;
            this.destinationDirectory = destinationDirectory;
            this.name = name;
            this.backupType = backupType;
            this.logChoice = logChoice;
            this.lastTimeRuned = "Never";
            PropertyChanged += BackupJobModel_PropertyChanged;
        }
    }

}
