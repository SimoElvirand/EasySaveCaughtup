using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.Text.Json;
using WPF.model;
using System.Diagnostics;

namespace WPF.Network
{
    public class TcpListenerSingleton
    {
        private TcpListener _server;
        private bool _isRunning;
        private ObservableCollection<BackupJobModel> _backupJobList;


        private static TcpListener _tcpListener;
        private static readonly object _lock = new object();

        private TcpListenerSingleton() { }

        public static TcpListener GetInstance()
        {
            if (_tcpListener == null)
            {
                lock (_lock)
                {
                    if (_tcpListener == null)
                    {
                        _tcpListener = new TcpListener(IPAddress.Any, 5000);
                        _tcpListener.Start();
                        Console.WriteLine("TcpListener started on port 5000.");
                    }
                }
            }
            return _tcpListener;
        }
    }

}
