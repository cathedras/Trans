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
using log4net;
using ElCommon.Util;

namespace TspUtil
{
    /*
    /// <summary>
    /// this is command mode class,if you need you can add a new method to the last one,then qoute it upstatirs and run the qoutes
    /// </summary>
    public class DataSendClient
    {
        private IVmParam _vmParam;
        public Invoker invoker = null;

        public IVmParam VmParam
        {
            get => _vmParam;
            set => _vmParam = value;
        }

        private readonly ListenUtil lu;

        public DataSendClient(IVmParam vmParam)
        {
            this._vmParam = vmParam;
            lu = new ListenUtil(VmParam);
        }
        public bool NetWorkListener(out TcpClient tcpClient)
        {
            tcpClient = null;
            try
            {
                if (_vmParam.LinkTimes == 0)
                {
                    _tcpListener = new TcpListener(_vmParam.IpAddress, _vmParam.Port);
                    _tcpListener.Start();
                }

                //check the client's connection by cycle
                tcpClient = _tcpListener.AcceptTcpClient();

                Console.WriteLine($@"TCP Listen <----- {tcpClient}");

                _vmParam.ClientId++;
                int currentId = _vmParam.ClientId;
                _vmParam.ClientList.TryAdd(_vmParam.ClientId, tcpClient);
                _vmParam.LinkTimes++;
                _vmParam.Even.TryAdd(_vmParam.ClientId, new ManualResetEvent(true));
                _vmParam.Even[_vmParam.ClientId].Reset();
                Task.Factory.StartNew((obj) =>
                {
                    var client = obj as TcpClient;
                    byte[] tmp = new byte[512];
                    if (client != null)
                    {
                        var revList = new List<byte>();
                        var stream = client.GetStream();
                        var receivingStr = new List<string>();
                        _vmParam.Even[currentId].Set();
                        while (true)
                        {
                            var pollState = client.Client.Poll(500, SelectMode.SelectRead);

                            //断线检测不出来？？
                            //TODO:
                            if (pollState && client.Available == 0
                                 || !client.Client.Connected)
                                break;

                            if (client.Available == 0 && client.Connected)
                                continue;

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
                        //_vmParam.IsConnected = false;
                        _vmParam.Even[currentId].Set();
                        _vmParam.ClientList.TryRemove(currentId, out TcpClient dump);
                    }
                }, tcpClient);
            }
            catch (Exception es)
            {
                _log.Error(es.Message);
                tcpClient = null;
            }

            return tcpClient != null;
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
    }*/

    public class DevInitUtil 
    {
        private static readonly ILog _log = LogManager.GetLogger("exlog");

        private static TcpListener _tcpListener = null;

        public static void Listen(Gbl gbl, out TcpClient tcpClient)
        {
            tcpClient = null;
            try
            {
                if (_tcpListener == null)
                {
                    _tcpListener = new TcpListener(IPAddress.Any, gbl.Port);
                    _tcpListener.Start();
                }

                _log.Debug($"Start TCP Listen...");
                //check the client's connection by cycle
                tcpClient = _tcpListener.AcceptTcpClient();
                

                _log.Debug($@"TCP Listen <----- {tcpClient}");
            }
            catch (Exception es)
            {
                _log.Error(es.Message);
                tcpClient = null;
            }
        }

        public static bool SerialConfig(Gbl gbl, out Pcomm pcomm)
        {
            pcomm = new Pcomm();
            var hasht = new Hashtable()
            {
                {Pcomm.KEY_PORT, $"{gbl.SelectCom}"},
                {Pcomm.KEY_BAUDRATE, $"{gbl.SelectSpeed}"},
                {Pcomm.KEY_BYTESIZE, $"{gbl.SelectDataBits}"},
                {Pcomm.KEY_PARITY, $"{gbl.SelectParity}"},
                {Pcomm.KEY_STOPBITS, $"{gbl.SelectStopBits}"},
                {Pcomm.KEY_READINTERVALTIMEOUT, $"{gbl.IntevaTime}"},
                {Pcomm.KEY_READTOTALTIMEOUT, $"{gbl.TotalTime}"}
            };
            pcomm.InitComm(hasht);
            return true;
        }

        public static void ConnectToServer(Gbl gbl,out TcpSocketEx tcp)
        {
            var client = new TcpSocketEx(gbl.RemoteIpAddress,gbl.Port);
            try
            {
                client.Connect(IPAddress.Parse(gbl.RemoteIpAddress), gbl.Port);
                tcp = client;
            }
            catch(Exception e)
            {
                tcp = null;
            }           
        }

        public static async void Connection(TcpClient client, Gbl gbl)
        {
            try
            {
                await client.ConnectAsync(gbl.RemoteIpAddress, gbl.Port);
            }
            catch (Exception ex)
            {
                throw new SocketException(-1);
            }
        }
        
        public static void ReleaseConnection(TcpSocketEx client)
        {
            client.Disconnect(false);
        }
    }
}
