using System;
using System.IO;

namespace myzy.Util
{
    /// <summary>
    /// 子文件夹生成测试
    /// </summary>
    public class SubDirOp
    {
        public virtual string NextDir(string baseDir, string curDir)
        {
            return baseDir;
        }
    }

    /// <summary>
    /// 按日期组织文件夹
    /// </summary>
    public class SubDirOpWithDate : SubDirOp
    {
        public override string NextDir(string baseDir, string curDir)
        {
            var dir = Path.Combine(baseDir, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }

    /// <summary>
    /// 文件备份测试
    /// </summary>
    public class FileBakOp
    {
        public string NextFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fn"></param>
        /// <returns>是否新建文件</returns>
        public virtual bool NextFile(FileInfo fn)
        {
            NextFileName = fn.FullName;

            if (!File.Exists(NextFileName))
                return true;

            return false;
        }
    }

    /// <summary>
    /// 文件过大时移动备份
    /// </summary>
    public class FileBakOpWithSize : FileBakOp
    {
        private readonly int _maxSize;
        private readonly int _fileBaseIdx;

        public FileBakOpWithSize(int maxSize, int fileBaseIdx = 0)
        {
            _maxSize = maxSize;
            _fileBaseIdx = fileBaseIdx;
        }

        public override bool NextFile(FileInfo finfo)
        {
            if (!finfo.Exists)
                return base.NextFile(finfo);

            if (finfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(finfo.FullName);
            }

            var ext = finfo.Extension;
            var fn1 = Path.GetFileNameWithoutExtension(finfo.FullName);

            bool res = false;
            if (finfo.Length >= _maxSize)
            {
                int idx = _fileBaseIdx;
                while(true)
                {
                    string strBak = $"{fn1}_{idx}{ext}";
                    var bak = Path.Combine(finfo.DirectoryName, strBak);
                    if (!File.Exists(bak))
                    {
                        try
                        {
                            File.Move(finfo.FullName, bak);
                            res = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    }
                    idx++;
                }
            }
            return res;
        }
    }
}