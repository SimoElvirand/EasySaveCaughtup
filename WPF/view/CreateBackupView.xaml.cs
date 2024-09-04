using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using WPF.model;
using WPF.view_model;

namespace WPF.view
{
    /// <summary>
    /// Interaction logic for CreateBackupView.xaml
    /// </summary>
    public partial class CreateBackupView : Page
    {
        public CreateBackupView()
        {
            InitializeComponent();
            BackupJobViewModel addBackupJobViewModel = new BackupJobViewModel();
            this.DataContext = addBackupJobViewModel;
            LanguageManager.Instance.LanguageChanged += LanguageChangedHandler;
            UpdateUI();
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            // Update UI elements with localized resources here
            // For example, update TextBlock.Text, Button.Content, etc.
        }
        private void Source_File_Window_Button(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Update the SourceDirectory property in the ViewModel
                    BackupJobViewModel addBackupJobViewModel = (BackupJobViewModel)this.DataContext;
                    addBackupJobViewModel.SourceDirectoryy = folderBrowserDialog.SelectedPath;
                    sourcePath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void Destination_File_Window_Button(object sender, RoutedEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Update the DestinationDirectory property in the ViewModel
                    BackupJobViewModel addBackupJobViewModel = (BackupJobViewModel)this.DataContext;
                    addBackupJobViewModel.DestinationDirectory = folderBrowserDialog.SelectedPath;
                    targetPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }


    }
}
