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
}
