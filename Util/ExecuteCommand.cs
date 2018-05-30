using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace myzy.Util
{
    public interface IExeCommanLine
    {
        bool Execulte();
        StringBuilder ReadData();
        StringBuilder ReadErrData();
        void Abort();
        int ExitCode { get; }
        bool IsCanceled { get; }
    }

    public class ExecuteCommand : IExeCommanLine
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");

        /// <summary>
        /// Exit Code
        /// </summary>
        public int ExitCode
        {
            get { return _exitCode; }
        }

        /// <summary>
        /// IsAborted
        /// </summary>
        public bool IsCanceled
        {
            get { return _isCanceled; }
        }

        public static void KillProgressByExeName(string exe)
        {
            var fn = Path.GetFileNameWithoutExtension(exe);
            try
            {
                var pros = Process.GetProcesses();
                for (var i = 0; i < pros.Length; i++)
                {
                    if (pros[i].ProcessName == fn)
                    {
                        _log.Debug($"Kill Progress {fn} --> {pros[i].Id}");
                        pros[i].Kill();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ExecuteCommand(string exe, string commandParameter, int maxTimeout = 5 * 1000)
        {
            _exePath = exe;
            _commandParameter = commandParameter;
            _maxTimeout = maxTimeout;
        }

        private readonly string _exePath = "cmd.exe";
        private readonly string _commandParameter;
        private readonly int _maxTimeout = 5 * 1000;

        private Process _process;

        /// <summary>
        /// Err Msg
        /// </summary>
        /// <returns></returns>
        public StringBuilder ReadErrData()
        {
            return _stringErrBuilder;
        }

        private void CreateProcess()
        {
            _slim.EnterWriteLock();
            try
            {
                var path = Path.GetDirectoryName(_exePath);
                if (path == null)
                    path = Environment.CurrentDirectory;

                _process = new Process
                {
                    StartInfo =
                    {
                        FileName = _exePath,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        //WindowStyle = ProcessWindowStyle.Normal,
                        //CreateNoWindow = false,
                        WorkingDirectory = path,
                    },
                    EnableRaisingEvents = true,
                };

                _process.Exited += (sender, args) =>
                {
                    if (sender == _process)
                    {
                        _exitCode = _process.ExitCode;
                        _process.OutputDataReceived -= _process_OutputDataReceived;
                        _process.ErrorDataReceived -= _process_ErrorDataReceived;
                        _isSuccess = true;
                        _process = null;
                        _resetEvent.Set();
                    }
                };
                _process.OutputDataReceived += _process_OutputDataReceived;
                _process.ErrorDataReceived += _process_ErrorDataReceived;
                _log.Debug($"Create Process {_exePath}");
            }
            catch (Exception e)
            {
                _log.Debug($"Create Process exception. {e.Message}");
            }
            _slim.ExitWriteLock();
        }

        private int _exitCode = 0;
        private bool _isProcessing = false;
        private bool _isCanceled = false;

        private readonly ReaderWriterLockSlim _slim = new ReaderWriterLockSlim();

        public void Abort()
        {
            _slim.EnterWriteLock();

            _isCanceled = true;
            try
            {
                if (_isProcessing && _process != null && !_process.HasExited)
                {
                    _log.Debug($"Send Kill To Process {_process.Id}, IsProcessing = {_isProcessing}");
                    _process?.Kill();
                    _resetEvent.Set();
                }
            }
            catch (Exception e)
            {
               // _log.Debug($"Abort Msg = {e.Message}");
            }

            _slim.ExitWriteLock();
        }

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_process == sender && !_isCanceled && _isProcessing)
            {
                _stringErrBuilder.AppendLine(e.Data);
            }
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (sender == _process && !_isCanceled && _isProcessing)
            {
                _stringBuilder.AppendLine(e.Data);
            }
        }

        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(true);
        private bool _isSuccess = false;
        private StringBuilder _stringBuilder = new StringBuilder();
        private StringBuilder _stringErrBuilder = new StringBuilder();

        /// <summary>
        /// Normal Read Data.
        /// </summary>
        /// <returns></returns>
        public StringBuilder ReadData()
        {
            return _stringBuilder;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns></returns>
        public bool Execulte()
        {
            _isSuccess = false;
            _log.DebugFormat("Send Cmd {0} : TimeOut = {1}", _commandParameter, _maxTimeout);
            _resetEvent.Reset();
            _stringBuilder = new StringBuilder();
            _stringErrBuilder = new StringBuilder();
            _isProcessing = true;
            if (!_isCanceled)
            {
                CreateProcess();
                if (_process != null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _log.Debug("Create WAITING THREAD COMPLETE");
                            Debug.Assert(_process != null);
                            _process.StartInfo.Arguments = _commandParameter; //+ "/r/n exit";
                            _process.Start();
                            _process.BeginOutputReadLine();
                            _process.BeginErrorReadLine();
                            _process.WaitForExit();
                            _log.Debug("WAITING THREAD FINISH");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            _log.Debug($"CMD Port Except: {e.Message}");
                        }
                    });
                    if (_resetEvent.WaitOne(_maxTimeout))
                    {
                        _isSuccess = true;
                    }
                }
            }
            _isProcessing = false;
            return _isSuccess;
        }
    }
}