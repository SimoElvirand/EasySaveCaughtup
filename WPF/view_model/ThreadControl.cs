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

namespace WPF.view_model
{
    public class ThreadControl
    {
        private Thread thread;
        private ManualResetEvent pauseEvent;
        private bool stopRequested;
        private BackupJobModel backupJob;
        private int jobCount;

        public ThreadControl(BackupJobModel backupJob, int jobCount)
        {
            this.backupJob = backupJob;
            this.jobCount = jobCount;
            pauseEvent = new ManualResetEvent(true);
            thread = new Thread(DoWork);
            stopRequested = false;
        }

        public void Start()
        {
            thread.Start();
        }

        public void Pause()
        {
            pauseEvent.Reset();  // Mettre en pause le thread
            //BackupListManager.PauseBackup();
            Debug.WriteLine($"Thread pour {backupJob.name} en pause Thread ID threadcontrol:" + Thread.CurrentThread.ManagedThreadId);
        }

        public void Resume()
        {
            pauseEvent.Set();  // Reprendre le thread
            BackupListManager.ResumeBackup();
            Debug.WriteLine($"Thread pour {backupJob.name} repris.");
        }

        public void Stop()
        {
            stopRequested = true;  // Arrêter le thread
            pauseEvent.Set();  // S'assurer que le thread n'est pas en pause lorsqu'il est arrêté
            Debug.WriteLine($"Thread pour {backupJob.name} arrêté.");
        }

       

        private void DoWork()
        {
            while (!stopRequested)
            {
                pauseEvent.WaitOne();  // Attendre que le thread soit repris

                // Exécution de la sauvegarde
                Debug.WriteLine($"Travail de sauvegarde pour {backupJob.name} en cours...");
                Thread.Sleep(10);
                BackupListManager.RunBackupJob(backupJob, jobCount);

                Thread.Sleep(10);  // Simule un travail qui prend du temps
            }

            Debug.WriteLine($"Thread pour {backupJob.name} terminé.");
        }
      }
    }
