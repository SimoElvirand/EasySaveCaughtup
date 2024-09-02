using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.view
{
    /// <summary>
    /// Logique d'interaction pour progresbar.xaml
    /// </summary>
    public partial class progresbar : Page
    {
        private bool isRunning = false;
        private bool isPaused = false;
        private bool IsStop = false;
        private Thread progressThread;
        int c = 0;
        private static ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public progresbar()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                progressThread = new Thread(RunProgress);
                progressThread.Start();
            }
        }

        private void RunProgress()
        {
            for (int i = 0; i <= 100; i++)
            {
                while (isPaused)
                {
                    pauseEvent.Set(); // Pause for 100 milliseconds
                    continue;
                }
                if (IsStop)
                {
                    cancellationTokenSource.Dispose();
                    break;
                }

                Dispatcher.Invoke(() => progressBar.Value = i); // Update progress bar on UI thread
                Thread.Sleep(100); // Simulate some work (adjust as needed)
            }

            isRunning = false;
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            isPaused = false;
            IsStop = true;
            progressBar.Value = 0;
        }
    }
}
