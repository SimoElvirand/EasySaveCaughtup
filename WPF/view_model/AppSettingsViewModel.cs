using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using WPF.commands;
using WPF.model;

namespace WPF.view_model
{
    class AppSettingsViewModel
    {
        public string Extentions { get; set; }
        public string logPath { get; set; }
        public string name { get; set; }
        public long size{ get; set; }

        public ICommand saveLogPathCommand { get; set; }
        public ICommand saveExtensionsCommand { get; set; }

        public ICommand savesizeCommand { get; set; }

        public ICommand savenameCommand { get; set; }

        public AppSettingsViewModel()
        {
            saveLogPathCommand = new RelayCommand(saveLog, CanSaveLog);
            saveExtensionsCommand = new RelayCommand(saveExtensions, CanSaveExtensions);
            savenameCommand = new RelayCommand(savename, CanSavename);
            savesizeCommand = new RelayCommand(savesize, CanSavesize);

        }

        private bool CanSaveExtensions(object obj)
        {
            return true;
        }

        private bool CanSavename(object obj)
        {
            return true;
        }

        private bool CanSavesize(object obj)
        {
            return true;
        }
        private void savename(object obj)
        {
            BackupListManager.names = name;
        }

        private void savesize(object obj)
        {
            BackupListManager.sizes = size;
        }

        private void saveExtensions(object obj)
        {
            BackupListManager.ExtensionsToEncrypt = Extentions;
            Debug.WriteLine(BackupListManager.ExtensionsToEncrypt);
        }

        private bool CanSaveLog(object obj)
        {
            return true;
        }

        private void saveLog(object obj)
        {
            BackupListManager.path = logPath;
            Debug.WriteLine(BackupListManager.path);
        }
    }
}
