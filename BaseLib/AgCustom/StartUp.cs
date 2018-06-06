using System;
using System.Windows;
using Microsoft.VisualBasic.ApplicationServices;

namespace myzy.AgCustom
{
    /*
    public class StartUp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var wrap = new SingleInstanceApplicationWrapper();
                wrap.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public partial class App : Application
    {
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
    */

    public class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase
    {
        private Application _app;

        public SingleInstanceApplicationWrapper(Application app)
        {
            this.IsSingleInstance = true;
            _app = app;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            _app.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            _app.MainWindow?.Activate();
        }
    }
}