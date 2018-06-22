using System;
using System.IO;
using System.Text;
using System.Threading;

namespace myzy.Util
{
    public class TxtSimpLog
    {
        private string _baseDir;
        private readonly Encoding _encoding;

        public SubDirOp SubDirOp { get; set; }
        public FileBakOp FileBakOp { get; set; }

        public TxtSimpLog(Encoding encoding)
        {
            _encoding = encoding;
            _baseDir = @".\";
            SubDirOp = new SubDirOp();
            FileBakOp = new FileBakOp();
            _fn = "simplog.txt";
        }

        public void InitSimpLogDir(string dir)
        {
            _baseDir = dir;
        }

        public string FileName
        {
            get { return _fn; }
            set
            {
                _lockSlim.EnterWriteLock();
                _fn = value;
                _lockSlim.ExitWriteLock();
            }
        }

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();
        private string _dir = string.Empty;
        private string _fn;

        public void WriteSperator()
        {
            WriteLogFile(
                "-------------------------------------------------------------------------------------------------");
        }

        public void Rename(string newFileName)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                if (string.Compare(_fn, newFileName, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var old = Path.Combine(_dir, _fn);
                    var cur = Path.Combine(_dir, newFileName);
                    File.Move(old, cur);
                    FileName = newFileName;
                }
            }
            catch (Exception e)
            {
                //ignore
            }
            _lockSlim.ExitWriteLock();
        }

        public void WriteLogFile(string format)
        {
            _lockSlim.EnterWriteLock();

            try
            {
                _dir = SubDirOp.NextDir(_baseDir, _dir);
                var cur = Path.Combine(_dir, _fn);
                if (FileBakOp.NextFile(new FileInfo(cur)))
                {
                    cur = FileBakOp.NextFileName;
                }
                //using (var fs = finfo.Open(FileMode.Open, FileAccess.Write))
                using (var fs = File.Open(cur, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs, _encoding))
                    {
                        sw.BaseStream.Seek(0, SeekOrigin.End);
                        sw.Write($"{format}");
                        sw.WriteLine();
                        sw.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                //ignore
            }
            _lockSlim.ExitWriteLock();
        }
    }
}