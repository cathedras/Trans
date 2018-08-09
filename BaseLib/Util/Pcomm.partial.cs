using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using log4net;

namespace myzy.Util
{
    public partial class Pcomm
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private byte[] CmdToBytes(string strCmd)
        {
            var bytes = Encoding.ASCII.GetBytes(strCmd);
            return bytes;
        }

        private string RecvString(long timeout)
        {
            var str = string.Empty;
            var sw = new Stopwatch();
            var buf = new List<byte>();

            sw.Start();

            while (sw.ElapsedMilliseconds < timeout)
            {
                var chr = new byte[1];
                var count = this.sio_read(ref chr, chr.Length);
                if (count > 0)
                {
                    buf.AddRange(chr);
                    var tmp = Encoding.ASCII.GetString(buf.ToArray(), 0, buf.Count);
                    if (buf[buf.Count - 1] == '\n')
                    {
                        str = tmp;
                        buf.Clear();
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
            sw.Stop();

            if (buf.Count > 0)
            {
                var dummy = Encoding.ASCII.GetString(buf.ToArray(), 0, buf.Count);
                //_log.Debug($"REV DUMMY STRING. {dummy}");
            }
            return str;
        }

        public CmdState SendCmdToMcu(string cmd)
        {
            sio_write(CmdToBytes(cmd));
            return CmdState.Success;
        }

        private readonly int _innerTimeout = 1000;

        private readonly ConcurrentQueue<string> _revQueue = new ConcurrentQueue<string>();

        public CmdState SendCmdToMcuAndRead(bool syncInnerMsg, string cmd, ref string data, ref List<string> innerText, Func<string, bool> chk = null, int timeout = 20000)
        {
            CmdState cmdState = CmdState.Fail;
            _slimForSerial.WaitOne(Timeout.Infinite);
            data = string.Empty;
            innerText.Clear();
            while (syncInnerMsg && _revQueue.TryDequeue(out data)) { }
            data = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    Console.WriteLine($"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++{cmd}");
                    SendCmdToMcu(cmd);
                }
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < timeout)
                {
                    var str = RecvString(_innerTimeout);
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (chk == null || chk(str))
                        {
                            data = str;
                            break;
                        }
                        else
                        {
                            innerText?.Add(str);
                            if (syncInnerMsg)
                                _revQueue.Enqueue(str);
                        }
                    }
                }
                sw.Stop();

                while (syncInnerMsg && !_revQueue.IsEmpty)
                {
                    Thread.Sleep(1);
                }

                if (!string.IsNullOrEmpty(data))
                {
                    cmdState = CmdState.Success;
                }
            }
            catch (Exception e)
            {
                _log.Error($"Exception -> {e.Message}");
            }
            _slimForSerial.Set();

            return cmdState;
        }

        private readonly AutoResetEvent _slimForSerial = new AutoResetEvent(true);
    }

    public enum CmdState
    {
        Fail,
        Success,
    }
}