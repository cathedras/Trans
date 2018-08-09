using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using myzy.Util;

namespace TspUtil
{
    /// <summary>
    /// this is command mode class,if you need you can add a new method to the last one,then qoute it upstatirs and run the qoutes
    /// </summary>
    public class SendClientConfig
    {
        private IVmParam _vmParam;
        public Invoker invoker = null;

        public IVmParam VmParam
        {
            get => _vmParam;
            set => _vmParam = value;
        }

        public SendClientConfig()
        {
            ListenUtil lu = new ListenUtil(VmParam);
            Command command = new ConcreteCommand(lu);
            Invoker invoker = new Invoker(command);
            invoker.NetWorkListenCommand();
        }

        public SendClientConfig(IVmParam vmParam)
        {
            this._vmParam = vmParam;
            ListenUtil lu = new ListenUtil(VmParam);
            Command command = new ConcreteCommand(lu);
            invoker = new Invoker(command);
           
        }
        public bool NetWorkListener()
        {
            invoker.NetWorkListenCommand();
            return true;
        }
        public void PcomConfigure()
        {
            invoker.PcomConfigCommand();
        }

    }

    public class Invoker//coach
    {
        public Command _command;

        public Invoker(Command command)
        {
            this._command = command;
        }

        public void NetWorkListenCommand()
        {
            _command.ListenExe();
        }
        public void PcomConfigCommand()
        {
            _command.PcommExe();
        }
    }
    public abstract class Command//abstract command
    {
        protected ListenUtil listen;

        public Command(ListenUtil listen)
        {
            this.listen = listen;
        }

        public abstract void ListenExe();
        public abstract void PcommExe();
    }
    public class ConcreteCommand : Command//command class
    {
        public ConcreteCommand(ListenUtil listen) 
            : base(listen)
        {
        }

      
        public override void ListenExe()
        {
            this.listen.Listen();
        }

        public override void PcommExe()
        {
            this.listen.SerialConfig();
        }
    }
    public class ListenUtil 
    {
        public IVmParam _vmParam;
        public TcpListener _tcpListener = null;
        private int _taskId = 0;
        public ListenUtil(IVmParam VmParam)
        {
            this._vmParam = VmParam;
        }

        public void Listen()
        {
            try
            {
                if (_vmParam.LinkTimes == 0)
                {
                    _tcpListener = new TcpListener(_vmParam.IpAddress, _vmParam.Port);
                    _tcpListener.Start();
                }

                //check the client's connection by cycle
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();
        
                _vmParam.ClientList.TryAdd(_vmParam.ClientId, tcpClient);
                _vmParam.LinkTimes++;
                _vmParam.IsConnected = true;
                _vmParam.Even.TryAdd(_vmParam.ClientId, new ManualResetEvent(true));
                _vmParam.Even[_vmParam.ClientId].Reset();
                _vmParam.ClientId++;
                Task.Factory.StartNew((obj) =>
                {
                    var client = obj as TcpClient;
                    byte[] tmp = new byte[512];
                    if (client != null)
                    {
                        var revList = new List<byte>();
                        var stream = client.GetStream();
                        var receivingStr = new List<string>();
                        int currentId = _taskId++;
                        _vmParam.Even[currentId].Set();
                        while (client.Client.Poll(-1, SelectMode.SelectRead) && client.Connected && client.Available != 0)
                        {
                            //VmParam.Even[VmParam.ClientId].Reset();
                            var count = stream.Read(tmp, 0, tmp.Length);
                            if (count > 0)
                            {
                                revList.AddRange(tmp.Take(count));
                            }

                            //TODO
                            var receivestring = string.Empty;
                            if (count <= revList.Count)
                            {
                                List<byte> tmplst = new List<byte>();
                                tmplst.AddRange(revList.Take(count));
                                receivestring = Encoding.Default.GetString(tmplst.ToArray());
                                revList.RemoveRange(0, tmplst.Count);
                                tmplst.Clear();
                                receivingStr.Add(receivestring);
                                _vmParam.ReceivingString.TryAdd(currentId, receivingStr);
                            }
                            _vmParam.Even[currentId].Set();
                            Thread.Sleep(10);
                        }
                        _vmParam.IsConnected = false;
                        _vmParam.Even[currentId].Set();
                        _vmParam.ClientList.TryRemove(currentId, out TcpClient dump);
                    }
                }, tcpClient);
            }
            catch (Exception es)
            {
                ViewModel.LogPrint(es.ToString());
            }
        }
        Pcomm pcomm = new Pcomm();
        public void SerialConfig()
        {
            _vmParam.Pcom = null;
            _vmParam.Even.TryAdd(0, new ManualResetEvent(true));
            var hasht = new Hashtable()
            {
                {Pcomm.KEY_PORT, $"{_vmParam.Com}"},
                {Pcomm.KEY_BAUDRATE, $"{_vmParam.Baud}"},
                {Pcomm.KEY_BYTESIZE, $"{_vmParam.DataBits}"},
                {Pcomm.KEY_PARITY, $"{_vmParam.Parity}"},
                {Pcomm.KEY_STOPBITS, $"{(int) _vmParam.StopBits}"},
                {Pcomm.KEY_READINTERVALTIMEOUT, $"{(int) _vmParam.IntevalTime}"},
                {Pcomm.KEY_READTOTALTIMEOUT, $"{(int) _vmParam.TotalTime}"}
            };
            pcomm.InitComm(hasht);
            _vmParam.Pcom = pcomm;
        }

    }
}
