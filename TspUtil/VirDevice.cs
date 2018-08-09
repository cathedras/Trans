using myzy.Util;
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
    class VirDevice : IDev
    {
        public TcpClient Sock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ManualResetEvent ResetEvent => throw new NotImplementedException();

        public List<byte> HeaderList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<byte> FinalList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SendLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CancellationTokenSource CancelToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsReceiveNg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long LmgLenCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Pcomm Pcom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<byte> Receive()
        {
            throw new NotImplementedException();
        }

        public bool Send(byte[] sendlst, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
