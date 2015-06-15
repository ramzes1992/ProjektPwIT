using DesktopApp.Helpers;
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
        public delegate void ImageChangedEventHndler(object sender, byte[] image);

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
            TcpListener myListener = new TcpListener(ipAd, _port);

            myListener.Start();
            while (!_worker.CancellationPending)
            {
                Socket s = myListener.AcceptSocket();
                
                byte[] buff = new byte[1024];
                long frameSize = 0;
                int lenght = s.Receive(buff, SocketFlags.None);
                byte[] famreSizeBuff = buff.Take(lenght).ToArray();

                Array.Reverse(famreSizeBuff);

                frameSize = BitConverter.ToInt32(famreSizeBuff, 0);
                string resp = frameSize.ToString();

                s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.NoDelay, true);
                s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                s.NoDelay = true;
                byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes(resp);
                s.SendBufferSize = sendBuffer.Length;
                s.Send(sendBuffer);

                byte[] frameReceived = new byte[frameSize];

                int i = 0;
                while (i < frameSize)
                {
                    byte[] tmpBuffer = new byte[1024];
                    int receivedBytes = s.Receive(tmpBuffer);
                    for (int j = 0; j < receivedBytes; j++)
                    {
                        frameReceived[i] = tmpBuffer[j];
                        i++;
                    }
                }

                    
                RaiseImageChangedEvent(frameReceived);

                using(Image testImg = Image.FromStream(new MemoryStream(DrawingHelper.GetData())))
                {
                    MemoryStream ms = new MemoryStream();
                    testImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageEncoded = ms.ToArray();

                    s.Send(System.Text.Encoding.ASCII.GetBytes(imageEncoded.Length.ToString()));
                    s.Send(imageEncoded);
                }

                s.Close();
            }
            myListener.Stop();


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

        private void RaiseImageChangedEvent(byte[] image)
        {
            if (ImageChanged != null)
            {
                ImageChanged(this, image);
            }
        }
    }
}
