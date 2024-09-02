using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF.view_model;
using WPF.view;
using System.Threading;
using WPF.model;



namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "EasyBackupUniqueName7896";

            // Try to create a new Mutex instance.
            mutex = new Mutex(true, mutexName, out bool createdNew);

            // If createdNew is false, it means another instance is already running.
            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0); // Exit the application.
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Release the Mutex when the application exits.
            mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}

