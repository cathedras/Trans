using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Elcommon.AgLib;
using ErrorHandler;
using log4net;
using ElCommon.Util;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace TspUtil
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ViewModelLocator Locator =>
            (ViewModelLocator)Application.Current.FindResource("Locator");

        public App()
        {
            ExceptionHandler.AddHandler(false, false, false);
            NativeDllHelper.PreLoadNativeDlls();
            Application.Current.Resources.Add("Locator", new ViewModelLocator());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }

    public class TspStartUp
    {
        private static List<string> path = new List<string>();
        [STAThread]
        public static void Main(string[] args)
        {

            if (args.Any())
            {
                for(int i=1;i<args.Length; i++)
                {
                    path.Add(args[i]);
                }
                ParseCmd(args[0]);
            }
            //try
            //{
            //    var wrap = new SingleInstanceApplicationWrapper(new App());
            //    var wnd = new MainWindow();
            //    wrap.SetMainWnd(wnd);

            //    wrap.OnPreLoadEvent += () =>
            //    {
            //        ExceptionHandler.AddHandler(false, false, false);
            //        NativeDllHelper.PreLoadNativeDlls();

            //        //var login = new LoginWnd();
            //        //login.ShowDialog();
            //    };
            //    wrap.Run(args);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            ConsoleDebug.Hide();
            App app = new App();
            MainWindow window=new MainWindow();
            app.MainWindow = window;
            app.Run(new MainWindow());

        }

        public static string ParseCmd(string cmd)
        {
            switch (cmd)
            {
                case "-push":
                    if (path.Any())
                    {
                        SendToDevice(path.ToArray());
                    }
                    break;
                case "-pull":

                    break;
                   
            }
            return string.Empty;
        }


        public static void SendToDevice(string[] args)
        {
            if (args.Any())
            {
                var vm = new MianViewModel();
                vm.AddLogMsg("启动命令：TspUtil,version:1.0.2");
                vm.IsCmdRun = true;
                // vm.TestExecute(); 
                // Trace.WriteLine("hello");
                foreach (var item in args)
                {
                    var itms = item.Split('\\');
                    vm.ClearSendList();
                    vm.ImgItemInfos.Add(new ImgItemCmd()
                    {
                        FnPath = item,
                        Cs = "",
                        Des = itms.LastOrDefault(),
                        IsActived = true,
                    });
                }
                vm.SockConnect();
                vm.CmdNetWorkSend();
                vm.SockDisconnect();

                // ConsoleDebug.FreeConsole();
                Environment.Exit(0);

            }
        }



    }

}
