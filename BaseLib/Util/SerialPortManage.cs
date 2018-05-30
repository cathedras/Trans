using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

namespace myzy.Util
{
    public class SerialPortManage : List<SerialPortXParam>
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");
        private readonly Regex _serialPortNameRegex = new Regex(@"COM\d+");
        private readonly int _readTimeout = 500;
        private readonly int _readBuffLength = 1024;

        public bool DoInitialize(bool isSerial = false)
        {
            var availComPorts = SerialPort.GetPortNames().ToList();
            var tsks = new List<Task>();

            //发送命令测试联通状态
            Func<Pcomm, Hashtable, byte[], Func<byte[], bool>, bool> comQuery = (serial, hasht, cmd, chk) =>
            {
                var resCode = serial.InitComm(hasht);
                var success = false;
                if (resCode >= 0)
                {
                    var bys = cmd;
                    serial.sio_write(bys);
                    var sw = new Stopwatch();
                    sw.Start();
                    var lstBuf = new List<byte>();
                    while (sw.ElapsedMilliseconds < _readTimeout)
                    {
                        var buf = new byte[_readBuffLength];
                        var len = serial.sio_read(ref buf, buf.Length);
                        if (len > 0)
                        {
                            lstBuf.AddRange(buf.Take(len));
                            if (chk(lstBuf.ToArray()))
                            {
                                success = true;
                                break;
                            }
                        }
                    }
                    sw.Stop();
                    serial.CloseComm();
                }
                return success;
            };

            for (int i = 0; i < this.Count; i++)
            {
                var cur = this[i];
                if (!cur.IsAutoSearch)
                    continue;
                if (!_serialPortNameRegex.IsMatch(cur.PreSetComName))
                    continue;
                if (!availComPorts.Contains(cur.PreSetComName))
                    continue;

                for (int j = 0; j < availComPorts.Count; j++)
                {
                    var com = availComPorts[j];

                    if (this.Any(p=>p.ComName == com))
                        continue;

                    if (isSerial)
                        Task.WaitAll(tsks.ToArray());

                    if (!string.IsNullOrEmpty(cur.ComName))
                        break;

                    tsks.Add(Task.Factory.StartNew(() =>
                    {
                        #region Query The COM Interface

                        var serial = new Pcomm();

                        var hasht = new Hashtable
                        {
                            {Pcomm.KEY_PORT, $"{com}"},
                            {Pcomm.KEY_BAUDRATE, $"{cur.Baud}"},
                            {Pcomm.KEY_BYTESIZE, $"{cur.DataBits}"},
                            {Pcomm.KEY_PARITY, $"{cur.Parity}"},
                            {Pcomm.KEY_STOPBITS, $"{(int) cur.StopBits}"}
                        };

                        if (comQuery(serial, hasht, cur.QueryString, cur.QueryRegex))
                        {
                            cur.ComName = com;
                        }
                        #endregion

                    }));
                }
                Task.WaitAll(tsks.ToArray());
            }

            Task.WaitAll(tsks.ToArray());
            if (this.Where(c=>c.IsAutoSearch).ToList().TrueForAll(p => !string.IsNullOrWhiteSpace(p.ComName)))
            {
                _log.Debug($"All {this.Count} Device  ARE PreSet Correctly.");
                return true;
            }
            return false;
        }
    }
}
