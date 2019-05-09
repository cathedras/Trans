using ElCommon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TspUtil
{
    /// <summary>
    /// this class has no implementation in a time,
    /// </summary>
    class NetVirDevice : IDev
    {
        private readonly IViewModel _vm;
        private readonly ManualResetEvent _resetEvent;

        public ManualResetEvent ResetEvent => _resetEvent;
        public List<byte> HeaderList { get; set; }
        public List<byte> FinalList { get; set; }
        public int SendLength { get; set; }
        public CancellationTokenSource CancelToken { get; }
        public bool IsReceiveNg { get; set; }
        public long LmgLenCount { get; set; }

        public NetVirDevice(IViewModel vm)
        {
            _vm = vm;
            _resetEvent = new ManualResetEvent(false);
            CancelToken = new CancellationTokenSource();
        }

        public bool DataSendFrame(byte[] sendlst, int offset, int sendTimeOut = 20000)
        {
            var msg = string.Join(" ", Array.ConvertAll(sendlst, b => $"{b:X2}"));
            _vm.SaveDataFrame(msg);
            _resetEvent.Set();
            Thread.Sleep(5);
            return true;
        }

        public List<byte> Receive()
        {
            var lst = new List<byte>();
            if (_resetEvent.WaitOne(100))
            {
                lst.AddRange(Encoding.ASCII.GetBytes("picok"));
            }

            return lst;
        }
    }
}
