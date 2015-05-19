using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleTestApp
{
    class Program
    {
        static System.Timers.Timer myTimer = new System.Timers.Timer();

        static void Main(string[] args)
        {
            myTimer.Interval = 1000;
            myTimer.Elapsed += someAsync;
            myTimer.Start();
            Console.WriteLine(DateTime.Now);

            Console.ReadLine();
        }

        private static bool isBusy = false;
        private static async void someAsync(object sender, EventArgs e)
        {
            if (!isBusy)
            {
                isBusy = true;
                await someTaskAsync();
                Console.WriteLine("tick");
                isBusy = false;
            }
        }

        private static Task someTaskAsync()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(2 * 1000);// 1000ms = 1s
            });
        }
    }
}
