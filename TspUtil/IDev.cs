using ElCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TspUtil
{
    public interface IDev
    {
        ManualResetEvent ResetEvent
        {
            get;
        }
        List<byte> HeaderList
        {
            set;
            get;
        }
        List<byte> FinalList
        {
            set;
            get;
        }
        int SendLength
        {
            set;
            get;
        }
        CancellationTokenSource CancelToken
        {
            get;
        }
        bool IsReceiveNg
        {
            get;
            set;
        }
        long LmgLenCount
        {
            get;
            set;
        }

        bool DataSendFrame(byte[] sendlst, int offset,int sendTimeOut = 500);
        List<byte> Receive();
    }
}
