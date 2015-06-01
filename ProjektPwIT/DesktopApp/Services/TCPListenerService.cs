using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopApp.Services
{
    public class TCPListenerService
    {
        private string _ipAddress;
        private int _port;
        private BackgroundWorker _worker = new BackgroundWorker();

        private bool _isRunning = false;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }

        public event ImageChangedEventHndler ImageChanged;
        public delegate void ImageChangedEventHndler(object sender, Bitmap image);

        public TCPListenerService(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IPAddress ipAd = IPAddress.Parse(_ipAddress);
            TcpListener myList = new TcpListener(ipAd, _port);

            // Start Listeneting at the specified port /
            myList.Start();

            Socket s = myList.AcceptSocket();
            var picture = new List<byte[]>();
            int size = 0;
            while (!_worker.CancellationPending)
            {
                byte[] b = new byte[1024];
                int k = s.Receive(b);
                size += k;

                if (k <= 0 && size > 0)
                {
                    s = myList.AcceptSocket();

                    byte[] result = ConcatByteArrays(picture, size, 1024);
                    try
                    {
                        //var bmp = ByteArrayToImage(result);
                        RaiseImageChangedEvent(ByteArrayToImage(result));
                    }
                    catch (Exception ex)
                    {
                        //continue;
                        Console.WriteLine(ex.Message);
                    }

                    size = 0;
                    picture.Clear();
                }
                else
                {
                    picture.Add(b);
                }

                Thread.Sleep(2);
            }

            s.Close();
            myList.Stop();
        }

        private Bitmap ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return new Bitmap(returnImage);
        }

        private byte[] ConcatByteArrays(List<byte[]> picture, int size, int bufferSize)
        {
            byte[] result = new byte[size];

            for (int i = 0; i < picture.Count; i++)
            {
                for (int j = 0; j < bufferSize; j++)
                {
                    if ((i * bufferSize) + j < size)
                    {
                        result[(i * bufferSize) + j] = picture[i][j];
                    }
                }
            }
            return result;
        }

        public void RunServiceAsync()
        {
            _worker.RunWorkerAsync();
            _isRunning = true;
        }

        public void CancelServiceAsync()
        {
            _worker.CancelAsync();
            _isRunning = false;
        }

        private void RaiseImageChangedEvent(Bitmap image)
        {
            if (ImageChanged != null)
            {
                ImageChanged(this, image);
            }
        }
    }
}
