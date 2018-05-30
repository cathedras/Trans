using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using log4net;

namespace myzy.Util
{
    /// <summary>
    /// 非托管程序集加载帮助类
    /// </summary>
    public class NativeDllHelper
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private static readonly List<IntPtr> HInptrs = new List<IntPtr>();

        [DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern IntPtr LoadLibrary(string fileName);

        public static void PreLoadNativeDlls()
        {
            var dataLayers = (List<NativeDllInfo>)
                ConfigurationManager.GetSection("NativeDlls");

            dataLayers.ToList().ForEach(p =>
            {
                var fil = System.Environment.Is64BitProcess
                        ? @"x64\" + p.Path : @"x86\" + p.Path;
                _log.DebugFormat($"Try To Load Dynamic Link Library: Name={p.Name} Path={fil} IsCopy={p.CopyOrLoad}");

                if (File.Exists(fil))
                {
                    try
                    {
                        if (p.CopyOrLoad)
                        {
                            File.Copy(fil, p.Path, true);
                            Console.WriteLine($"Copy Dynamic Link Library: {fil} --> {p.Path}");
                        }
                        else
                        {
                            HInptrs.Add(LoadLibrary(fil));
                            Console.WriteLine("Load Dynamic Link Library: {0}", fil);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to Load Dynamic Link Library: {0}", fil);
                }
            });
        }
    }


}