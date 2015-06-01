using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleTestApp
{
    class Program
    {
        static System.Timers.Timer myTimer = new System.Timers.Timer();
        static Stopwatch stopWatch = new Stopwatch();

        static void Main(string[] args)
        {
            IPAddress ipAd = IPAddress.Parse("192.168.0.103");
            TcpListener myList = new TcpListener(ipAd, 6666);
            myList.Start();

            Socket s = myList.AcceptSocket();
            var picture = new List<byte[]>();
            int size = 0;
            int counter = 0;
            while (true)
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
                        var bmp = byteArrayToImage(result);
                        bmp.Save("somefile" + counter.ToString() + ".bmp");
                        Console.WriteLine("Saved file: {0}", "somefile" + counter.ToString() + ".bmp");
                        counter++;
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

            //s.Close();
            //myList.Stop();

            //byte[] result = ConcatByteArrays(picture, size, 1024);

            //var bmp = byteArrayToImage(result);
            //bmp.Save("somefile.bmp");
            Console.WriteLine(picture.Sum(t => t.Count()));



            Console.ReadKey();
        }

        private static byte[] ConcatByteArrays(List<byte[]> picture, int size, int bufferSize)
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

        public static Bitmap byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return new Bitmap(returnImage);
        }
    }
}
