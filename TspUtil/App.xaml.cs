using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ErrorHandler;
using myzy.AgCustom;
using myzy.Util;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ExceptionHandler.AddHandler(false, false, false);
            NativeDllHelper.PreLoadNativeDlls();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //var login = new LoginWnd();
            //login.ShowDialog();
            //Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mainWnd = new MainWindow();
            Current.MainWindow = mainWnd;
            Current.MainWindow?.Show();

            base.OnStartup(e);
        }
    }

    public class TspStartUp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();
                var wrap = new SingleInstanceApplicationWrapper(app);
                wrap.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
