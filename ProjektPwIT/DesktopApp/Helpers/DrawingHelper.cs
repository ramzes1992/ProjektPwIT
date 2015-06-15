using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Helpers
{
    public static class DrawingHelper
    {
        private static byte[] _data = File.ReadAllBytes("test.png");
        private static object _sync = new object();

        public static void SetData(byte[] data)
        {
            lock(_sync){
                _data = data;
            }
        }

        public static byte[] GetData()
        {
            lock (_sync)
            {
                return _data;
            }
        }
    }
}
