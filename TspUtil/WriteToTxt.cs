using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspUtil
{
    static class WriteToTxt
    {
        public static void WriteOut<T>(T[] by,string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate|FileMode.Append, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(fs);
            foreach(T data in by)
            {
                writer.Write(data);
            }
            writer.Close();
            fs.Close();
        }
        public static void WriteOut(string str,string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs);  
            writer.Write(str);
            writer.Close();
            fs.Close();
        }
    }
}
