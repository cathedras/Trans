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
    public interface IDev
    {

        TcpClient Sock
        {
            set;
            get;
        }
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
            //set;
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
        Pcomm Pcom
        {
            set;
            get;
        }


        bool Send(byte[] sendlst, int offset);
        List<byte> Receive();


    }
}
