using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Drawing;

namespace DesktopApp.Services
{
    public class WebCamService
    {
        private Capture _capture;

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

        public WebCamService()
        {
            _capture = new Capture();
            _capture.ImageGrabbed += _capture_ImageGrabbed;
        }

        void _capture_ImageGrabbed(object sender, EventArgs e)
        {
            RaiseImageChangedEvent(_capture.RetrieveBgrFrame().Bitmap);
        }

        public void RunServiceAsync()
        {
            _capture.Start();
            _isRunning = true;
        }

        public void CancelServiceAsync()
        {
            _capture.Stop();
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
