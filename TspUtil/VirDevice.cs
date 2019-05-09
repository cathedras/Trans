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
        public ManualResetEvent ResetEvent { get; }
        public List<byte> HeaderList { get; set; }
        public List<byte> FinalList { get; set; }
        public int SendLength { get; set; }
        public CancellationTokenSource CancelToken { get; }
        public bool IsReceiveNg { get; set; }
        public long LmgLenCount { get; set; }

        public bool DataSendFrame(byte[] sendlst, int offset, int sendTimeOut = 20000)
        {
            return true;
        }

        public List<byte> Receive()
        {
            throw new NotImplementedException();
        }
    }
}
