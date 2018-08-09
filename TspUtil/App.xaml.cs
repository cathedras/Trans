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
using log4net;
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
    }

    public class TspStartUp
    {
        [STAThread]
        public static void Main(string[] args)
        {

            try
            {
                var wrap = new SingleInstanceApplicationWrapper();
                var wnd = new MainWindow();
                wrap.SetMainWnd(wnd);

                wrap.OnPreLoadEvent += () =>
                {
                    ExceptionHandler.AddHandler(false, false, false);
                    NativeDllHelper.PreLoadNativeDlls();

                    //var login = new LoginWnd();
                    //login.ShowDialog();
                };
                wrap.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
