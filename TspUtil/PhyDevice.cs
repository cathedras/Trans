using myzy.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TspUtil
{
    /// <summary>
    /// this is a interface for one client param.
    /// </summary>
    public class Serial : IDev
    {
        private ManualResetEvent _resetevent;
        private List<byte> _header;
        private List<byte> _finalList;
        private int _sendLngth;
        private CancellationTokenSource _cancelToken;
        private long _imgLenCount;
        private Pcomm _pcom;
        public static byte[] start = new byte[] { 0x7f, 0xf7 };
        public static byte[] end = new byte[] { 0x81, 0x7e };

        public static byte receiveOk = 0x79;
        public Serial(Pcomm pcom)
        {
            Pcom = pcom;
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
        public bool IsReceiveNg
        {
            get => throw new NotImplementedException();
            set
            {
                throw new NotImplementedException();
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

        public Pcomm Pcom
        {
            get => _pcom;
            set
            {
                _pcom = value;
            }
        }
        public List<byte> Receive()
        {
            byte[] buff = new byte[256];
            List<byte> lst = new List<byte>();
            int reclen = Pcom.sio_read(ref buff, buff.Length);
           
            if (reclen > 0)
            {
                lst.AddRange(buff.Take(reclen));
            }
            return lst;
        }

        public bool Send(byte[] sendlst, int offset = 0)
        {
            if (Pcom.sio_write(sendlst) != 0)
            {
                return true;
            }
            return false;
        }

       
    }
    /// <summary>
    /// network using in this class if something need to be sent you think
    /// </summary>
    public class NetClient : IDev
    {
        private TcpClient _sock;
        private ManualResetEvent _resetevent;
        private List<byte> _header;
        private List<byte> _finalList;
        private int _sendLngth;
        private CancellationTokenSource _cancelToken;
        private bool _isReceiveNg;
        private long _imgLenCount;
        public static string picok = "picok";
        public static string picNG = "picNG";
        public NetClient(TcpClient sock)
        {
            Sock = sock;
            _cancelToken = new CancellationTokenSource();
        }

        public TcpClient Sock
        {
            get { return _sock; }
            set
            {
                _sock = value;
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
            //    CancelToken = value;
            //}
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

        public Pcomm Pcom
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool Send(byte[] sendlst, int offset = 0)
        {
            Sock?.GetStream().Write(sendlst, offset, sendlst.Length);
            return true;
        }


        public List<byte> Receive()
        {
            List<byte> lst = new List<byte>();
            byte[] buf = new byte[512];
            int reclen = Sock.Client.Receive(buf);
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
