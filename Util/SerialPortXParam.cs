using System;
using System.IO.Ports;

namespace myzy.Util
{
    public class SerialPortXParam
    {
        public string ComName { get; internal set; }

        public string PreSetComName { get;}

        public SerialPortXParam(string preSetComName, int baud, int dataBits, Parity parity, StopBits stopBits, byte[] queryString, Func<byte[], bool> queryRegex)
        {
            Baud = baud;
            Parity = parity;
            StopBits = stopBits;
            QueryString = queryString;
            QueryRegex = queryRegex;
            PreSetComName = preSetComName;
            DataBits = dataBits;
            IsAutoSearch = true;
        }

        public bool IsAutoSearch { get; set; }

        public int DataBits { get;}

        public int Baud { get; }

        public Parity Parity { get; }

        public StopBits StopBits { get; }
        /// <summary>
        /// 查询用指令
        /// </summary>
        public byte[] QueryString { get; }
        /// <summary>
        /// 查询返回指令判定规则
        /// </summary>
        public Func<byte[], bool> QueryRegex { get; }
    }
}