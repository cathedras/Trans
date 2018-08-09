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
    */

    public class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase
    {
        private Application _app;
        private Window _mainWindow;

        public SingleInstanceApplicationWrapper()
        {
            this.IsSingleInstance = true;
        }

        public void SetMainWnd(Window wnd)
        {
            _mainWindow = wnd;
        }

        public event Action OnPreLoadEvent;

        public event Func<bool> OnPreCheckEvent;

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            var preCheck = true;
            if (OnPreCheckEvent != null)
            {
                preCheck = OnPreCheckEvent.Invoke();
            }

            if (preCheck)
            {
                _app = new Application();
                OnPreLoadEvent?.Invoke();
                _app.Run(_mainWindow);
                return false;
            }
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            _mainWindow?.Activate();
        }
    }
}