using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WPF.Network
{
    public class TcpServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public TcpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _listener.Start();
            Task.Run(() => ListenForClients(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }

        private async Task ListenForClients(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Task.Run(() => HandleClient(client, cancellationToken));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");
                    // Handle the received message (e.g., start, stop, pause backup jobs)
                    HandleMessage(message);
                }
            }
        }

        private void HandleMessage(string message)
        {
            // Implement your message handling logic here
            // For example, parse the message and call appropriate methods on your ViewModel
            if (message == "start")
            {
                // Call the method to start backup jobs
            }
            else if (message == "stop")
            {
                // Call the method to stop backup jobs
            }
            else if (message == "pause")
            {
                // Call the method to pause backup jobs
            }
            else if (message == "resume")
            {
                // Call the method to resume backup jobs
            }
        }
    }

}
