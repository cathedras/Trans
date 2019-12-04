using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TspUtil
{
    public class ConsoleDebug
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean AttachConsole(IntPtr ptr);

        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        public static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }
        public static void Hide()
        {
            SetOutAndErrorNull();
            FreeConsole();
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        static void InvalidateOutAndError()
        {
            Type typ = typeof(System.Console);
            System.Reflection.FieldInfo _out = typ.GetField("_out", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.FieldInfo _error = typ.GetField("_error", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.MethodInfo _init = typ.GetMethod("InitializeStdOutError", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Debug.Assert(_out != null);
            Debug.Assert(_error != null);
            Debug.Assert(_init != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);
            _init.Invoke(null, new object[] { true});
        }

        public static void WriteLine(string format,params object[] args)
        {
            WriteLine(string.Format(format,args));
        }

        public static void WriteLine(string output, ConsoleColor level)
        {
            Console.ForegroundColor = level;
            Console.WriteLine(@"[{0}]{1}", DateTime.Now, output);
        }
    }
    class CmdExr
    {
        public static string Execute(string dosCommand)
        {
            return Execute(dosCommand, 10);
        }
        /// <summary>
        /// 执行DOS命令，返回DOS命令的输出
        /// </summary>
        /// <param name="dosCommand">dos命令</param>
        /// <param name="milliseconds">等待命令执行的时间（单位：毫秒），
        /// 如果设定为0，则无限等待</param>
        /// <returns>返回DOS命令的输出</returns>
        public static string Execute(string command, int seconds)
        {
            string output = ""; //输出字符串
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;//不重定向输入
                startInfo.RedirectStandardOutput = true; //重定向输出
                startInfo.CreateNoWindow = true;//不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }
    }


}
