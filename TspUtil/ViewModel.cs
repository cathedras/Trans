
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using myzy.Util;
using Microsoft.Win32;
using TspUtil.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;
using log4net;
using System.IO.Ports;
using System.Windows.Threading;

namespace TspUtil
{
    public class ViewModel : INotifyPropertyChanged
    {
        //base attribute
        private ObservableCollection<LogItem> _logItems;
        private ObservableCollection<ImgItemInfo> _imgItemInfos;
        private ObservableCollection<ClientList> _viewClients;
        private readonly string _cfg = @"..\tspUtil.ini";
        private readonly string _usingFileName = "_tmpUsing.bmp";
        private string _activeFn;
        private readonly Gbl _gbl;
        private bool _isEthSim;

        //sock
        private string _currentIp = "未选择";

        //pic config
        private int _expPixHeight;
        private int _expPixWidth;
        private string _swVersion;
        private PadLoc _padLoc;
        private string _padStr;
        private bool _isInverse;

        //window attribute
        private ImageSource _imgSource;
        private ObservableCollection<MaskForArgbItem> _oddMaskArgb;
        private ObservableCollection<MaskForArgbItem> _evenMaskArgb;
        private string _oddOffset;
        private string _evenOffset;
        private double _progressData = 0;
        private double _maximum = 100;
        private ObservableCollection<ItemSourceItemInfo> _serialSpeed;
        private ObservableCollection<ItemSourceItemInfo> _serialComs;
        private string _selectCom;
        private string _selectSpeed;
        //window status attribute
        private bool _panelUnLock = true;
        private bool _isSerialSend = false;
        private bool _isNetWorkSend = false;

        //clients' attribute
        private readonly ConcurrentDictionary<int, IDev> _clientRunList = new ConcurrentDictionary<int, IDev>();
        static VmParam _vmParam = new VmParam();
        SendClientConfig _configure = new SendClientConfig(VmParam);
        private int _selectClientId = 0;
        private bool _usinSimData = false;
        public bool IsSerialSend
        {
            get => _isSerialSend;
            set
            {
                if (value == _isSerialSend) return;
                _isSerialSend = value;
                _gbl.IsSerialSend = value;
                OnPropertyChanged();
            }
        }
        public bool IsNetWorkSend
        {
            get => _isNetWorkSend;
            set
            {
                if (value == _isNetWorkSend) return;
                _isNetWorkSend = value;
                _gbl.IsNetWorkSend = value;
                OnPropertyChanged();
            }
        }
        public string SelectCom
        {
            get => _selectCom;
            set
            {
                if (value == _selectCom) return;
                _selectCom = value;
                _gbl.SelectCom = value;
                OnPropertyChanged();
            }
        }
        public string SelectSpeed
        {
            get => _selectSpeed;
            set
            {
                if (value == _selectSpeed) return;
                _selectSpeed = value;
                _gbl.SelectSpeed = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ItemSourceItemInfo> SerialComs
        {
            get => _serialComs ?? (_serialComs = new ObservableCollection<ItemSourceItemInfo>());
        }

        public ObservableCollection<ItemSourceItemInfo> SerialSpeed
        {
            get => _serialSpeed ?? (_serialSpeed = new ObservableCollection<ItemSourceItemInfo>());
           
        }

        public string OddOffset
        {
            get => _oddOffset;
            set
            {
                if (value == _oddOffset) return;
                _oddOffset = value;
                _gbl.OddOffset = Int32.Parse(value);
                OnPropertyChanged();
            }
        }
        public string EvenOffset
        {
            get => _evenOffset;
            set
            {
                if (value == _evenOffset) return;
                _evenOffset = value;
                _gbl.EvenOffset = Int32.Parse(value);
                OnPropertyChanged();
            }
        }

        public bool UsinSimData
        {
            get => _usinSimData;
            set
            {
                if (value == _usinSimData) return;
                _usinSimData = value;
                _gbl.UsingSimData = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ClientList> ViewClients
        {
            get => _viewClients ?? (_viewClients = new ObservableCollection<ClientList>());
        }
        public int SelectClientId
        {
            get => _selectClientId;
            set => _selectClientId = value;
        }

        public SendClientConfig Configure
        {
            get => _configure;
            set => _configure = value;
        }
        public static VmParam VmParam
        {
            get => _vmParam;
            set => _vmParam = value;
        }
        public ConcurrentDictionary<int, IDev> ClientRunList
        {
            get => _clientRunList;
        }
        public string CurrentIp
        {
            get => _currentIp;
            set
            {
                if (value == _currentIp) return;
                _currentIp = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LogItem> LogItems
        {
            get => _logItems ?? (_logItems = new ObservableCollection<LogItem>());
        }
        public ImageSource ImgSource
        {
            get => _imgSource;
            set
            {
                _imgSource = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ImgItemInfo> ImgItemInfos
        {
            get => _imgItemInfos ?? (_imgItemInfos = new ObservableCollection<ImgItemInfo>());
        }
        public bool IsEthSim
        {
            get => _isEthSim;
            set
            {
                _gbl.IsEthSim = value;
                _isEthSim = value;
            }
        }
        public IPAddress IpAddress
        {
            get => VmParam.IpAddress;
            set
            {
                VmParam.IpAddress = value;
                OnPropertyChanged();
            }
        }
        public bool IsConnected
        {
            get => VmParam.IsConnected;
            set
            {
                VmParam.IsConnected = value;
                OnPropertyChanged();
            }
        }
        public int Port
        {
            get => VmParam.Port;
            set
            {
                VmParam.Port = value;
                OnPropertyChanged();
            }
        }
        public bool PanelUnLock
        {
            get => _panelUnLock;
            set
            {
                if (value == _panelUnLock) return;
                _panelUnLock = value;
                OnPropertyChanged();
            }
        }


        public string SwVersion
        {
            get => _swVersion;
            set
            {
                if (value == _swVersion) return;
                _swVersion = value;
                OnPropertyChanged();
            }
        }
        public string ActiveFn
        {
            get => _activeFn;
            set
            {
                if (value == _activeFn) return;
                _activeFn = value;
                OnPropertyChanged();
            }
        }

        public int ExpPixWidth
        {
            get => _expPixWidth;
            set
            {
                if (value == _expPixWidth) return;
                _expPixWidth = value;

                _gbl.ExpByteWidth = value;
                OnPropertyChanged();
            }
        }

        public int ExpPixHeight
        {
            get => _expPixHeight;
            set
            {
                if (value == _expPixHeight) return;
                _expPixHeight = value;
                _gbl.ExpByteHeight = value;
                OnPropertyChanged();
            }
        }

        public PadLoc PadLoc
        {
            get => _padLoc;
            set
            {
                if (value == _padLoc) return;
                _padLoc = value;
                _gbl.PadLoc = value.ToString();
                OnPropertyChanged();
            }
        }

        public string PadStr
        {
            get => _padStr;
            set
            {
                if (value == _padStr) return;
                _padStr = value;
                _gbl.PadStr = value;
                OnPropertyChanged();
            }
        }
        public double ProgressData
        {
            get => _progressData;
            set
            {
                if (value == _progressData) return;
                _progressData = value;
                OnPropertyChanged();
            }
        }
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (value == _maximum) return;
                _maximum = value;
                OnPropertyChanged();
            }
        }


        public bool IsInverse
        {
            get => _isInverse;
            set
            {
                if (value == _isInverse) return;
                _isInverse = value;
                _gbl.IsInverse = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// this two list is using for the window param odd number and even number
        /// </summary>
        public ObservableCollection<MaskForArgbItem> OddMaskArgb
        {
            get => _oddMaskArgb ?? (_oddMaskArgb = new ObservableCollection<MaskForArgbItem>());
        }

        public ObservableCollection<MaskForArgbItem> EvenMaskArgb
        {
            get => _evenMaskArgb ?? (_evenMaskArgb = new ObservableCollection<MaskForArgbItem>());
        }

        public Gbl GblInfo
        {
            get { return _gbl; }
        }

        /// <summary>
        /// Add log to window with level index
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void AddLogMsg(string msg, int level = 0)
        {
           var dispatcher =  Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                if (dispatcher.CheckAccess())
                {
                    LogItems.Add(new LogItem()
                    {
                        DateTime = DateTime.Now,
                        Info = msg,
                        Level = level
                    });
                }
                else
                {
                    dispatcher.Invoke(() => { AddLogMsg(msg, level); });
                }
            }
        }
        /// <summary>
        /// Save the ini file.
        /// </summary>
        public void SaveDataOnExit()
        {
            try
            {
                _gbl.OddRgbA = OddMaskArgb[0].ExpRGBA;
                _gbl.EvenRgbA = EvenMaskArgb[0].ExpRGBA;
                _gbl.IsInverse = IsInverse;
                _gbl.UsingSimData = UsinSimData;
                
                _gbl.Save(_cfg, typeof(Gbl));
            }
            catch (Exception e)
            {
                AddLogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Clients' send method with 1024
        /// </summary>
        private readonly int _sendTimeOut = 2000;
        private bool DataSend(byte[] buffer, int currentSockId)
        {
            ManualResetEvent revResetEvent = null;
            if (IsNetWorkSend)
            {
                 revResetEvent = ClientRunList[currentSockId].ResetEvent;
            }
            else
            {
                revResetEvent = _serial.ResetEvent;
            }

            bool isFrameSendSuccess = false;
            try
            {
                var sw = new Stopwatch();
                while (sw.Elapsed.Milliseconds < _sendTimeOut)
                {
                    sw.Start();
                    revResetEvent.Reset();
                    if (IsNetWorkSend)
                    {
                        ClientRunList[currentSockId].LmgLenCount += buffer.Length;
                    }
                    else
                    {
                        _serial.LmgLenCount += buffer.Length;
                    }
                    if (!IsEthSim)
                    {
                        if (ClientRunList.Count != 0)
                        {
                            ClientRunList[currentSockId]?.Send(buffer, 0);
                        }
                        _serial?.Send(buffer, 0);
                    }
                    else
                    {
                        WriteToTxt.WriteOut(string.Join(" ", Array.ConvertAll(buffer, p => $"{p:X2}")));
                        var tsk = Task.Factory.StartNew(() =>
                        {
                            revResetEvent.Set();
                        });
                        WriteToTxt.WriteOut("\r\n");
                    }
                    
                    if (revResetEvent.WaitOne())
                    {
                        isFrameSendSuccess = true;
                        break;
                    }
                    else
                    {
                        isFrameSendSuccess = false;
                        AddLogMsg("数据发送失败，请重新发送");
                        break;
                    }
                }
                sw.Stop();
            }
            catch (Exception x)
            {
                AddLogMsg(x.Message);
            }
            return isFrameSendSuccess;
        }
        /// <summary>
        /// data send with a frame and wait 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="currentSockId"></param>
        /// <returns></returns>
        private bool DataSend(byte[] data, int size, int currentSockId)
        {
            var dataBuffer = new byte[size];
            int length = (data.Length % size == 0)
                ? data.Length / size
                : (data.Length - data.Length % size) / size + 1;
            bool sendState = false;
            ClientRunList[currentSockId].SendLength = 0;
            for (int i = ClientRunList[currentSockId].SendLength; i < length; i++)
            {
                var ct = ClientRunList[currentSockId].CancelToken.Token;
                if (ct.IsCancellationRequested) break;
                if (data.Length < (i + 1) * size)
                {
                    if (ClientRunList[currentSockId].IsReceiveNg)
                    {
                        i--;
                    }
                    byte[] newSend = new byte[data.Length - (i) * size];
                    Array.Copy(data, i * size, newSend, 0, newSend.Length);
                    sendState = DataSend(newSend, currentSockId);
                    if (!sendState)
                    {
                        AddLogMsg("最后未满规定字节数的一帧数据发送失败，请重新发送");
                        break;
                    }
                }
                else
                {
                    if (ClientRunList[currentSockId].IsReceiveNg)
                    {
                        i--;
                    }
                    Array.Copy(data, i * size, dataBuffer, 0, size);
                    sendState = DataSend(dataBuffer, currentSockId);
                    if (!sendState)
                    {
                        AddLogMsg("本帧数据未能成功发送，请重新开始发送");
                        break;
                    }
                }
                ClientRunList[currentSockId].SendLength = i;
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    double strike = ((double)i / length) * Maximum;
                    ProgressData = strike;
                    if (i == length - 1)
                    {
                        ProgressData = Maximum;
                    }
                });
            }
            return sendState;
        }
        /// <summary>
        /// bin file send by serial for every one frame
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool SerialDataSend(byte[] data, int size)
        {
            var dataBuffer = new byte[size];
            int length = (data.Length % size == 0)
                ? data.Length / size
                : (data.Length - data.Length % size) / size + 1;
            bool sendState = false;
            _serial.SendLength = 0;
            for (int i = _serial.SendLength; i < length; i++)
            {
                var ct = _serial.CancelToken.Token;
                if (ct.IsCancellationRequested) break;
                Array.Copy(data, i * size, dataBuffer, 0, size);
                sendState = DataSend(dataBuffer, 0);
                if (!sendState)
                {
                    AddLogMsg("本帧数据未能成功发送，请重新开始发送");
                    break;
                }
                _serial.SendLength = i;
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    double strike = ((double)i / length) * Maximum;
                    ProgressData = strike;
                    if (i == length - 1)
                    {
                        ProgressData = Maximum;
                    }
                });
            }
            return sendState;
        }
        /// <summary>
        /// Send all bmpphoto by network. 
        /// </summary>
        public void NetWorkSendAll()
        {
            byte[] erase = StringToByteArray("erase");//清除指令
            byte[] store = StringToByteArray("store");//保存指令
            if (IsConnected)
            {
                foreach (var dev in ClientRunList)
                {
                    dev.Value.HeaderList = new List<byte>();
                    dev.Value.FinalList = new List<byte>();
                }

                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                {
                    var currentSockId = pair.Key;

                    if (!DataSend(erase, currentSockId))
                    {
                        AddLogMsg($"erase指令发送失败, {currentSockId}");
                    }
                    else
                    {
                        for (int i = 0; i < ImgItemInfos.Count; i++)
                        {

                            if (ImgItemInfos[i].IsActived)
                            {
                                if (!ActiveImgItem(ImgItemInfos[i], currentSockId))
                                {
                                    AddLogMsg("解析失败！");
                                }
                                //data analyze
                                else if (!DataSend(ClientRunList[currentSockId].HeaderList.ToArray(), ClientRunList[currentSockId].HeaderList.Count, currentSockId))
                                {
                                    AddLogMsg("图片头发送失败，请重新发送");
                                }
                                else if (!DataSend(ClientRunList[currentSockId].FinalList.ToArray(), 1024, currentSockId))
                                {
                                    AddLogMsg("图片数据发送失败，请重新发送");
                                }
                                else
                                {
                                    //picoff
                                    byte[] picoff = StringToByteArray("picoff");
                                    List<byte> Checksum = new List<byte>();
                                    Checksum.AddRange(DataBmpAlg.HexStringToByteArray(ImgItemInfos[i].Cs));

                                    byte[] send = new byte[Checksum.ToArray().Length + picoff.Length];
                                    Array.Copy(picoff, 0, send, 0, 6);
                                    Array.Copy(Checksum.ToArray(), 0, send, picoff.Length, Checksum.ToArray().Length);
                                    if (!DataSend(send, send.Length, currentSockId))
                                    {
                                        AddLogMsg("pioff发送失败，结尾发送失败请重新发送");
                                    }
                                    else if (!DataSend(store, currentSockId))
                                    {
                                        AddLogMsg("指令发送失败，请重新发送");
                                    }
                                    else
                                    {
                                        AddLogMsg("图片发送完成,总共发送字节数：" + ClientRunList[currentSockId].LmgLenCount);
                                        ClientRunList[currentSockId].LmgLenCount = 0;
                                        ImgItemInfos[i].ImgOpState = ImgOpState.Success;
                                    }
                                }

                                if (ImgItemInfos[i].ImgOpState == ImgOpState.None)
                                    ImgItemInfos[i].ImgOpState = ImgOpState.Fail;
                            }
                            else
                            {
                                AddLogMsg("无图片被选中，无法解析");
                                break;
                            }
                        }
                    }
                }));

                Task.WaitAll(tsks.ToArray());
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送");
            }
        }
        /// <summary>
        /// serial send data 
        /// </summary>
        public void SerialSendAll()
        {
            if (ImgItemInfos.Any() && ImgItemInfos[ImgItemInfos.Count - 1].IsActived)
            {
                Task.Factory.StartNew(new Action(() =>
                {
                    if (ActiveBinItem(ImgItemInfos[0]))
                    {
                        AddLogMsg("开始发送bin");
                        if (!DataSend(Serial.start, 0))
                        {
                            AddLogMsg("未能成功发送命令");
                        }
                        else if (!SerialDataSend(_serial.FinalList.ToArray(), 258))
                        {
                            AddLogMsg("未能成功发送数据");
                        }
                        else if (!DataSend(Serial.end, 0))
                        {
                            AddLogMsg("未发送完成数据结束标志");
                        }
                        else
                        {

                            _serial.CancelToken.Cancel();
                            _serial.FinalList.Clear();
                            AddLogMsg("发送完成");
                        }
                    }
                    else
                    {
                        AddLogMsg("解析失败！");
                    }
                }));
            }
            else
            {
                AddLogMsg("无bin文件选择，请打开一个bin文件 或者 无com口连接");
            }
        }
        /// <summary>
        /// Make a byte array for the string
        /// </summary>
        /// <param name="command">The string what you need to transfer</param>
        /// <returns></returns>
        public byte[] StringToByteArray(String command)
        {
            return System.Text.Encoding.Default.GetBytes(command);
        }
        /// <summary>
        /// header create for img send data
        /// </summary>
        /// <param name="picIndex">acquiesce to zero</param>
        /// <param name="size">your data list or array is how length </param>
        /// <param name="name">you open the file's name</param>
        /// <returns>the head array</returns>
        public byte[] CreateHeadData(int picIndex=0, int size=0, string name="")
        {
            List<byte> head = new List<byte>();
            head.AddRange(StringToByteArray("pic"));
            head.Add(Convert.ToByte(picIndex));//4
            head.AddRange(StringToByteArray("BM"));
            head.AddRange(BitConverter.GetBytes(size));//get the picture's data length
            head.AddRange(StringToByteArray(name));
            return head.ToArray();
        }


        public ViewModel()
        {
            SwVersion = "1.0.0b";
#if DEBUG
            SwVersion = "0.0.0";
#endif
            _gbl = new Gbl();
            _gbl.LoadGbl<Gbl>(_cfg);


            IsEthSim = _gbl.IsEthSim;
            Connections();



            ExpPixHeight = _gbl.ExpByteHeight;
            ExpPixWidth = _gbl.ExpByteWidth;
            Enum.TryParse(_gbl.PadLoc, out _padLoc);
            SelectCom = _gbl.SelectCom;
            SelectSpeed = _gbl.SelectSpeed;
            LocalSerialComm();
            PadStr = _gbl.PadStr;
            _gbl.IpAddress = LocalIPAddress();
            IpAddress = IPAddress.Parse(_gbl.IpAddress);
            Port = _gbl.Port;
            IsInverse = _gbl.IsInverse;
            UsinSimData = _gbl.UsingSimData;
            OddOffset = _gbl.OddOffset.ToString();
            EvenOffset = _gbl.EvenOffset.ToString();
            IsSerialSend = _gbl.IsSerialSend;
            IsNetWorkSend = _gbl.IsNetWorkSend;

            OddMaskArgb.Add(new MaskForArgbItem(_gbl.OddRgbA));
            EvenMaskArgb.Add(new MaskForArgbItem(_gbl.EvenRgbA));

        }
        /// <summary>
        /// network connection lisenning
        /// </summary>
        private void Connections()
        {
            if (!IsEthSim)
            {
                new Task(new Action(delegate ()
                {
                    while (true)
                    {
                        if (IsNetWorkSend)
                        {
                            SockListen();
                        }
                    }
                })).Start();
            }
            else
            {
                CurrentIp = "模拟客户端";
                IsConnected = true;
            }
        }





        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Get local coms and add it to the window
        /// </summary>
        public void LocalSerialComm()
        {
            foreach (string vPortName in SerialPort.GetPortNames())
            {
                SerialComs.Add(new ItemSourceItemInfo()
                {
                    Name = vPortName,
                    Des = vPortName,
                    Val = vPortName,
                });
            }
            var speed = new string[] { "9600", "19200", "115200" };
            for(int i=0; i < speed.Length; i++)
            {
                SerialSpeed.Add(new ItemSourceItemInfo()
                {
                    Name = speed[i],
                    Des = speed[i],
                    Val = speed[i],
                });
            }  
        }


        /// <summary>
        /// Get local ip address for the server.
        /// </summary>
        /// <returns>Local machine ip address</returns>
        public string LocalIPAddress()
        {
            string ipaddress = string.Empty;
            try
            {
                string hostName = Dns.GetHostName(); //get your computer name
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                for (int i = 0; i < ipEntry.AddressList.Length; i++)
                {
                    //check from the address list which is similar 
                    if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipaddress = ipEntry.AddressList[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                AddLogMsg("网络客户端获取错误："+ex.ToString());
            }
            return ipaddress;
        }

        /// <summary>
        /// open a task for listenning the background network connection
        /// </summary>
        int _listenTaskId = 0;
        private void SockListen()
        {
            string clientIp = string.Empty;
            if (Configure.NetWorkListener() && Configure.VmParam.ClientList.Count != 0)
            {
                int clientId = Configure.VmParam.ClientId;
                clientId--;
                Configure.VmParam.Even[clientId].WaitOne(200);
                ClientRunList.TryAdd(clientId, new NetClient(Configure.VmParam.ClientList[clientId]));
                ClientRunList[clientId].Sock = Configure.VmParam.ClientList[clientId];
                if (ClientRunList[clientId].Sock != null)
                {
                    clientIp = ClientRunList[clientId].Sock.Client.RemoteEndPoint.ToString();
                    CurrentIp = clientIp;
                }

                AddLogMsg("客户端" + clientIp + "已连接到本服务器");
                //clientId = 0;
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ViewClients.Add(new ClientList(clientId, "客户端: " + ClientRunList[clientId].Sock.Client.RemoteEndPoint.ToString(), Configure.VmParam.IsConnected));
                });
                //client
                Task.Factory.StartNew(() =>
                {
                    int currentSockId = _listenTaskId++;

                    while (true)
                    {
                        Configure.VmParam.Even[currentSockId].WaitOne();
                        List<string> tmpReceivingString = null;
                        if ((Configure.VmParam.ReceivingString.Count) > 0)
                        {
                            tmpReceivingString = Configure.VmParam.ReceivingString[currentSockId];
                        }
                        if (tmpReceivingString != null && tmpReceivingString.Contains(NetClient.picok))
                        {
                            ClientRunList[currentSockId].ResetEvent.Set();
                            Configure.VmParam.ReceivingString[currentSockId].Clear();
                        }
                        else if (tmpReceivingString != null && tmpReceivingString.Contains(NetClient.picNG))
                        {
                            ClientRunList[currentSockId].ResetEvent.Set();
                            AddLogMsg(clientIp + "收到picNG,请重新发送");
                            ClientRunList[currentSockId].IsReceiveNg = true;
                        }
                        else if (tmpReceivingString != null
                        && tmpReceivingString.Count != 0
                        && !tmpReceivingString.Contains(NetClient.picok)
                        && !tmpReceivingString.Contains(NetClient.picNG))
                        {
                            AddLogMsg("客户端为:" + clientIp + ":接受参数错误,请重新发送");
                            Configure.VmParam.ReceivingString[currentSockId].Clear();
                        }
                        else
                        {
                            //device disconnected
                            IsConnected = Configure.VmParam.IsConnected;
                            if (!IsConnected)
                            {
                                AddLogMsg("设备号:" + (currentSockId) + ",   设备ip:" + clientIp + "已断开连接");
                                Application.Current?.Dispatcher.Invoke(() =>
                                {
                                    string curentIp = "客户端: " + ClientRunList[currentSockId].Sock.Client.RemoteEndPoint.ToString();
                                    foreach (ClientList arr in ViewClients)
                                    {
                                        if (arr.ClientIp.Equals(curentIp))
                                        {
                                            ViewClients.Remove(arr);
                                            break;
                                        }
                                    }
                                });
                                if (tmpReceivingString != null && tmpReceivingString.Count != 0)
                                {
                                    tmpReceivingString.Clear();
                                }
                                ClientRunList[currentSockId].CancelToken.Cancel();
                                ProgressData = 0;
                                CurrentIp = "未选择";
                                ClientRunList.Clear();
                                break;
                            }
                        }
                        Configure.VmParam.Even[currentSockId].Reset();
                    }
                });
            }              
        }

        IDev _serial = null;
        /// <summary>
        /// serial pcomm get
        /// </summary>
        private void SerialPcomm()
        {
            VmParam.Com = SelectCom;
            VmParam.Baud = SelectSpeed;
            VmParam.Parity = _gbl.SelectParity;
            VmParam.DataBits = _gbl.SelectDataBits;
            VmParam.StopBits = _gbl.SelectStopBits;
            VmParam.IntevalTime = _gbl.IntevaTime;
            VmParam.TotalTime = _gbl.TotalTime;
            Configure.PcomConfigure();
            _serial = new Serial(Configure.VmParam.Pcom)
            {
               // CancelToken = new CancellationTokenSource()
            };
            AddLogMsg("串口打开成功");
            Task.Factory.StartNew(new Action(() =>
            {
                List<string> strList = new List<string>();
                string buf = string.Empty;
                var buff = new byte[512];
                List<byte> lst = null;
                while (!_serial.CancelToken.IsCancellationRequested)
                {
                    lst = _serial.Receive();
                    if (lst.Count != 0 && lst[0] == Serial.receiveOk)
                    {
                        lst.Clear();
                        _serial.ResetEvent.Set();
                    }
                }
            }), _serial.CancelToken.Token);
        }

        private bool ActiveBinItem(ImgItemInfo imgItem, bool dataProcess = true)
        {
            ActiveFn = string.Empty;
            byte[] oriBytes = null;
            _gbl.BinFileName = imgItem.Des;

            var fs = new FileStream(imgItem.FnPath, FileMode.Open);
            oriBytes = new byte[fs.Length];
            fs.Read(oriBytes, 0, oriBytes.Length);
            fs.Close();

            if (dataProcess)
            {
                Stopwatch watch = new Stopwatch();
                AddLogMsg("解析任务:" + imgItem.Des);
                watch.Start();
                var data = new DataBmpAlg(_gbl, oriBytes, OddMaskArgb[0], EvenMaskArgb[0], PadLoc);
                if (_serial != null)
                {
                    _serial.FinalList = data.FinalData;
                }
                else
                {
                    AddLogMsg("没有serial对象");
                }

                watch.Stop();
                AddLogMsg("解析完成用时" + watch.ElapsedMilliseconds);
            }

            ActiveFn = imgItem.FnPath;
            return true;
        }



        private SemaphoreSlim _slim = new SemaphoreSlim(1);
        /// <summary>
        /// analyze the picture
        /// </summary>
        private bool ActiveImgItem(ImgItemInfo imgItem, int currentTaskId, bool dataProcess = true)
        {
            bool res = false;
            Debug.Assert(imgItem != null);
            _slim.Wait(Timeout.Infinite);
            ActiveFn = string.Empty;
            try
            {
                File.Delete(_usingFileName);
            }
            catch (Exception e)
            {
                AddLogMsg($"Fail to delete {_usingFileName}, MSG = {e.Message}");
            }

            try
            {
                byte[] oriBytes = null;
                var pic = Image.FromFile(imgItem.FnPath);
                pic.Save(_usingFileName, ImageFormat.Bmp);
                pic.Dispose();

                var fs = new FileStream(_usingFileName, FileMode.Open);
                oriBytes = new byte[fs.Length];
                fs.Read(oriBytes, 0, oriBytes.Length);
                fs.Close();

                if (dataProcess)
                {
                    //start monitors the picture analyze task
                    Stopwatch watch = new Stopwatch();
                    AddLogMsg("解析任务:" + imgItem.Des);
                    watch.Start();
                    var data = new DataBmpAlg(_gbl, oriBytes, OddMaskArgb[0], EvenMaskArgb[0], PadLoc);

                    //激活项目的序号
                    var imageIndex = ImgItemInfos.Where(p => p.IsActived).ToList().IndexOf(imgItem);
                    if (ClientRunList.Count != 0 && imageIndex >= 0)
                    {
                        ClientRunList[currentTaskId].FinalList = data.FinalData;
                        ClientRunList[currentTaskId].HeaderList.AddRange(CreateHeadData(imageIndex, data.FinalData.Count, imgItem.Des));
                    }

                    watch.Stop();
                    AddLogMsg("解析完成用时" + watch.ElapsedMilliseconds);
                }


                var img = BitmapImageUtil.CreateBitmapImageWithBys(oriBytes);
                img.Freeze();
                ImgSource = img;
               
                ActiveFn = imgItem.FnPath;

                res = true;
            }
            catch (Exception e)
            {
                AddLogMsg(e.ToString());
            }

            _slim.Release();
            return res;
        }

        private ICommand _imgItemSelectionChangedCmd;
        public ICommand ImgItemSelectionChangedCmd
        {
            get => _imgItemSelectionChangedCmd ?? (_imgItemSelectionChangedCmd = new RelayCommand(delegate (object obj)
            {
                var param = obj as ExCommandParameter;
                if (param?.Parameter is ImgItemInfo info)
                {
                    foreach (var imgItemInfo in ImgItemInfos)
                    {
                        imgItemInfo.ImgOpState = ImgOpState.None;
                    }

                    Task.Factory.StartNew(() =>
                    {
                        PanelUnLock = false;

                        if (info.Des.Contains("bmp") || info.Des.Contains("png"))
                        {
                            ActiveImgItem(info, SelectClientId, false);
                            AddLogMsg("所选择图片信息" + info.Des);
                        }
                        else
                        {
                            ActiveBinItem(info, false);
                            AddLogMsg("所选择文件信息" + info.Des);
                        }

                        PanelUnLock = true;
                    });
                }
            }, pre =>
            {
                return true;
            }));
        }


        private ICommand _openFdClearCmd;
        public ICommand OpenFdClearCmd
        {
            get => _openFdClearCmd ?? (_openFdClearCmd = new RelayCommand(delegate (object obj)
            {
                AddLogMsg("已清除所有文件");
                ImgItemInfos.Clear();

                foreach (var keyValuePair in ClientRunList)
                {
                    keyValuePair.Value.CancelToken.Cancel();
                }
                ProgressData = 0;
                ClientRunList.Clear();

            }, pre =>
            {
                return true;
            }));
        }


        private ICommand _openFdCmd;
        public ICommand OpenFdCmd
        {
            get => _openFdCmd ?? (_openFdCmd = new RelayCommand(delegate (object obj)
            {
                var ofd = new OpenFileDialog()
                {
                    Multiselect = true,
                    Filter = "Images|*.png;*.bmp|Bin|*.bin;*.rpd",
                };
                if (ofd.ShowDialog(Application.Current.MainWindow).HasValue)
                {
                    if (ofd.FilterIndex == 1)
                    {
                        ofd.FileNames.ToList().ForEach(p =>
                        {
                            ImgItemInfos.Add(new ImgItemInfo()
                            {
                                IsActived = true,
                                FnPath = Path.GetFullPath(p),
                                Des = Path.GetFileName(p),
                                Cs = "EE50000000000000"
                            });
                        });
                    }
                    else
                    {
                        ImgItemInfos.Add(new ImgItemInfo()
                        {
                            IsActived = true,
                            FnPath = Path.GetFullPath(ofd.FileName),
                            Des = Path.GetFileName(ofd.FileName),
                            Cs = ""
                        });
                    }
                    

                }
            }, pre =>
            {
                return true;
            }));
        }

        private ICommand _sendItemsCmd;
        public ICommand SendItemsCmd
        {
            get => _sendItemsCmd ?? (_sendItemsCmd = new RelayCommand(delegate (object obj)
            {
                Task.Factory.StartNew(() =>
                {
                    PanelUnLock = false;
                    foreach (var imgItemInfo in ImgItemInfos)
                    {
                        imgItemInfo.ImgOpState = ImgOpState.None;
                    }
                    if (IsEthSim)
                    {
                        ClientRunList.Clear();
                        try
                        {
                            File.Delete(@"test.txt");
                        }
                        catch (Exception)
                        {
                            AddLogMsg("未能成功删除test.txt");
                        }

                        AddLogMsg("测试数据，写出文件：.\\test.txt");
                        ClientRunList.TryAdd(0, new NetClient(null));
                    }

                    if (IsNetWorkSend)
                    {
                        NetWorkSendAll();
                    }
                    else if (IsSerialSend)
                    {
                        VmParam.Pcom?.CloseComm();
                        SerialPcomm();
                        SerialSendAll();
                    }
                    else
                    {
                        AddLogMsg("未选择任何发送协议，请选择后再发");
                    }
                    PanelUnLock = true;
                    GC.Collect();
                });
            }, pre =>
            {
                return true;
            }));
        }


        private ICommand _selectionChangedCmd;
        public ICommand SelectionChangedCmd
        {
            get => _selectionChangedCmd ?? (_selectionChangedCmd = new RelayCommand(delegate (object obj)
            {
                var param = obj as ExCommandParameter;
                if (param?.Parameter is ClientList ins)
                {
                    ClientRunList[ins.ClientId].Sock = Configure.VmParam.ClientList[ins.ClientId];
                    SelectClientId = ins.ClientId;
                    CurrentIp = ClientRunList[ins.ClientId].Sock.Client.RemoteEndPoint.ToString();
                    IsConnected = ViewClients[ins.ClientId].IsConnect;
                }
            }, pre =>
           {
               return true;
           }));
        }
    }
}
