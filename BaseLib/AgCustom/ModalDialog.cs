using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace myzy.AgCustom
{
    public partial class ModalDialog : Control
    {
        static ModalDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalDialog),
                new FrameworkPropertyMetadata(typeof(ModalDialog)));
        }

        private bool _hideRequest = false;
        private bool _result = false;
        private UIElement _parent;

        public void SetParent(UIElement parent)
        {
            _parent = parent;
        }

        #region Message

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message", typeof(string), typeof(ModalDialog),
                new UIPropertyMetadata(string.Empty));

        #endregion

        public bool ShowHandlerDialog(string message)
        {
            Message = message;
            Visibility = Visibility.Visible;

            _parent.IsEnabled = false;

            _hideRequest = false;
            while (!_hideRequest)
            {
                // HACK: Stop the thread if the application is about to close
                if (this.Dispatcher.HasShutdownStarted ||
                    this.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                // HACK: Simulate "DoEvents"
                this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }

            return _result;
        }

        private void HideHandlerDialog()
        {
            _hideRequest = true;
            Visibility = Visibility.Hidden;
            _parent.IsEnabled = true;
        }

        public ICommand OkBtnCmd
        {
            get
            {
                return _okBtnCmd ?? (_okBtnCmd = new RelayCommand(o =>
                {
                    _result = true;
                    HideHandlerDialog();
                }));
            }
        }

        private ICommand _okBtnCmd;

        public ICommand CancelBtnCmd
        {
            get
            {
                return _cancelBtnCmd ?? (_cancelBtnCmd = new RelayCommand(o =>
                {
                    _result = false;
                    HideHandlerDialog();
                }));
            }
        }

        private ICommand _cancelBtnCmd;
    }
}


