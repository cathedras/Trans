using ElCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace TspUtil
{
    /// <summary>
    /// this is a interface for one client param.
    /// </summary>
    public class SerialDev : IDev
    {
        private ManualResetEvent _resetevent;
        private List<byte> _header;
        private List<byte> _finalList;
        private int _sendLngth;
        private CancellationTokenSource _cancelToken;
        private long _imgLenCount;
        private Pcomm _pcom;

        public static byte[] StartBytes
        {
            get { return new byte[] {0x7f, 0xf7}; }
        }

        public static byte[] EndBytes
        {
            get { return new byte[] {0x81, 0x7e}; }
        }

        public static byte receiveOk = 0x79;
        private Pcomm _pcomm;
        private readonly IViewModel _vm;

        public SerialDev(Pcomm pcom, IViewModel vm)
        {
            _pcomm = pcom;
            _vm = vm;
            _cancelToken = new CancellationTokenSource();
        }

        public TcpClient Sock
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ManualResetEvent ResetEvent
        {
            get => _resetevent ?? (_resetevent = new ManualResetEvent(false));
        }
        public List<byte> HeaderList
        {
            get => _header ?? (_header = new List<byte>());
            set
            {
                _header = value;
            }
        }
        public List<byte> FinalList
        {
            get { return _finalList; }
            set
            {
                _finalList = value;
            }
        }
        public int SendLength
        {
            get { return _sendLngth; }
            set
            {
                _sendLngth = value;
            }
        }

        public CancellationTokenSource CancelToken
        {
            get => _cancelToken;
            //set
            //{
            //    if (_cancelToken == value) return;
            //    _cancelToken = value;
            //}
        }
        public bool IsReceiveNg { get; set; }

        public long LmgLenCount
        {
            get => _imgLenCount;
            set
            {
                _imgLenCount = value;
            }
        }

        public bool DataSendFrame(byte[] sendlst, int offset, int sendTimeOut = 20000)
        {
            if (_pcomm.sio_write(sendlst) != 0)
            {
                LmgLenCount += sendlst.Length;
                return true;
            }
            return false;
        }

        public List<byte> Receive()
        {
            byte[] buff = new byte[256];
            List<byte> lst = new List<byte>();
            int reclen = _pcomm.sio_read(ref buff, buff.Length);
           
            if (reclen > 0)
            {
                lst.AddRange(buff.Take(reclen));
            }
            return lst;
        }

    }
    /// <summary>
    /// network using in this class if something need to be sent you think
    /// </summary>
    public class NetDev : IDev
    {
        private TcpClient _sock;
        private readonly IViewModel _vm;
        private ManualResetEvent _resetevent;
        private List<byte> _header;
        private List<byte> _finalList;
        private int _sendLngth;
        private CancellationTokenSource _cancelToken;
        private bool _isReceiveNg;
        private long _imgLenCount;
        public static Regex picok = new Regex("picok", RegexOptions.IgnoreCase);
        public static Regex picNG = new Regex("picNG", RegexOptions.IgnoreCase);
        public NetDev(TcpClient sock, IViewModel vm)
        {
            _sock = sock;
            _vm = vm;
            _cancelToken = new CancellationTokenSource();
            _resetevent = new ManualResetEvent(false);
        }
        
        public ManualResetEvent ResetEvent
        {
            get => _resetevent;
        }
        public List<byte> HeaderList
        {
            get => _header ?? (_header = new List<byte>());
            set
            {
                _header = value;
            }
        }
        public List<byte> FinalList
        {
            get { return _finalList; }
            set
            {
                _finalList = value;
            }
        }
        public int SendLength
        {
            get { return _sendLngth; }
            set
            {
                _sendLngth = value;
            }
        }

        public CancellationTokenSource CancelToken
        {
            get => _cancelToken;
        }
        public bool IsReceiveNg
        {
            get => _isReceiveNg;
            set
            {
                _isReceiveNg = value;
            }
        }

        public long LmgLenCount
        {
            get => _imgLenCount;
            set
            {
                _imgLenCount = value;
            }
        }

        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private SemaphoreSlim _slim = new SemaphoreSlim(1);

        private int _maxRetryCount = 1;
        public bool DataSendFrame(byte[] buffer, int offset, int sendTimeOut = 20 * 1000)
        {
            _vm.AddLogMsg("Send Command ---> ");
            bool isFrameSendSuccess = false;
            _slim.Wait(Timeout.Infinite);
            try
            {
                var count = 1;

                while (count <= _maxRetryCount && !isFrameSendSuccess)
                {
                    _resetevent.Reset();
                    IsReceiveNg = false;

                    _sock.GetStream().Write(buffer, 0, buffer.Length);

                    if (!_resetevent.WaitOne(sendTimeOut))
                    {
                        break;
                    }

                    isFrameSendSuccess = true;
                    LmgLenCount += buffer.Length;
                }
            }
            catch (Exception x)
            {
                _log.Debug(x.Message);
            }

            _slim.Release();

            return isFrameSendSuccess;
        }


        public List<byte> Receive()
        {
            List<byte> lst = new List<byte>();
            byte[] buf = new byte[512];
            int reclen = _sock.Client.Receive(buf);
            if (reclen > 0)
            {
                byte[] temp = new byte[reclen];
                Array.Copy(buf, 0, temp, 0, reclen);
                lst.AddRange(temp);
            }
            return lst;
        }

    }
}
