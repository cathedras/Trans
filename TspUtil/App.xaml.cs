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
        private readonly static ILog _log = LogManager.GetLogger("exlog");
        private static List<string> path = new List<string>();
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Any())
            {
                path.AddRange(args);
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
            //ConsoleDebug.Hide();
            //App app = new App();
            //MainWindow window=new MainWindow();
            //app.MainWindow = window;
            //app.Run(new MainWindow());

        }

        public static string ParseCmd(string cmd)
        {
            switch (cmd)
            {
                case "-push":
                    SendToDevice(path.ToArray());
                    break;
                case "-pull":
                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// 须修改
        /// </summary>
        /// <param name="args"></param>
        public static void SendToDevice(string[] args)
        {
            if (args.Any())
            {
                MianViewModel.IsCmdRun = true;
                var vm = new MianViewModel();
                vm.AddLogMsg("启动命令：TspUtil,version:1.0.2");
                //vm.ClearSendList();
                for (int i=1;i<args.Length;i++)
                {
                    vm.AddLogMsg($"{args[i]}");
                    var itms = args[i].Split('\\');
                    vm.ImgItemInfos.Add(new ImgItemCmd()
                    {
                        FnPath = args[i],
                        Cs = "",
                        Des = itms.LastOrDefault(),
                        IsActived = true,
                        FileIndex = i,
                    });
                }
                vm.SockConnect();
                var isOk = vm.SendByCmd();
                vm.SockDisconnect();
                if (isOk)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Environment.Exit(-1);
                }
                
                // ConsoleDebug.FreeConsole();
              //  Environment.Exit(0);

            }
        }



    }

}
