using DesktopApp.Helpers;
using DesktopApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private WebCamService _webCamService;
        private TCPListenerService _TCPListenerService;

        private Bitmap _frame;
        public Bitmap Frame
        {
            get
            {
                return _frame;
            }

            set
            {
                if (_frame != value)
                {
                    _frame = value;
                    RaisePropertyChanged(() => Frame);
                }
            }
        }

        private ICommand _toggleWebServiceCommand;
        public ICommand ToggleWebServiceCommand
        {
            get
            {
                return _toggleWebServiceCommand;
            }

            private set { }
        }

        public MainWindowViewModel()
        {
            InitializeServices();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _webCamService = new WebCamService();
            _webCamService.ImageChanged += _webCamService_ImageChanged;

            _TCPListenerService = new TCPListenerService("192.168.0.103", 6666);
            _TCPListenerService.ImageChanged += _webCamService_ImageChanged;
        }

        private void InitializeCommands()
        {
            _toggleWebServiceCommand = new DelegateCommand(ToggleWebServiceExecute);
        }

        private void _webCamService_ImageChanged(object sender, System.Drawing.Bitmap image)
        {
            this.Frame = image;
        }

        private void ToggleWebServiceExecute()
        {
            //if (!_webCamService.IsRunning)
            //{
            //    _webCamService.RunServiceAsync();
            //}
            //else
            //{
            //    _webCamService.CancelServiceAsync();
            //}

            if (!_TCPListenerService.IsRunning)
            {
                _TCPListenerService.RunServiceAsync();
            }
            else
            {
                _TCPListenerService.CancelServiceAsync();
            }
        }
    }
}
