using System;
using System.IO;
using System.Text;
using System.Threading;

namespace myzy.Util
{
    public class CsvSummary
    {
        private string _curFileName;

        private readonly Encoding _encoding;

        public CsvSummary(Encoding encoding)
        {
            _encoding = encoding;
            SubDirOp = new SubDirOp();
            FileBakOp = new FileBakOp();
            BaseDir = @".\";
            _dir = BaseDir;
            FileName = "tst.csv";
            _curFileName = FileName;
        }

        public SubDirOp SubDirOp { get; set; }

        public FileBakOp FileBakOp { get; set; }

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

        public string BaseDir
        {
            get { return _baseDir; }
            set { _baseDir = value; }
        }

        public virtual void InitHeraders(CsvFileWriter csv, object resultInfo)
        {
        }

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();
        private string _dir;
        private string _fn;
        private string _baseDir = "./";

        public void AppendResultInfo(object resultInfo)
        {
            _lockSlim.EnterWriteLock();

            try
            {
                _dir = SubDirOp.NextDir(_baseDir, _dir);

                var cur = Path.Combine(_dir, _fn);

                Action<CsvFileWriter, object> act = null;

                if (FileBakOp.NextFile(new FileInfo(cur)))
                {
                    _curFileName = FileBakOp.NextFileName;
                    act = InitHeraders;
                }

                using (var fs = File.Open(_curFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.Seek(0, SeekOrigin.End);
                    using (var csv = new CsvFileWriter(fs, _encoding))
                    {
                        var csr = ResultInfoToCsvRow(resultInfo);
                        if (csr != null)
                        {
                            act?.Invoke(csv, resultInfo);
                            csv.WriteRow(csr);
                            csv.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //ignore
            }

            _lockSlim.ExitWriteLock();
        }

        public virtual CsvRow ResultInfoToCsvRow(object resultInfo)
        {
            return null;
        }
    }
}