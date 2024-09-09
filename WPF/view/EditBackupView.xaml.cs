using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.view_model;
using WPF.view;
using WPF.model;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using WPF.Network;


namespace WPF.view
{
    /// <summary>
    /// Interaction logic for EditBackupView.xaml
    /// </summary>
    public partial class EditBackupView : Page
    {
        private bool isPaused = false;
        private bool IsStop = false;
        int c = 0;
        private static ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private System.Timers.Timer processCheckTimer;
        private string[] allowedProcesses = {"WPF.exe, EasySave.exe, Calculatrice.exe" }; // Ajoutez ici les noms des processus autorisés

        static byte[] Buffer { get; set; }
        static Socket sck;
        List<BackupJobModel> selectbackupList;
        public EditBackupView()
        {
            InitializeComponent();
            InitializeProcessCheckTimer();
            // Create an instance of the view model
            BackupJobViewModel viewModel = new BackupJobViewModel();
            //BackupJobModel p = new BackupJobModel();
            //p.StartTcpServer();
            // Set the view model as the DataContext for the view
            this.DataContext = viewModel;
          //DataContext = p;
        }


        private void InitializeProcessCheckTimer()
        {
            processCheckTimer = new System.Timers.Timer(5000); // Vérifie toutes les 5 secondes
            
            processCheckTimer.AutoReset = true;
            processCheckTimer.Enabled = true;
        }


       
        /*    protected override void OnFormClosed(FormClosedEventArgs e)
        {
            processCheckTimer.Stop();  // Arrête le timer
            processCheckTimer.Dispose();  // Libère les ressources utilisées par le timer
            base.OnFormClosed(e);  // Appelle la méthode de base pour la fermeture de la fenêtre
        }*/
        private void UserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the previous selection
            (DataContext as BackupJobViewModel)?.SelectedBackupJobs.Clear();
            // Process the new selections
            foreach (var selectedBackup in UserGrid.SelectedItems)
            {
                if (selectedBackup is BackupJobModel backupJob)
                {
                    // Add selected backups to the view model
                    (DataContext as BackupJobViewModel)?.SelectedBackupJobs.Add(backupJob);
                }
            }
            Debug.WriteLine((DataContext as BackupJobViewModel)?.SelectedBackupJobs.Count);
            selectbackupList = (DataContext as BackupJobViewModel)?.SelectedBackupJobs.ToList();

            // Update the UI based on the first selected item (if any)
            if (UserGrid.SelectedItem != null)
            {
                BackupJobModel selectedBackupJob = (BackupJobModel)UserGrid.SelectedItem;
                nameTXT.Text = selectedBackupJob.name;
                sourceTXT.Text = selectedBackupJob.sourceDirectory;
                destinationTXT.Text = selectedBackupJob.destinationDirectory;
                typeTXT.Text = selectedBackupJob.backupType.ToString();
                logTXT.Text = selectedBackupJob.logChoice.ToString();
            }
        }


        // Méthode qui initialise la barre de progression 
        

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //initialisation de la barre de progression avec le pourcentage de progression
           // pbstatus1.Value = e.ProgressPercentage;

            //Affichage de la progression sur un label
            //lb_etat_prog_server.Content = pbstatus1.Value.ToString() + "%";
            



        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            c = 0;
           
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
            
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            c++;
            if (c < 2)
            {
                isPaused = !isPaused;
            }
            else
            {
                isPaused = false;
                c = 0;
            }
        }

        private double estimateTime()
        {
            double estimatedCopyTimeSeconds = 0;
            // Chemin source et destination du fichier
            
            foreach (var select in selectbackupList)
            {
                string sourceFilePath = select.sourceDirectory;
                string destinationFilePath = select.destinationDirectory;

                // Obtention de la taille du fichier source
                long fileSizeBytes = GetDirectorySize(sourceFilePath);

                // Estimation de la vitesse de transfert moyenne (en bytes par seconde)
                double averageTransferSpeedBytesPerSecond = 2 * 1024 * 1024; // Exemple : 10 MB/s

                // Calcul du temps estimé de copie (en secondes)
                estimatedCopyTimeSeconds += fileSizeBytes / averageTransferSpeedBytesPerSecond;
            }

            //socket part start
            //sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            //sck.Connect(localEndPoint);
            //Console.WriteLine("Unable to connect to remote end point!\r\n");
            //Main(args);

            //string r = estimatedCopyTimeSeconds.ToString();
            //Console.WriteLine("enter text go");
            //byte[] data = Encoding.ASCII.GetBytes(r);
            //sck.Send(data);
            //Console.Write("data send");
            //Console.ReadLine();
            //sck.Close();
            //end
            return estimatedCopyTimeSeconds;
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isPaused = false;
            c = 0;
            IsStop = true;
            //pbstatus1.Value = 0;
            //lb_etat_prog_server.Content = pbstatus1.Value.ToString() + "%";
        }
        static long GetDirectorySize(string directoryPath)
        {
            long totalSize = 0;

            try
            {
                // Obtenir la taille de tous les fichiers dans le répertoire
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    totalSize += new FileInfo(file).Length;
                }

                // Récursivement parcourir les sous-répertoires
                string[] subdirectories = Directory.GetDirectories(directoryPath);
                foreach (string subdirectory in subdirectories)
                {
                    totalSize += GetDirectorySize(subdirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }

            return totalSize;
        }

        private void pauseclick(object sender, RoutedEventArgs e)
        {
       
         
            
            
        }
    }
}
