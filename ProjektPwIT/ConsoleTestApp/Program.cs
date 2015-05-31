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
            //bool done = false;

            //UdpClient listener = new UdpClient(6500);
            //IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 6500);

            //try
            //{
            //    while (!done)
            //    {
            //        Console.WriteLine("Waiting for broadcast");
            //        byte[] bytes = listener.Receive(ref groupEP);

            //        Console.WriteLine("Received broadcast from {0} :\n {1}\n",
            //            groupEP.ToString(),
            //            Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            //    }

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            //finally
            //{
            //    listener.Close();
            //}
                IPAddress ipAd = IPAddress.Parse("192.168.0.103");
                // use local m/c IP address, and 
                // use the same in the client

                // Initializes the Listener /
                TcpListener myList = new TcpListener(ipAd, 6666);

                // Start Listeneting at the specified port /
                myList.Start();

                Console.WriteLine("The server is running at port 6666...");
                Console.WriteLine("The local End point is  :" +
                                  myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");

                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                byte[] mdat = { Convert.ToByte('m'), 
                                Convert.ToByte('d'), 
                                Convert.ToByte('a'), 
                                Convert.ToByte('t') };

                int headers = 10;

                var picture = new List<byte[]>();
                int size = 0;
                while (true)
                {
                    byte[] b = new byte[1024];
                    int k = s.Receive(b);
                    size += k;
                    //if (headers > 0)
                    //{
                    //    Console.WriteLine("\n======HEADER========\n");
                    //    Console.WriteLine(System.Text.Encoding.UTF8.GetString(b));
                    //    headers--;
                    //    continue;
                    //}
                    //else
                    //{
                    //    break;
                    //}

                    if (k <= 0) 
                    {
                        break; 
                    }
                    picture.Add(b);

                    Thread.Sleep(2);

                    //stopWatch.Start();
                    //Console.WriteLine("\n======Recieved========\n" + k.ToString() + "\n");
                    //Console.WriteLine(System.Text.Encoding.UTF8.GetString(b));
                    //stopWatch.Stop();
                    //Console.WriteLine("Miliseconds: {0}\n", stopWatch.ElapsedMilliseconds);
                    //stopWatch.Reset();
                }

                s.Close();
                myList.Stop();

                byte[] result = new byte[size];
                
                for (int i = 0; i < picture.Count; i++)
                {
                    for (int j = 0; j < 1024; j++)
                    {
                        if ((i * 1024) + j < size)
                        {
                            result[(i * 1024) + j] = picture[i][j];
                        }
                    }
                }

                var bmp = byteArrayToImage(result);
                bmp.Save("somefile.bmp");
                Console.WriteLine(picture.Sum(t => t.Count()));

            

            Console.ReadKey();
        }

        public static Bitmap byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return new Bitmap(returnImage);
        }
    }
}
