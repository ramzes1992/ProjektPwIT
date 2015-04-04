using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjektPwIT.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool _isRunning = false;

        private void v_Button_ToggleWebCam_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;

            v_Button_ToggleWebCam.Content = _isRunning ? "Stop" : "Start";
            v_InkCanvas.EditingMode = _isRunning ? InkCanvasEditingMode.Ink : InkCanvasEditingMode.None;
        }

        private void v_InkCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isRunning)
            {
                ExportToPng("test.bmp", v_InkCanvas);
            }
        }

        private void ExportToPng(string path, InkCanvas surface)
        {
            if (path == null) return;

            Size size = new Size(surface.Width, surface.Height);

            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
        }
    }
}
