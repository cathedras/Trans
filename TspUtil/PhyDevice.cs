﻿using ElCommon.Util;
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

        public bool DataSendFrame(byte[] sendlst, int offset, int sendTimeOut = 500)
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

        public bool Connected()
        {
            return false;
        }

        public bool Disconnect()
        {
            return false;
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
        public NetDev()
        {
            Sock.SendBufferSize = 1024 * 2;
            Sock.ReceiveBufferSize = 1024 * 2;
            Sock.NoDelay = false;

          
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

        public TcpClient Sock
        {
            get { return _sock; }
            set { _sock = value; }
        }

       

        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private SemaphoreSlim _slim = new SemaphoreSlim(1);

        private int _maxRetryCount = 1;
        public bool DataSendFrame(byte[] buffer, int offset, int sendTimeOut = 500)
        {
            //_vm.AddLogMsg("Send Command ---> ");
            byte[] sampleBytes = null;
            if (buffer.Length > 5)
            {
                sampleBytes = buffer.Take(5).ToArray();
            }
            else
            {
                sampleBytes = buffer.Take(buffer.Length).ToArray();
            }
            //var msg  = string.Join(" ", Array.ConvertAll(sampleBytes, input => $"{input:X2}"));
            //_log.Debug($"--> Len: {buffer.Length:D5}  Raw: {msg} ....");

            bool isFrameSendSuccess = false;
            _slim.Wait(Timeout.Infinite);
            try
            {
                var count = 1;

                while (count <= _maxRetryCount && !isFrameSendSuccess)
                {
                    _resetevent.Reset();
                    IsReceiveNg = false;

                    Sock.GetStream().Write(buffer, 0, buffer.Length);
                    
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
                _log.Error(x.Message);
            }

            _slim.Release();

            return isFrameSendSuccess;
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

                _log.Debug($"<--- {string.Join($" ", Array.ConvertAll(temp, input=>$"{input:X2}"))}:{Encoding.ASCII.GetString(temp)}");
            }

            return lst;
        }

        public bool Connected()
        {
            return Sock.Connected;
        }

        public bool Disconnect()
        {
            return false;
        }
    }

    public class ClientNetDev : IDev
    {
        private TcpSocketEx _sock;
        private ManualResetEvent _resetevent;
        private List<byte> _header;
        private List<byte> _finalList;
        private int _sendLngth;
        private CancellationTokenSource _cancelToken;
        private bool _isReceiveNg;
        private long _imgLenCount;
        private Gbl _gbl;
        public static Regex picok = new Regex("pic ok", RegexOptions.IgnoreCase);
        public static Regex picNG = new Regex("pic NG", RegexOptions.IgnoreCase);
        public static Regex ddrOffOk = new Regex("ddroff ok", RegexOptions.IgnoreCase);
        public static Regex ddrOffNg = new Regex("ddroff NG", RegexOptions.IgnoreCase);
        public static Regex eraseOk = new Regex("erase ok", RegexOptions.IgnoreCase);
        public static Regex eraseNG = new Regex("erase NG", RegexOptions.IgnoreCase);
        public static Regex storeOk = new Regex("store ok", RegexOptions.IgnoreCase);
        public static Regex storeNG = new Regex("store NG", RegexOptions.IgnoreCase);
        public static Regex ddrstopOk = new Regex("ddrstop ok", RegexOptions.IgnoreCase);
        public static Regex ddrstopNG = new Regex("ddrstop NG", RegexOptions.IgnoreCase);
        public static Regex ddrOk = new Regex("ddr ok", RegexOptions.IgnoreCase);
        public static Regex ddrNG = new Regex("ddr NG", RegexOptions.IgnoreCase);
        public static Regex showok = new Regex("show ok", RegexOptions.IgnoreCase);
        public static Regex showng = new Regex("show ng", RegexOptions.IgnoreCase);
        public static Regex poweronok = new Regex("poweron ok", RegexOptions.IgnoreCase);
        public static Regex poweronng = new Regex("poweron ng", RegexOptions.IgnoreCase);
        public static Regex slavePowerOn = new Regex("P.DUT.PowerON", RegexOptions.IgnoreCase);
        public static Regex slavepowerOff = new Regex("P.DUT.PowerOff", RegexOptions.IgnoreCase);
        public static Regex slaveCheckSum = new Regex("p.checksum,0000", RegexOptions.IgnoreCase);
        public static Regex slaveShow = new Regex("ShowImage,0000", RegexOptions.IgnoreCase);
        public static Regex progmOk = new Regex(@"\{OK\d*", RegexOptions.IgnoreCase);
        public static Regex progmEr = new Regex(@"\{ER\d*", RegexOptions.IgnoreCase);
        public static Regex slaveWrite = new Regex("p.register.write,0000", RegexOptions.IgnoreCase);
        public static Regex slaveRead = new Regex("p.register.Read", RegexOptions.IgnoreCase);
        public static Regex deviceVersion = new Regex("p.version", RegexOptions.IgnoreCase);

        
        public void InitParam()
        {
            Sock.SendBufferSize =  1024 * 4;
            Sock.ReceiveBufferSize = 1024 * 4;
           // Sock.NoDelay = true;
            

            _cancelToken = new CancellationTokenSource();
            _resetevent = new ManualResetEvent(false);
        }

        public ManualResetEvent ResetEvent => _resetevent;

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

        public TcpSocketEx Sock { get => _sock; set => _sock = value; }

        public Gbl Gbl1
        {
            get { return _gbl; }
            set { _gbl = value; }
        }

        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private SemaphoreSlim _slim = new SemaphoreSlim(1);
        private int _maxRetryCount = 1;
        /// <summary>
        /// ClientSend
        /// </summary>
        /// <param name="sendlst"></param>
        /// <param name="offset"></param>
        /// <param name="sendTimeOut"></param>
        /// <returns></returns>
        public bool DataSendFrame(byte[] sendlst, int offset, int sendTimeOut = 500)
        {
            byte[] sampleBytes = null;
            if (sendlst.Length > 5)
            {
                sampleBytes = sendlst.Take(5).ToArray();
            }
            else
            {
                sampleBytes = sendlst.Take(sendlst.Length).ToArray();
            }

            if (_gbl.UsingLogInfo)
            {
                var msg = string.Join(" ", Array.ConvertAll(sampleBytes, input => $"{input:X2}"));
                _log.Debug($"--> Len: {sendlst.Length:D5}  Raw: {msg}:{ASCIIEncoding.ASCII.GetString(sendlst)} ....");
            }

            bool isFrameSendSuccess = false;
            _slim.Wait(Timeout.Infinite);
            try
            {
                var count = 1;

                while (count <= _maxRetryCount && !isFrameSendSuccess)
                {
                    _resetevent.Reset();
                    IsReceiveNg = false;

                    //_sock.GetStream().Write(buffer, 0, buffer.Length);
                    //_sock.Send(sendlst, 0, sendlst.Length, SocketFlags.MaxIOVectorLength, out SocketError code);
                    Sock.Send(sendlst);
                    
                    if (!_resetevent.WaitOne(sendTimeOut))
                    {
                        break;
                    }

                    isFrameSendSuccess = true;
                    LmgLenCount += sendlst.Length;
                }
            }
            catch (Exception x)
            {
                _log.Error(x.Message);
            }

            _slim.Release();

            return isFrameSendSuccess;

        }

        public List<byte> Receive()
        {
            List<byte> lst = new List<byte>();

            byte[] buf = new byte[512];


            int reclen = Sock.Receive(buf, SocketFlags.None);
            if (reclen > 0)
            {
                byte[] temp = new byte[reclen];
                Array.Copy(buf, 0, temp, 0, reclen);
                lst.AddRange(temp);
                //Console.WriteLine($"{string.Join($" ", Array.ConvertAll(temp, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(temp)}");
                if (_gbl.UsingLogInfo)
                {
                    _log.Debug(
                        $"<--- {string.Join($" ", Array.ConvertAll(temp, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(temp)}");
                }
            }

            return lst;
        }

        public bool Connected()
        {
            var status = _sock.IsSocketConnected();
           // var sendOk = DataSendFrame(App.Locator.Main.StringToByteArray("version"), 0,2000);
            return (_sock.ReceiveTimeout < 0)||status;
        }

        public bool Disconnect()
        {
            _sock.Disconnect(false);
            if (!_sock.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
