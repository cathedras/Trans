
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
using ElCommon.Util;
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

using RelayCommand = ElCommon.Util.UtilRelayCommand;

namespace TspUtil
{
    public interface IViewModel
    {
        void AddLogMsg(string msg, int level = 0);
        void SaveDataFrame(string msg);
    }

    public class ViewModel : INotifyPropertyChanged, IViewModel
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
        //private string _currentIp = "未选择";

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

        private readonly static ILog _log = LogManager.GetLogger("exlog");

        //clients' attribute
        private readonly List<IDev> _clientRunList = new List<IDev>();
       // static VmParam _vmParam = new VmParam();
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

        public bool HighLowBytesRevert
        {
            get => _highLowBytesRevert;
            set
            {
                if (value == _highLowBytesRevert) return;
                _highLowBytesRevert = value;
                _gbl.HighLowBytesRevert = value;
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

        //public static VmParam VmParam
        //{
        //    get => _vmParam;
        //    set => _vmParam = value;
        //}

        public List<IDev> ClientRunList
        {
            get => _clientRunList;
        }
        //public string CurrentIp
        //{
        //    get => _currentIp;
        //    set
        //    {
        //        if (value == _currentIp) return;
        //        _currentIp = value;
        //        OnPropertyChanged();
        //    }
        //}

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
        //public IPAddress IpAddress
        //{
        //    get => VmParam.IpAddress;
        //    set
        //    {
        //        VmParam.IpAddress = value;
        //        OnPropertyChanged();
        //    }
        //}

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value == _isConnected) return;
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (value == _port) return;
                _port = value;
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

        /// <summary>
        /// 命令头中加入图片分辨率信息
        /// </summary>
        public bool IsAddSizeToHeader
        {
            get => _isAddSizeToHeader;
            set
            {
                if (value == _isAddSizeToHeader) return;
                _isAddSizeToHeader = value;
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
                    if (level > 0)
                    {
                        _log.Info(msg);
                    }
                    else
                    {
                        _log.Debug(msg);
                    }
                }
                else
                {
                    dispatcher.Invoke(() => { AddLogMsg(msg, level); });
                }
            }
        }

        public void SaveDataFrame(string msg)
        {
            _simp.WriteLogFile(msg);
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
                _gbl.IsAddSizeToHeader = IsAddSizeToHeader;
                _gbl.Save(_cfg, typeof(Gbl));
            }
            catch (Exception e)
            {
                AddLogMsg(e.ToString(), 1);
            }
        }

        /// <summary>
        /// Clients' send method with 1024
        /// </summary>
        /*private bool DataSendFrame(byte[] buffer, IDev dev, int sendTimeOut)
        {
            ManualResetEvent revResetEvent = dev.ResetEvent;

            bool isFrameSendSuccess = false;
            try
            {
                var count = 1;

                while (count < _maxRetryCount && !isFrameSendSuccess)
                {
                    AddLogMsg($"Send Data -> ...");
                    revResetEvent.Reset();
                    dev.IsReceiveNg = false;
                    if (IsEthSim && IsNetWorkSend)
                    {
                        Thread.Sleep(10);
                        revResetEvent.Set();
                    }
                    else if (IsNetWorkSend)
                    {
                       Configure.VmParam.ReceivingString.Clear();
                       ClientRunList[currentSockId]?.Send(buffer, 0);
                    }
                    else if (IsSerialSend)
                    {
                        _serial?.Send(buffer, 0); 
                    }
                    if (!revResetEvent.WaitOne(sendTimeOut))
                    {
                        AddLogMsg("Timeout for DataSend.", 1);
                        break;
                    }

                    if (IsNetWorkSend) // Ethnet
                    {
                        if (!ClientRunList[currentSockId].IsReceiveNg)
                        {
                            isFrameSendSuccess = true;
                        }
                        else
                        {
                            AddLogMsg($"RESEND DATA: -> {count}/{_maxRetryCount} Count(s)");
                        }
                    }
                    count++;
                }
            }
            catch (Exception x)
            {
                AddLogMsg(x.Message, 1);
            }
            return isFrameSendSuccess;
        }
        */
        /// <summary>
        /// 按Page = 4 * size，Block = 64 Page  Block进行错误重试数据, 不足size时，需要填充数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool DataSendWithBlockRetry(byte[] data, int size, IDev dev, bool fillUp, Action<IDev, double> prog)
        {
            int length = (data.Length % size == 0)
                ? data.Length / size
                : (data.Length - data.Length % size) / size + 1;
            bool sendState = true;
            dev.SendLength = 0;

            while (dev.SendLength < length)
            {
                var i = dev.SendLength;

                var ct = dev.CancelToken.Token;
                if (ct.IsCancellationRequested)
                {
                    sendState = false;
                    break;
                }

                var dataBuffer = new byte[size];
                var curDataLen = size;
                if (data.Length < (i + 1) * size)
                {
                    //byte[] newSend = new byte[data.Length - (i) * size];
                    curDataLen = data.Length - i * size;
                    dataBuffer = new byte[size];
                    if (!fillUp)
                    {
                        dataBuffer = new byte[curDataLen];
                    }
                }

                Array.Copy(data, i * size, dataBuffer, 0, curDataLen);
                if (!dev.DataSendFrame(dataBuffer, 0))
                {
                    AddLogMsg("数据发送失败", 1);
                    prog?.Invoke(dev, 0);
                    sendState = false;
                    break;
                }
                else if (!dev.IsReceiveNg)
                {
                    dev.SendLength ++;
                }
                else //IsReceiveNg
                {
                    dev.SendLength = i / (4 * 64);
                    AddLogMsg($"数据发送成功，返回NG， 重新调整到 {dev.SendLength} 进行发送.", 1);
                }

                if (prog != null)
                {
                    double strike = ((double) i / length) * Maximum;
                    if (i == length - 1)
                    {
                        strike = Maximum;
                    }
                    prog(dev, strike);
                }
            }

            return sendState;
        }
        /// <summary>
        /// bin file send by serial for every one frame
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        /*
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
                sendState = DataSendFrame(dataBuffer, 0);
                if (!sendState)
                {
                    AddLogMsg("本帧数据未能成功发送，请重新开始发送", 1);
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
        */
        /// <summary>
        /// Send all bmpphoto by network. 
        /// </summary>
        public void NetWorkSendAll()
        {
            byte[] erase = StringToByteArray("erase");//清除指令
            byte[] store = StringToByteArray("store");//保存指令
            if (ClientRunList.Any())
            {
                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                {
                   // if (!DataSendFrame(erase, currentSockId, GblInfo.LongTimeoutForElapsed))
                    if (!pair.DataSendFrame(erase, 0, GblInfo.LongTimeoutForElapsed))
                    {
                        AddLogMsg($"erase指令发送失败, {pair}", 1);
                    }
                    else
                    {
                        if (ImgItemInfos.Any(p => p.IsActived))
                        {
                            for (int i = 0; i < ImgItemInfos.Count; i++)
                            {
                                if (ImgItemInfos[i].IsActived)
                                {
                                    foreach (var dev in ClientRunList)
                                    {
                                        dev.HeaderList = new List<byte>();
                                        dev.FinalList = new List<byte>();
                                        dev.LmgLenCount = 0;
                                    }

                                    if (!ActiveImgItem(ImgItemInfos[i], pair))
                                    {
                                        AddLogMsg("解析失败！", 1);
                                    }
                                    //data analyze
                                    else
                                    {
                                        var sw = new Stopwatch();
                                        sw.Start();

                                        var frameLen = 1024;
                                        if (IsEthSim)
                                        {
                                            _simp.WriteSperator();
                                            _simp.WriteLogFile($"IMAGE DATA --> HL: {pair.HeaderList.Count} , DL: {pair.FinalList.Count}      {ImgItemInfos[i].Des}");
                                            _simp.WriteSperator();
                                            frameLen = _gbl.SimFrameLen;
                                        }

                                        if (!pair.DataSendFrame(pair.HeaderList.ToArray(), pair.HeaderList.Count))
                                        {
                                            AddLogMsg("图片头发送失败，请重新发送", 1);
                                        }
                                        else if (!DataSendWithBlockRetry(pair.FinalList.ToArray(), frameLen, pair, true, (a, b) =>
                                        {
                                            ProgressData = b; //更新进度条
                                        }))
                                        {
                                            AddLogMsg("图片数据发送失败，请重新发送", 1);
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
                                            if (!pair.DataSendFrame(send, send.Length))
                                            {
                                                AddLogMsg("pioff发送失败，结尾发送失败请重新发送", 1);
                                            }
                                            else
                                            {
                                                Thread.Sleep(1000);
                                                AddLogMsg($"图片{ImgItemInfos[i].Des} 发送完成,总共发送字节数：{pair.LmgLenCount}, 耗时 {sw.ElapsedMilliseconds:D6}ms");
                                                ImgItemInfos[i].ImgOpState = ImgOpState.Success;
                                            }
                                        }

                                        sw.Stop();
                                    }

                                    if (ImgItemInfos[i].ImgOpState == ImgOpState.None)
                                        ImgItemInfos[i].ImgOpState = ImgOpState.Fail;
                                }
                            }

                            if (!pair.DataSendFrame(store, 0))
                            {
                                AddLogMsg("指令发送失败，请重新发送", 1);
                            }
                        }
                        else
                        {
                            AddLogMsg("无图片被选中，无法解析", 1);
                        }
                    }
                }));

                Task.WaitAll(tsks.ToArray());
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
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
                        if (!_serial.DataSendFrame(SerialDev.StartBytes, 0))
                        {
                            AddLogMsg("未能成功发送命令", 1);
                        }
                        else if (!_serial.DataSendFrame(_serial.FinalList.ToArray(), 258))
                        {
                            AddLogMsg("未能成功发送数据", 1);
                        }
                        else if (!_serial.DataSendFrame(SerialDev.EndBytes, 0))
                        {
                            AddLogMsg("未发送完成数据结束标志", 1);
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
                        AddLogMsg("解析失败！", 1);
                    }
                }));
            }
            else
            {
                AddLogMsg("无bin文件选择，请打开一个bin文件 或者 无com口连接", 1);
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
        public byte[] CreateHeadData(int picIndex, int size, int w, int h, string name = "")
        {
            List<byte> head = new List<byte>();
            head.AddRange(StringToByteArray("pic"));
            head.Add(Convert.ToByte(picIndex));//4
            head.AddRange(StringToByteArray("BM"));
            head.AddRange(BitConverter.GetBytes(size));//get the picture's data length
            if (IsAddSizeToHeader)
            {
                head.AddRange(BitConverter.GetBytes((ushort)w));
                head.AddRange(BitConverter.GetBytes((ushort)h));
            }
            head.AddRange(StringToByteArray(name));
            return head.ToArray();
        }

        private readonly string _xmlCfgV2 = @"..\cfgv2.xml";
        public ViewModel()
        {
            SwVersion = "1.0.1";
#if DEBUG
            SwVersion = "0.0.0";
#endif
            _gbl = new Gbl();
            _gbl.LoadGbl<Gbl>(_cfg);

            //PlainXmlDb db = new PlainXmlDb(_xmlCfgV2);
            //db.SaveObjToDb("GblV2", _gbl);
            //db.FlushToDb();

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

            Port = _gbl.Port;
            IsInverse = _gbl.IsInverse;
            UsinSimData = _gbl.UsingSimData;
            OddOffset = _gbl.OddOffset.ToString();
            EvenOffset = _gbl.EvenOffset.ToString();
            IsSerialSend = _gbl.IsSerialSend;
            IsNetWorkSend = _gbl.IsNetWorkSend;
            IsAddSizeToHeader = _gbl.IsAddSizeToHeader;
            HighLowBytesRevert = _gbl.HighLowBytesRevert;

            OddMaskArgb.Add(new MaskForArgbItem(_gbl.OddRgbA));
            EvenMaskArgb.Add(new MaskForArgbItem(_gbl.EvenRgbA));

        }

        /// <summary>
        /// network connection lisenning
        /// </summary>
        private void Connections()
        {
            if (IsEthSim)
            {
                var dev = new NetVirDevice(this);
                ClientRunList.Add(dev);
                ViewClients.Add(new ClientList(dev, 0, "Vir Eth Dev"));
            }
            else
            {
                new Task(new Action(delegate()
                {
                    while (true)
                    {
                        if (IsNetWorkSend && IsConnected)
                        {
                            SockListen();
                        }
                        Thread.Sleep(10);
                    }
                })).Start();
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
                AddLogMsg("网络客户端获取错误：" + ex.ToString(), 1);
            }
            return ipaddress;
        }

        /// <summary>
        /// open a task for listenning the background network connection
        /// </summary>
        private void SockListen()
        {
            DevInitUtil.Listen(GblInfo, out var tcpClient00);

            if (tcpClient00 != null)
            {
                var dev = new NetDev(tcpClient00, this);
                ClientRunList.Add(dev);
                var clientIp = tcpClient00.Client.RemoteEndPoint.ToString();

                AddLogMsg("客户端" + clientIp + "已连接到本服务器");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewClients.Add(new ClientList(dev, ViewClients.Count, clientIp));
                });
                //client
                Task.Factory.StartNew((obj) =>
                {
                    var d = (IDev) obj;
                    var revData = new List<byte>();
                    while (ClientRunList.Contains(d))
                    {
                        try
                        {
                            var data = d.Receive();
                            if (data.Any())
                            {
                                revData.AddRange(data);
                                var tmpReceivingString = new List<string>();
                                tmpReceivingString.Add(Encoding.ASCII.GetString(revData.ToArray()));
                                revData.Clear();

                                //AddLogMsg($"REV <-- {string.Join("", Array.ConvertAll(tmpReceivingString.ToArray(), p => p))}");

                                if (tmpReceivingString.Exists(p => NetDev.picok.IsMatch(p)))
                                {
                                    d.ResetEvent.Set();
                                }
                                else if (tmpReceivingString.Exists(p => NetDev.picNG.IsMatch(p)))
                                {
                                    d.IsReceiveNg = true;
                                    d.ResetEvent.Set();
                                }
                            }

                            Thread.Sleep(1);
                        }
                        catch (Exception e)
                        {
                            AddLogMsg($"Rev ERR: {e.Message}", 1);
                            ClientRunList.Remove(d);
                            for (var i = 0; i < ViewClients.Count; i++)
                            {
                                if (ViewClients[i].Dev == d)
                                {
                                    ViewClients[i].IsOffLine = true;
                                }
                            }
                        }
                    }
                }, dev);
            }              
        }

        private Pcomm _pcomm;

        IDev _serial = null;
        /// <summary>
        /// serial pcomm get
        /// </summary>
        private void SerialPcomm()
        {
            if (_serial == null)
            {
                _pcomm?.CloseComm();

                DevInitUtil.SerialConfig(GblInfo, out _pcomm);
                _serial = new SerialDev(_pcomm, this);

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
                        if (lst.Count != 0 && lst[0] == SerialDev.receiveOk)
                        {
                            lst.Clear();
                            _serial.ResetEvent.Set();
                        }
                    }
                }), _serial.CancelToken.Token);
            }
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
                    AddLogMsg("没有serial对象", 1);
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
        private bool ActiveImgItem(ImgItemInfo imgItem, IDev dev)
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
                AddLogMsg($"Fail to delete {_usingFileName}, MSG = {e.Message}", 1);
            }

            try
            {
                byte[] oriBytes = null;
                var pic = Image.FromFile(imgItem.FnPath);
                pic.Save(_usingFileName, ImageFormat.Bmp);
                var w = pic.Width;
                var h = pic.Height;
                pic.Dispose();

                var fs = new FileStream(_usingFileName, FileMode.Open);
                oriBytes = new byte[fs.Length];
                fs.Read(oriBytes, 0, oriBytes.Length);
                fs.Close();

                if (dev!= null)
                {
                    //start monitors the picture analyze task
                    Stopwatch watch = new Stopwatch();
                    AddLogMsg("解析任务:" + imgItem.Des);
                    watch.Start();
                    var data = new DataBmpAlg(_gbl, oriBytes, OddMaskArgb[0], EvenMaskArgb[0], PadLoc);

                    //激活项目的序号
                    var imageIndex = ImgItemInfos.Where(p => p.IsActived).ToList().IndexOf(imgItem);
                    if (imageIndex >= 0)
                    {
                        var lstdata = data.FinalData;
                        if (HighLowBytesRevert)
                        {
                           lstdata = data.HighLowBytesRevert(lstdata);
                        }

                        dev.FinalList = lstdata;
                        dev.HeaderList.AddRange(
                            CreateHeadData(imageIndex, data.FinalData.Count, w, h, imgItem.Des));

                        AddLogMsg($"数据RawData 长度： {data.FinalData.Count}, 分辨 ： {w}, {h}", 1);
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
                            ActiveImgItem(info, null);
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
                ImgItemInfos.Clear();
                AddLogMsg("已清除所有文件");
                ProgressData = 0;
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
        private bool _isConnected;
        private bool _isAddSizeToHeader;

        private readonly TxtSimpLog _simp = new TxtSimpLog(Encoding.ASCII);
        private int _port;
        private bool _highLowBytesRevert;

        public ICommand SendItemsCmd
        {
            get => _sendItemsCmd ?? (_sendItemsCmd = new RelayCommand(delegate (object obj)
            {
                LogItems.Clear();

                ProgressData = 0;
                AddLogMsg($"开始数据发送...");
                Task.Factory.StartNew(() =>
                {
                    PanelUnLock = false;
                    foreach (var imgItemInfo in ImgItemInfos)
                    {
                        imgItemInfo.ImgOpState = ImgOpState.None;
                    }
                    if (IsEthSim)
                    {
                        var dirName = "..\\tmpdata";
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        _simp.InitSimpLogDir(dirName);
                        _simp.FileName = $"rawdata_{DateTime.Now:HHmmtt}.txt";
                        AddLogMsg($"测试数据，写出文件. {_simp.FileName}");
                        //TODO:
                    }

                    if (IsNetWorkSend)
                    {
                        NetWorkSendAll();
                    }
                    else if (IsSerialSend)
                    {
                        SerialPcomm();
                        SerialSendAll();
                    }
                    else
                    {
                        AddLogMsg("未选择任何发送协议，请选择后再发", 1);
                    }
                    PanelUnLock = true;
                    GC.Collect();
                });
            }, pre =>
            {
                return true;
            }));
        }

        /*
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
        */
    }
}
