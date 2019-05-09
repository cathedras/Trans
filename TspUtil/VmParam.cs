using ElCommon.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TspUtil
{
    public class VmParam001
    {
        //thes fields is saving for Command mode class and View Model to transfer params.
        private IPAddress _ipAddress;
        private int _port;
        private readonly ConcurrentDictionary<int, TcpClient> _clientList = new ConcurrentDictionary<int, TcpClient>();
        private ConcurrentDictionary<int, List<string>> _receivingString = new ConcurrentDictionary<int, List<string>>();
        private int _clientId = 0;
        private ConcurrentDictionary<int, ManualResetEvent> _even = new ConcurrentDictionary<int, ManualResetEvent>();
        private int _linkTimes = 0;
        private string _com = string.Empty;
        private string _baud = string.Empty;
        private string _dataBits = string.Empty;
        private string _parity = string.Empty;
        private int _stopBits = 1;
        private int _totalTime = 100;
        private int _intevaTime = 30;
        private Pcomm _pcom;

        public IPAddress IpAddress
        {
            get => _ipAddress;
            set
            {
                if (value == _ipAddress) return;
                _ipAddress = value;
            }
        }
        public int ClientId
        {
            get => _clientId;
            set
            {
                if (value == _clientId) return;
                _clientId = value;
            }
        }
        public int Port
        {
            get => _port;
            set
            {
                if (value == _port) return;
                 _port = value;
            }
        }
        public ConcurrentDictionary<int, TcpClient> ClientList
        {
            get => _clientList;
        }
        public ConcurrentDictionary<int, List<string>> ReceivingString
        {
            get => _receivingString;
        }
        public ConcurrentDictionary<int, ManualResetEvent> Even
        {
            get => _even;
        }
        public int LinkTimes
        {
            get => _linkTimes;
            set
            {
                _linkTimes = value;
            }
        }

        public string Com
        {
            get => _com;
            set
            {
                _com = value;
            }
        }
        public string Baud
        {
            get => _baud;
            set
            {
                _baud = value;
            }
        }
        public string DataBits
        {
            get => _dataBits;
            set
            {
                _dataBits = value;
            }
        }
        public string Parity
        {
            get => _parity;
            set
            {
                _parity = value;
            }
        }
        public int StopBits
        {
            get => _stopBits;
            set 
            {
                _stopBits = value;
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

        public int IntevalTime
        {
            get => _intevaTime;
            set
            {
                _intevaTime = value;
            }
        }
        public int TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
            }
        }
    }



}
