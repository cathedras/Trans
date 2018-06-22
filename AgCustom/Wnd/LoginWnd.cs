using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace myzy.AgCustom
{
    /// <summary>
    /// Interaction logic for LoginWnd.xaml
    /// </summary>
    public partial class LoginWnd : Window
    {
        static LoginWnd()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoginWnd), new FrameworkPropertyMetadata(typeof(LoginWnd)));
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(LoginWnd), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LoginWnd), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsShowUserNameProperty =
            DependencyProperty.Register("IsShowUserName", typeof(bool), typeof(LoginWnd), new PropertyMetadata(true));

        public Func<string, string, bool> OnLoginEvent;
        private ICommand _loginCmd;

        public LoginWnd()
        {
            Width = 400;
            Height = 200;
            this.Loaded += LoginWnd_Loaded;
        }

        private void LoginWnd_Loaded(object sender, RoutedEventArgs e)
        {
            var tb = VisualTreeHelperEx.FindDescendantByName(this, "tbUserName");
            var tb2 = VisualTreeHelperEx.FindDescendantByName(this, "tbPassword");

            if (tb is TextBox tbUserName )
            {
                if (tbUserName.IsEnabled)
                    tbUserName.Focus();
            }
            else if (tb2 is TextBox tbPassword)
            {
                if (tbPassword.IsEnabled)
                    tbPassword.Focus();
            }
        }
        

        public string UserName
        {
            get { return (string) GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public bool IsShowUserName
        {
            get { return (bool)GetValue(IsShowUserNameProperty); }
            set { SetValue(IsShowUserNameProperty, value); }
        }

        public ICommand LoginCmd
        {
            get
            {
                return _loginCmd ?? (_loginCmd = new RelayCommand(o =>
                {
                    bool needClose = true;
                    if (OnLoginEvent != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            needClose = OnLoginEvent.Invoke(UserName, Password);
                        });
                    }
                    if (needClose)
                    {
                        this.Close();
                    }
                }, o => !string.IsNullOrWhiteSpace(Password)));
            }
        }
    }
}
