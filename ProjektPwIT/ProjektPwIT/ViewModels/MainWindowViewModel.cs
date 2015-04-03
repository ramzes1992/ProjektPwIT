using ProjektPwIT.Helpers;
using ProjektPwIT.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjektPwIT.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private WebCamService _webCamService;

        private Bitmap _frame;
        public Bitmap Frame
        {
            get
            {
                if (_frame != null)
                {
                    return _frame;
                }
                else
                {
                    return new Bitmap(640, 480);
                }
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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            InitializeServices();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _webCamService = new WebCamService();
            _webCamService.ImageChanged += _webCamService_ImageChanged;
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
            if (!_webCamService.IsRunning)
            {
                _webCamService.RunServiceAsync();
            }
            else
            {
                _webCamService.CancelServiceAsync();
            }
        }
    }
}
