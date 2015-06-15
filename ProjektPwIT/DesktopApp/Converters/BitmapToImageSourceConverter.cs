using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopApp.Converters
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            MemoryStream stream = new MemoryStream((byte[])value);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    ((System.Drawing.Bitmap)value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //    BitmapImage image = new BitmapImage();
            //    image.BeginInit();
            //    ms.Seek(0, SeekOrigin.Begin);
            //    image.StreamSource = new MemoryStream(ms.ToArray());
            //    image.EndInit();

            //    return image;
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
