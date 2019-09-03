using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ElCommon.Util;
using GalaSoft.MvvmLight;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using log4net;
using Microsoft.Win32;
using TspUtil.Annotations;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace TspUtil
{
    public class MianViewModel :ViewModelBase,INotifyPropertyChanged, IViewModel
    {
        #region fields
        //base attribute
        private ObservableCollection<LogItem> _logItems;
        private ObservableCollection<ImgItemInfo> _imgItemInfos;
        private ObservableCollection<ImgItemInfo> _programmeFiles;
        private ObservableCollection<ClientList> _viewClients;

        private readonly string _cfg = @"..\tspUtil.ini";
        private readonly string _usingFileName = "_tmpUsing.bmp";
        private string _activeFn;
        private readonly Gbl _gbl;
        private bool _isEthSim;
        private string _rdFile;
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
        private string _currentChooseFile = string.Empty;
        private bool _isCrossData = false;
        private bool _isInferiorData = false;
        private string _regsTxt = string.Empty;
        private readonly static ILog _log = LogManager.GetLogger("exlog");
        //clients' attribute
        private readonly List<IDev> _clientRunList = new List<IDev>();
        private int _selectClientId = 0;
        private bool _usinSimData = false;
        private bool _usingNoBoundLst = false;
        bool _isServerConnected = false;
        bool _isCmdRun = false;
        private bool isChooseDisplay;
        private int _fontSize;

        private bool _isPictureReArang;
        //programme tab attribute
        // private bool _isProgramNetSend = false;
        private Dictionary<string, AlgCmd> _algMap;
        private string _curProgmFile;
        private string _lastSaveProgmFile;
        private bool _enableBtn;

        private int _wantToExeCount;
        //Operator
        private string _iovccParam = string.Empty;
        private string _vspParam = string.Empty;
        private string _resxParam = string.Empty;
        private string _brightParam = string.Empty;

        private string _pllParam = string.Empty;

        private string _regb6Param = string.Empty;
        private string _regd6Param = string.Empty;
        private string _regdeParam = string.Empty;

        private string _panelb6Param = string.Empty;

        private string _vsaParam = string.Empty;
        private string _hasParam = string.Empty;
        private string _vfpParam = string.Empty;
        private string _hfpParam = string.Empty;
        private string _vbpParam = string.Empty;
        private string _hbpParam = string.Empty;
        private string _hactParam = string.Empty;
        private string _vactParam = string.Empty;

        private bool _isAllComp = false;
        private int _fileIndex = 0;
        private LayoutDocumentPane _pane;
        private bool _fileManaEnable = true;
        private bool _normal = true;
        private bool _compress = false;
        #endregion fields


        #region properties
        public bool Normal
        {
            get => _normal;
            set
            {
                if (value== _normal) return;
                _normal = value;
                RaisePropertyChanged();
            }
        }
        public bool Compress
        {
            get => _compress;
            set
            {
                if (value == _compress) return;
                _compress = value;
                RaisePropertyChanged();
            }
        }
        public Dictionary<string, AlgCmd> AlgMap
        {
            get => _algMap;
        }
        public bool FileManaEnable
        {
            get { return _fileManaEnable; }
            set
            {
                if(value == _fileManaEnable)return;
                _fileManaEnable = value;
                RaisePropertyChanged();
            }
        }
        public LayoutDocumentPane Pane
        {
            get { return _pane;}
            set
            {
                if(value == _pane)return;
                _pane = value;
            }
        }
        public int FileIndex
        {
            get { return _fileIndex; }
            set
            {
                if(value==_fileIndex)return;
                _fileIndex = value; 
                RaisePropertyChanged();
            }
        }
        public int WantToExeCount
        {
            get { return _wantToExeCount; }
            set
            {
                if (value == _wantToExeCount) return;
                _wantToExeCount = value;
                _gbl.WantToExeCount = value;
                RaisePropertyChanged();
            }
        }

       
        public bool IsAllComp
        {
            get { return _isAllComp; }
            set
            {
                if (value== _isAllComp) return;
                _isAllComp = value;
                _gbl.IsAllComp = value;
                RaisePropertyChanged();
            }
        }


        public string IovccParam
        {
            get => _iovccParam;
            set
            {
                if (value == _iovccParam) return;
                _iovccParam = value;
                RaisePropertyChanged();
            }
        }
        public string VspParam
        {
            get => _vspParam;
            set
            {
                if (value == _vspParam) return;
                _vspParam = value;
                RaisePropertyChanged();
            }
        }
        public string ResxParam
        {
            get => _resxParam;
            set
            {
                if (value == _resxParam) return;
                _resxParam = value;
                RaisePropertyChanged();
            }
        }
        public string BrightParam
        {
            get => _brightParam;
            set
            {
                if (value == _brightParam) return;
                _brightParam = value;
                _gbl.Bright = value;
                RaisePropertyChanged();
            }
        }

        public string PllParam
        {
            get => _pllParam;
            set
            {
                if (value == _pllParam) return;
                _pllParam = value;
                RaisePropertyChanged();
            }
        }

        public string Regb6Param
        {
            get => _regb6Param;
            set
            {
                if (value == _regb6Param) return;
                _regb6Param = value;
                _gbl.Regb6 = value;
                RaisePropertyChanged();
            }
        }
        public string Regd6Param
        {
            get => _regd6Param;
            set
            {
                if (value == _regd6Param) return;
                _regd6Param = value;
                RaisePropertyChanged();
            }
        }
        public string RegdeParam
        {
            get => _regdeParam;
            set
            {
                if (value == _regdeParam) return;
                _regdeParam = value;
                RaisePropertyChanged();
            }
        }

        public string Panelb6Param
        {
            get => _panelb6Param;
            set
            {
                if (value == _panelb6Param) return;
                _panelb6Param = value;
                _gbl.Panelb6 = value;
                RaisePropertyChanged();
            }
        }

        public string VsaParam
        {
            get => _vsaParam;
            set
            {
                if (value == _vsaParam) return;
                _vsaParam = value;
                RaisePropertyChanged();
            }
        }
        public string HasParam
        {
            get => _hasParam;
            set
            {
                if (value == _hasParam) return;
                _hasParam = value;
                RaisePropertyChanged();
            }
        }
        public string VfpParam
        {
            get => _vfpParam;
            set
            {
                if (value == _vfpParam) return;
                _vfpParam = value;
                RaisePropertyChanged();
            }
        }
        public string HfpParam
        {
            get => _hfpParam;
            set
            {
                if (value == _hfpParam) return;
                _hfpParam = value;
                RaisePropertyChanged();
            }
        }
        public string VbpParam
        {
            get => _vbpParam;
            set
            {
                if (value == _vbpParam) return;
                _vbpParam = value;
                RaisePropertyChanged();
            }
        }
        public string HbpParam
        {
            get => _hbpParam;
            set
            {
                if (value == _hbpParam) return;
                _hbpParam = value;
                RaisePropertyChanged();
            }
        }
        public string HactParam
        {
            get => _hactParam;
            set
            {
                if (value == _hactParam) return;
                _hactParam = value;
                RaisePropertyChanged();
            }
        }
        public string VactParam
        {
            get => _vactParam;
            set
            {
                if (value == _vactParam) return;
                _vactParam = value;
                RaisePropertyChanged();
            }
        }


        public bool EnableBtn
        {
            get => _enableBtn;
            set
            {
                if (value == _enableBtn) return;
                _enableBtn = value;
                RaisePropertyChanged();
            }
        }
        public string CurProgmFile
        {
            get => _curProgmFile;
            set
            {
                if (value == _curProgmFile) return;
                _curProgmFile = value;
                RaisePropertyChanged();
            }
        }
        public string LastSaveProgmFile
        {
            get=> _lastSaveProgmFile; 
            set
            {
                if (value==_lastSaveProgmFile)return;
                _lastSaveProgmFile = value;
            }
        }
       
        public int FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize == value) return;
                _fontSize = value;
                _gbl.FontSize = value;
                RaisePropertyChanged();
            }
        }
        public bool IsChooseDisplay
        {
            get => isChooseDisplay;
            set
            {
                if (value == isChooseDisplay) return;
                isChooseDisplay = value;
                _gbl.IsChooseDisplay = value;
                RaisePropertyChanged();
            }
        }
        public bool IsPictureReArang
        {
            get => _isPictureReArang;
            set
            {
                if(value==_isPictureReArang)return;
                _isPictureReArang = value;
                _gbl.IsPicReArrange = value;
                RaisePropertyChanged();
            }
        }
        public string RegsTxt
        {
            get => _regsTxt;
            set
            {
                if (value == _regsTxt) return;
                _gbl.InferiorTxt = value;
                _regsTxt = value.ToLower();
                RaisePropertyChanged();
            }
        }

        public Gbl Gbl => _gbl;
        public bool IsCmdRun
        {
            get => _isCmdRun;
            set
            {
                if (value == _isCmdRun) return;
                _isCmdRun = value;
            }
        }
        
        public bool IsServerConnected
        {
            get => _isServerConnected;
            set
            {
                if (value == _isServerConnected) return;
                _isServerConnected = value;
                RaisePropertyChanged();
            }
        }
        public bool IsSerialSend
        {
            get => _isSerialSend;
            set
            {
                if (value == _isSerialSend) return;
                _isSerialSend = value;
                _gbl.IsSerialSend = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        public bool IsInferiorData
        {
            get => _isInferiorData;
            set
            {
                if (value == _isInferiorData) return;
                _isInferiorData = value;
                _gbl.IsInferiorData = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        public bool IsCrossData
        {
            get => _isCrossData;
            set
            {
                if (value == _isCrossData) return;
                _isCrossData = value;
                _gbl.IsCrossData = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
        //        RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        public bool UsingNoBoundLst
        {
            get => _usingNoBoundLst;
            set
            {
                if (_usingNoBoundLst == value) return;
                _usingNoBoundLst = value;
                _gbl.UsingNoBoundLst = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<ImgItemInfo> ProgrammeFiles
        {
            get => _programmeFiles ?? (_programmeFiles = new ObservableCollection<ImgItemInfo>());
        }
        public ObservableCollection<ImgItemInfo> ImgItemInfos
        {
            get => _imgItemInfos ?? (_imgItemInfos = new ObservableCollection<ImgItemInfo>());
        }
        public string CurrentChooseFile
        {
            get => _currentChooseFile;
            set => _currentChooseFile = value;
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
        //        RaisePropertyChanged();
        //    }
        //}

        public bool IsClientConnected
        {
            get => _isClientConnected;
            set
            {
                if (value == _isClientConnected) return;
                _isClientConnected = value;
                RaisePropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (value == _port) return;
                _port = value;
                RaisePropertyChanged();
            }
        }
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
            }
        }

        public bool PanelUnLock
        {
            get => _panelUnLock;
            set
            {
                if (value == _panelUnLock) return;
                _panelUnLock = value;
                RaisePropertyChanged();
            }
        }


        public string SwVersion
        {
            get => _swVersion;
            set
            {
                if (value == _swVersion) return;
                _swVersion = value;
                RaisePropertyChanged();
            }
        }
        public string ActiveFn
        {
            get => _activeFn;
            set
            {
                if (value == _activeFn) return;
                _activeFn = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
        public double ProgressData
        {
            get => _progressData;
            set
            {
                if (value == _progressData) return;
                _progressData = value;
                RaisePropertyChanged();
            }
        }
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (value == _maximum) return;
                _maximum = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
        #endregion properties
        /// <summary>
        /// Add log to window with level index
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void AddLogMsg(string msg, int level = 0)
        {
            if (!IsCmdRun)
            {
                var dispatcher = Application.Current?.Dispatcher;
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
                        dispatcher.Invoke(() =>
                        {
                            AddLogMsg(msg, level);
                        });

                    }
                }
            }
            else
            {
                ConsoleDebug.AllocConsole();
                ConsoleDebug.WriteLine(msg, ConsoleColor.Blue);
                _log.Debug(msg);
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
                //ProgrammeSaveFile.Execute(CurProgmFile);
            }
            catch (Exception e)
            {
                AddLogMsg(e.ToString(), 1);
            }
        }


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
                if (!dev.DataSendFrame(dataBuffer, 0, 2 * 1000))
                {
                    AddLogMsg("数据发送失败", 1);
                    prog?.Invoke(dev, 0);
                    sendState = false;
                    break;
                }
                else if (!dev.IsReceiveNg)
                {
                    dev.SendLength++;
                }
                else //IsReceiveNg
                {
                    dev.SendLength = i / _gbl.FrameRetrySize;//(4 * 64);
                    AddLogMsg($"数据发送成功，返回NG， 重新调整到 {dev.SendLength} 进行发送.", 1);
                }

                if (prog != null)
                {
                    double strike = ((double)i / length) * Maximum;
                    if (i == length - 1)
                    {
                        strike = Maximum;
                    }
                    prog(dev, strike);
                }
            }

            return sendState;
        }

        private bool DataSendWithBlockRetry(byte[] data, int size, IDev dev, bool fillUp)
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
                if (!dev.DataSendFrame(dataBuffer, 0, 2 * 1000))
                {
                    sendState = false;
                    break;
                }
                else if (!dev.IsReceiveNg)
                {
                    dev.SendLength++;
                }
                else //IsReceiveNg
                {
                    dev.SendLength = i / _gbl.FrameRetrySize;//(4 * 64);
                }
            }

            return sendState;
        }
        /// <summary>
        /// 
        /// </summary>
        public void CmdNetWorkSend()
        {
            byte[] erase = StringToByteArray("ddrstop");//DDR停止命令
            byte[] show = StringToByteArray("show");
            byte[] poweron = StringToByteArray("poweron");
            if (ClientRunList.Any())
            {
                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                {
                    if (pair.DataSendFrame(poweron, poweron.Length, _gbl.LongTimeoutForElapsed))
                    {
                        AddLogMsg("发送power on命令", 1);
                    }
                    // if (!DataSendFrame(erase, currentSockId, GblInfo.LongTimeoutForElapsed))
                    if (!pair.DataSendFrame(erase, 0, GblInfo.LongTimeoutForElapsed))
                    {
                        AddLogMsg($"ddrstop指令发送失败, {pair}", 1);
                    }
                    else
                    {
                        for (int i = 0; i < ImgItemInfos.Count; i++)
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
                                //PanelUnLock = true;
                            }
                            //data analyze
                            else
                            {
                                var sw = new Stopwatch();
                                sw.Start();

                                var frameLen = _gbl.FrameLen;
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
                                else if (!DataSendWithBlockRetry(pair.FinalList.ToArray(), frameLen, pair, true))
                                {
                                    AddLogMsg("图片数据发送失败，请重新发送", 1);
                                }
                                else
                                {



                                    //List<byte> Checksum = new List<byte>();
                                    //Checksum.AddRange(DataBmpAlg.HexStringToByteArray(ImgItemInfos[i].Cs));
                                    // Array.Copy(Checksum.ToArray(), 0, send, picoff.Length, Checksum.ToArray().Length);
                                    if (pair.DataSendFrame(poweron, poweron.Length, _gbl.LongTimeoutForElapsed))
                                    {
                                        AddLogMsg("发送power on命令", 1);
                                    }
                                    if (!pair.DataSendFrame(show, show.Length, _gbl.LongTimeoutForElapsed))
                                    {
                                        AddLogMsg("发送show命令", 1);
                                    }
                                    else
                                    {
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
                }));

                Task.WaitAll(tsks.ToArray());
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
            }
        }
        public void CmdPowerOn()
        {
            var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
            {
                byte[] poweron = StringToByteArray("poweron");
                pair.DataSendFrame(poweron, poweron.Length, _gbl.LongTimeoutForElapsed);
            }));
        }
        /// <summary>
        /// Send all bmpphoto by network. 
        /// </summary>
        public void NetWorkSendAll()
        {
            byte[] erase = StringToByteArray("erase");//清除指令
            byte[] store = StringToByteArray("store");//保存指令
            if (ClientRunList.Any())
            {
                _selectPic = false;
                _isPowerOn = false;
                _isPowerff = false;
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
                                        PanelUnLock = true;
                                    }
                                    //data analyze
                                    else
                                    {
                                        var sw = new Stopwatch();
                                        sw.Start();

                                        var frameLen = _gbl.FrameLen;
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
                                            if (!pair.DataSendFrame(send, send.Length, _gbl.LongTimeoutForElapsed))
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

                            if (!pair.DataSendFrame(store, 0, _gbl.LongTimeoutForElapsed))
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
        public byte[] CreateDDRHeadData(int picIndex, int size, int w, int h, string name = "")
        {
            List<byte> head = new List<byte>();
            head.AddRange(StringToByteArray("ddr"));
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
        public MianViewModel()
        {
            SwVersion = "1.1.0";
#if DEBUG
            SwVersion = "0.0.0";
#endif
#if custom

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
            Address = _gbl.RemoteIpAddress;
            _gbl.LocalIpAddress = LocalIPAddress();

            Port = _gbl.Port;
            IsInverse = _gbl.IsInverse;
            UsinSimData = _gbl.UsingSimData;
            UsingNoBoundLst = _gbl.UsingNoBoundLst;
            OddOffset = _gbl.OddOffset.ToString();
            EvenOffset = _gbl.EvenOffset.ToString();
            IsSerialSend = _gbl.IsSerialSend;
            IsNetWorkSend = _gbl.IsNetWorkSend;
            IsAddSizeToHeader = _gbl.IsAddSizeToHeader;
            HighLowBytesRevert = _gbl.HighLowBytesRevert;
            IsCrossData = _gbl.IsCrossData;
            IsInferiorData = _gbl.IsInferiorData;
            RegsTxt = _gbl.InferiorTxt.ToUpper();
            IsChooseDisplay = _gbl.IsChooseDisplay;
            IsPictureReArang = _gbl.IsPicReArrange;
            WantToExeCount = _gbl.WantToExeCount;
            OddMaskArgb.Add(new MaskForArgbItem(_gbl.OddRgbA));
            EvenMaskArgb.Add(new MaskForArgbItem(_gbl.EvenRgbA));
            if (File.Exists(_gbl.FileListXml))
            {
                FromFileRead(_gbl.FileListXml);
            }
            FontSize = _gbl.FontSize;
            _algMap = ProgrammeUtil.InitCmdMap();
            if (IsNetWorkSend)
            {
                LoadFromWorkDir();
            }
            EnableBtn = true;
        }
        private void LoadFromWorkDir()
        {
            if (Directory.Exists(_gbl.ProgmWorkDir))
            {
                var files = Directory.EnumerateFiles(_gbl.ProgmWorkDir);
                foreach(var fil in files)
                {
                    ProgrammeFiles.Add(new ImgItemUi()
                    {
                        Des = Path.GetFileName(fil),
                        FnPath = Path.GetFullPath(fil),
                        IsActived = false,
                    });
                }
            }
            else
            {
                Directory.CreateDirectory(_gbl.ProgmWorkDir);
                AddLogMsg($"自动创建编程工作目录:{_gbl.ProgmWorkDir},请存放文件");
            }
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
                new Task(new Action(delegate ()
                {
                    while (true && !IsCmdRun)
                    {
                        if (IsNetWorkSend && IsClientConnected)
                        {
                            SockListen();
                        }
                        Thread.Sleep(10);
                    }
                })).Start();
            }
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
            for (int i = 0; i < speed.Length; i++)
            {
                SerialSpeed.Add(new ItemSourceItemInfo()
                {
                    Name = speed[i],
                    Des = speed[i],
                    Val = speed[i],
                });
            }
        }



        public void SaveFileOpenList(string filePath)//
        {
            //var file = File.Open($"FileInfoItem.xml", FileMode.OpenOrCreate);
            var pXml = new PlainXmlDb(filePath);
            var list = new List<SaveFileList>();
            int i = 1;
            foreach (var var in ImgItemInfos)
            {
                list.Add(new SaveFileList()
                {
                    IsActived = var.IsActived,
                    Cs = var.Cs,
                    Des = var.Des,
                    FnPath = var.FnPath,
                    _index = i++,
                });

            }
            pXml.SaveObjListToDb("File", list);
            pXml.FlushToDb();
        }

        public void XmlDelete(string filePath)
        {
            File.Delete(filePath);
        }


        public void FromFileRead(string filePath)
        {
            var pXml = new PlainXmlDb(filePath);
            var allValue = new List<SaveFileList>();
            pXml.LoadObjListFromDb("File", ref allValue);
            foreach (var va in allValue)
            {
                if (!File.Exists(va.FnPath))
                {
                    break;
                }
                ImgItemInfos.Add(new ImgItemUi()
                {
                    IsActived = va.IsActived,
                    Cs = va.Cs,
                    Des = va.Des,
                    FnPath = va.FnPath,
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
        /// 连接服务器
        /// </summary>
        public void SockConnect()
        {
            DevInitUtil.ConnectToServer(GblInfo, out var server);

            if (server != null)
            {
                var dev = App.Locator.ClientDev;
                dev.Sock = server;
                dev.InitParam();
                IsServerConnected = true;
                ClientRunList.Add(dev);
                AddLogMsg($"已成功连接到{Port}端口上的服务器{Address}");
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    ViewClients.Add(new ClientList(dev, ViewClients.Count, Address));
                //});
                ViewClients.Add(new ClientList(dev, ViewClients.Count, Address));
                new Thread(obj =>
                    {
                        var d = (IDev)obj;
                        var revData = new List<byte>();
                        //var ok = Encoding.ASCII.GetBytes("ok");
                        // d.DataSendFrame(ok,0,ok.Length);
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
                               
#if debug
                                    //AddLogMsg($"REV <-- {string.Join("", Array.ConvertAll(tmpReceivingString.ToArray(), p => p))}");
#endif
                                    if (tmpReceivingString.Exists(p => ClientNetDev.picok.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.picNG.IsMatch(p)))
                                    {
                                        d.IsReceiveNg = true;
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.eraseOk.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.eraseNG.IsMatch(p)))
                                    {
                                        //d.IsReceiveNg = true;
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.storeOk.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.eraseNG.IsMatch(p)))
                                    {
                                        //d.IsReceiveNg = true;
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.ddrOk.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.ddrNG.IsMatch(p)))
                                    {
                                        d.IsReceiveNg = true;
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.ddrstopNG.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.ddrstopOk.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.showok.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.poweronok.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.showng.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.poweronng.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.slaveShow.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.progmOk.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();

                                        if (_isReadNextLine)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                var ds = new byte[revData.Count];
                                                Array.Copy(revData.ToArray(), 0, ds, 0, revData.Count);

                                                if (ds.Length > 1)
                                                {
                                                    var hexText = string.Join($" ", Array.ConvertAll(ds, input => $"{input:X2}"));
                                                    _curEditor.AppendText(hexText + "\r\n");
                                                }
                                                else
                                                {
                                                    var hexText = string.Join($" ", Array.ConvertAll(ds, input => $"{input:X2}"));
                                                    _curEditor.AppendText(hexText.Substring(0, hexText.Length - 1) + "\r\n");
                                                }

                                            });
                                            _isReadNextLine = false;
                                        }
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.progmEr.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.slaveWrite.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else if (tmpReceivingString.Exists(p => ClientNetDev.slaveRead.IsMatch(p)))
                                    {
                                        d.ResetEvent.Set();
                                    }
                                    else
                                    {
                                        Thread.Sleep(10);
                                    }
                                    if (_isPowerOn)
                                    {
                                        if (tmpReceivingString.Exists(p => ClientNetDev.slavePowerOn.IsMatch(p)))
                                        {
                                            d.ResetEvent.Set();

                                        }
                                        else
                                        {
                                            AddLogMsg("上电失败，请重新尝试");
                                        }
                                        _isPowerOn = false;
                                    }

                                    if (_selectPic)
                                    {
                                        if (tmpReceivingString.Exists(p => ClientNetDev.slaveCheckSum.IsMatch(p)))
                                        {
                                            d.ResetEvent.Set();
                                            if (CheckSumCompare(tmpReceivingString))
                                            {
                                                _rsst.Set();

                                            }
                                            else
                                            {
                                                AddLogMsg("无一致图片，请重新下载后选择显示");
                                            }
                                        }
                                        _selectPic = false;
                                    }

                                    if (_isPowerff)
                                    {
                                        if (tmpReceivingString.Exists(p => ClientNetDev.slavepowerOff.IsMatch(p)))
                                        {
                                            d.ResetEvent.Set();

                                        }
                                        else
                                        {
                                            AddLogMsg("关电失败，请重新尝试");
                                        }
                                        _isPowerff = false;
                                    }



                                }
                                else
                                {
                                    if (!d.Connected())
                                    {
                                        IsServerConnected = false;
                                        AddLogMsg("服务器已断开连接");
                                        PanelUnLock = true;
                                        ClientRunList.Remove(d);
                                        for (var i = 0; i < ViewClients.Count; i++)
                                        {
                                            if (ViewClients[i].Dev == d)
                                            {
                                                ViewClients[i].IsOffLine = true;
                                            }
                                        }
                                        break;
                                    }
                                }

                                revData.Clear();
                            }
                            catch (Exception e)
                            {
                                AddLogMsg($"Rev ERR: {e.Message}", 1);
                            }
                        }
                    })
                    { IsBackground = true, Priority = ThreadPriority.AboveNormal }.Start(dev);
            }
            else
            {
                IsServerConnected = false;
                AddLogMsg($"连接服务器失败:{_gbl.RemoteIpAddress}:{_gbl.Port}", 0);
            }
        }
        public void SockDisconnect()
        {

            if (ClientRunList.Any())
            {
                ClientRunList.RemoveAll(delegate (IDev item) {
                    return true;
                });
            }
        }
        

        /// <summary>
        /// open a task for listenning the background network connection
        /// </summary>
        private void SockListen()
        {
            DevInitUtil.Listen(GblInfo, out var tcpClient00);

            if (tcpClient00 != null)
            {
                var dev = App.Locator.ServDev;
                dev.Sock = tcpClient00;
                
                ClientRunList.Add(dev);
                var clientIp = tcpClient00.Client.RemoteEndPoint.ToString();

                AddLogMsg("客户端" + clientIp + "已连接到本服务器");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewClients.Add(new ClientList(dev, ViewClients.Count, clientIp));
                });

                new Thread(obj =>
                {
                    var d = (IDev)obj;
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
                                else
                                {
                                    Thread.Sleep(10);
                                }
                            }
                            else
                            {
                                if (d.Connected())
                                {
                                    AddLogMsg("客户端已断开连接");
                                    ClientRunList.Remove(d);
                                    for (var i = 0; i < ViewClients.Count; i++)
                                    {
                                        if (ViewClients[i].Dev == d)
                                        {
                                            ViewClients[i].IsOffLine = true;
                                        }
                                    }
                                    break;
                                }
                            }
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
                }) { IsBackground = true, Priority = ThreadPriority.AboveNormal }.Start(dev);
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
                var pic = System.Drawing.Image.FromFile(imgItem.FnPath);
                pic.Save(_usingFileName, ImageFormat.Bmp);
                var w = pic.Width;
                var h = pic.Height;
                pic.Dispose();

                var fs = new FileStream(_usingFileName, FileMode.Open);
                oriBytes = new byte[fs.Length];
                fs.Read(oriBytes, 0, oriBytes.Length);
                fs.Close();

                if (dev != null)
                {
                    //start monitors the picture analyze task
                    Stopwatch watch = new Stopwatch();
                    AddLogMsg("解析任务:" + imgItem.Des);
                    watch.Start();
                    var data = new DataBmpAlg(_gbl, oriBytes, OddMaskArgb[0], EvenMaskArgb[0], PadLoc);
                    //添加checkSum
                    var checkSum = string.Join("", Array.ConvertAll(data.CheckSum, b => $"{b:x2}"));
                    imgItem.Cs = checkSum;
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
                        if (!IsCmdRun)
                        {
                            dev.HeaderList.AddRange(
                                CreateHeadData(imageIndex, data.FinalData.Count, w, h, imgItem.Des));

                        }
                        else
                        {
                            dev.HeaderList.AddRange(
                                CreateDDRHeadData(imageIndex, data.FinalData.Count, w, h, imgItem.Des));
                        }

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
        
        public bool DataSendALine()
        {
            List<byte[]> allLine = new List<byte[]>();
            List<byte[]> oneLine = null;
            int curFileLine = 0;
            var alg = App.Locator.TextModal.ProgrammeLine(CurProgmFile, AlgMap, out IList<DocumentLine> edLines);
            foreach (var editorContent in App.Locator.TextModal.AllFileEditor)
            {
                if (editorContent.FilePath == CurProgmFile)
                {
                    curFileLine = editorContent.SelectedLine;
                    break;
                }
            }
            foreach (var line in edLines)
            {
                oneLine = alg.CompileLine(line, curFileLine);
                if (oneLine != null)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            allLine.AddRange(oneLine.ToArray());
            var lineResult = new List<byte[]>();
            for (int i=0;i<allLine.Count;i++)
            {
                var lineStr = i.ToString("000");
                lineResult.Add(ProgrammeUtil.ParseCmd("RU", "0", $"{lineStr}", allLine[i].ToArray()));
            }



            if (ClientRunList.Any())
            {
                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                {
                    foreach (var val in lineResult)
                    {
                        if (!pair.DataSendFrame(val, 0, 3000))
                        {
                            _log.Debug($"发送一行数据失败, {pair}");
                        }
                        _log.Debug($"---> {string.Join($" ", Array.ConvertAll(val, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(val)}");
                        AddLogMsg("发送一行数据成功。");
                    }
                }));
                try
                {
                    tsks.LastOrDefault().RunSynchronously();
                }
                catch (Exception ex)
                {
                    //
                }
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
            }
            return true;
        }

        private ManualResetEvent _executeNext = new ManualResetEvent(false);
        private AutoResetEvent waitSend = new AutoResetEvent(false);
        public bool DataSendALLLine()
        {
            if (ClientRunList.Any())
            {
                var alg = App.Locator.TextModal.ProgrammeLine(CurProgmFile, AlgMap,
                        out IList<DocumentLine> allLines);
                var tmpLst = new ConcurrentBag<DocumentLine>();
                foreach (var line in allLines)
                {
                    tmpLst.Add(line);
                }
                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew((p) =>
                {
                    var lineResult = new List<byte[]>();
                  
                    var stw = new Stopwatch();
                    stw.Start();
                    for (int j = 0; j < WantToExeCount; j++)
                    {
                        AddLogMsg($"开始执行第{j}次");
                        foreach (var line in (ConcurrentBag<DocumentLine>)p)
                        {

                            List<byte[]> allLine = new List<byte[]>();
                            List<byte[]> oneLine = null;
                            oneLine = alg.CompileLine(line, line.LineNumber);
                            if (oneLine != null)
                            {
                                allLine.AddRange(oneLine.ToArray());
                            }
                            else
                            {
                                continue;
                            }


                            for (int i = 0; i < allLine.Count; i++)
                            {
                                var lineStr = i.ToString("000");
                                lineResult.Add(ProgrammeUtil.ParseCmd("RU", "0", $"{lineStr}", allLine[i].ToArray()));
                            }

                            //提示当前执行的行号，并等待执行下一行的命令
                            foreach (var poin in App.Locator.TextModal.BreakPoints)
                            {
                                if (poin.BreakDown && line.LineNumber == int.Parse(poin.Id))
                                {
                                    for (int i = 0; i < allLine.Count; i++)
                                    {
                                        AddLogMsg(
                                            $"当前行{line.LineNumber}引发断点，请点击下一步,断点处的数据为{string.Join($" ", Array.ConvertAll(allLine[i].ToArray(), input => $"{input:X2}"))}:{Encoding.ASCII.GetString(allLine[i].ToArray())}");
                                    }

                                    Thread.Sleep(1000);
                                    _executeNext.WaitOne(-1);
                                }
                            }

                            foreach (var val in lineResult)
                            {
                                if (!pair.DataSendFrame(val, 0, _gbl.LongTimeoutForElapsed))
                                {
                                    _log.Debug($"发送一行数据失败, {pair}");
                                }

                                _log.Debug(
                                    $"---> {string.Join($" ", Array.ConvertAll(val, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(val)}");

                                AddLogMsg("发送一行数据成功");
                            }

                            lineResult.Clear();
                        }
                    }
                    stw.Stop();
                    AddLogMsg($"执行完成，所用时间{stw.Elapsed}s");
                }, tmpLst));

                try
                {
                    tsks.LastOrDefault().RunSynchronously();
                }
                catch (Exception e)
                {
                    
                }
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
            }
            return true;
        }
        public void ProgrammDownLoadFile()
        {
            if (ClientRunList.Any())
            {
                var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                {
                    EnableBtn = false;
                    var step = double.Parse($"{100.00 / (double) (_compFile.Count + 20)}");
                    var cn = ProgrammeUtil.ParseCmd("CN", "0", "000", new byte[] { });
                    if (!pair.DataSendFrame(cn, 0, _gbl.LongTimeoutForElapsed))
                    {
                        AddLogMsg($"发送握手失败, {pair}");
                        _log.Debug(
                            $"---> {string.Join($" ", Array.ConvertAll(cn, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(cn)}");
                    }
                    else
                    {
                        AddLogMsg("握手成功");
                        for (int i = 0; i < _compFile.Count; i++)
                        {
                            ProgressData = 0;
                            AddLogMsg("擦除数据");
                            ProgressData = 20 * step;
                            var erase = ProgrammeUtil.ParseCmd("ES", $"{i}", "000", new byte[] { });
                            if (!pair.DataSendFrame(erase, 0, _gbl.LongTimeoutForElapsed))
                            {
                                _log.Debug($"发送擦除失败, {pair}");
                            }

                            _log.Debug(
                                $"---> {string.Join($" ", Array.ConvertAll(erase, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(erase)}");
                            Thread.Sleep(1000);
                            for (int j = 0; j < _compFile[i].Count; j++)
                            {

                                var send = _compFile[i][j];
                                if (!pair.DataSendFrame(send, 0, _gbl.LongTimeoutForElapsed))
                                {
                                    _log.Debug($"发送一行数据失败, {pair}");
                                }

                                ProgressData = ProgressData + step;

                                _log.Debug(
                                    $"---> {string.Join($" ", Array.ConvertAll(_compFile[i][j], input => $"{input:X2}"))}:{Encoding.ASCII.GetString(_compFile[i][j])}");
                            }
                        }

                        Thread.Sleep(1000);
                        ProgressData = 100;
                        AddLogMsg("发送数据完成");
                        EnableBtn = true;
                    }
                }));
                try {
                    tsks.LastOrDefault().RunSynchronously();
                }
                catch (Exception ex)
                {
                    //
                }
               
            }
            else
            {
                AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
            }
        }
        public void ClearSendList()
        {
            ImgItemInfos.Clear();
            if (File.Exists(_gbl.FileListXml))
            {
                XmlDelete(_gbl.FileListXml);
            }
        }
        private ManualResetEvent _rsst = new ManualResetEvent(false);
        public bool CheckSumCompare(List<string> lst)
        {
            foreach (var tmp in lst)
            {
                var ss = tmp.Split(',');
                if (ss.Length < 1)
                {
                    break;
                }
                var checkSum = ss.LastOrDefault();
                var tmp1 = checkSum.Insert(checkSum.Length / 2 - 1, "0000");
                var tmp2 = tmp1.Insert(tmp1.Length - 2, "0000");
                var res = ImgItemInfos.Any(p=>{
                    return tmp2.Contains(p.Cs);
                });
                return res;
            }
            return false;
        }

        private TextEditor _curEditor;
        public void ReadFromDevice()
        {
            int curline = 0;
            if (!string.IsNullOrEmpty(CurProgmFile))
            {
                foreach (var editorContent in App.Locator.TextModal.AllFileEditor)
                {
                    if (editorContent.FilePath.Equals(CurProgmFile))
                    {
                        curline = editorContent.SelectedLine;
                        _curEditor = editorContent.Editor;
                        break;
                    }
                }
                var readCmd = ProgrammeUtil.ParseCmd("RL", $"{FileIndex}", $"{curline.ToString("000")}", new byte[] { });
                if (ClientRunList.Any())
                {
                    var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                    {
                        if (!pair.DataSendFrame(readCmd, 0, 3000))
                        {
                            _log.Debug($"发送一行数据失败, {pair}");
                        }

                        _log.Debug(
                            $"---> {string.Join($" ", Array.ConvertAll(readCmd, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(readCmd)}");

                        AddLogMsg("发送一行数据成功");
                    }));
                    try
                    {
                        tsks.LastOrDefault().RunSynchronously(TaskScheduler.Default);
                    }
                    catch (Exception e)
                    {

                    }

                }
                else
                {
                    AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
                }
            }
            else
            {
                AddLogMsg("未建立新编辑文件，请先打开一个文件");
            }
           
        }

        ManualResetEvent _rdFileEvent = new ManualResetEvent(false);
        public void ReadFilesFromDevice()
        {
            //for (int i = 0; i < WantToReadCount; i++)
            //{
            //    _rdFileEvent.Reset();
            //    ReadFromDevice();
               
            //}
        }
        #region 按钮功能
        private bool _selectPic = false;
        private ICommand _imgItemSelectionChangedCmd;
        public ICommand ImgItemSelectionChangedCmd
        {
            get => _imgItemSelectionChangedCmd ?? (_imgItemSelectionChangedCmd = new UtilRelayCommand(delegate (object obj)
            {
                var param = obj as ExCommandParameter;
                if (param?.Parameter is ImgItemUi info)
                {
                    _selectPic = true;
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
                            CurrentChooseFile = info.Des;
                            if (IsChooseDisplay)
                            {
                                _rsst.Reset();
                                var curIndex = 0;
                                for (int i=0;i<ImgItemInfos.Count;i++)
                                {
                                    if (ImgItemInfos[i].Des==info.Des)
                                    {
                                        curIndex = i;
                                        break;
                                    }
                                }
                                byte[] sendCheckSum = StringToByteArray($"$c.checksum,{curIndex.ToString("x2")}\r\n");
                                byte[] show = StringToByteArray($"$c.ShowImage,{curIndex.ToString("x2")}\r\n");
                                if (ClientRunList.Any())
                                {
                                    var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                                    {
                                        if (!pair.DataSendFrame(sendCheckSum, 0, 3000))
                                        {
                                            _log.Debug($"checkSum指令发送失败, {pair}");
                                            return;
                                        }
                                        if (!_rsst.WaitOne(2000))
                                        {
                                            return;
                                        }
                                        if (!pair.DataSendFrame(show, 0,1000))
                                        {
                                            _log.Debug($"show指令发送失败, {pair}");
                                        }
                                    }));

                                    try
                                    {
                                        tsks.LastOrDefault().RunSynchronously();
                                    }
                                    catch (Exception)
                                    {
                                        //
                                    }
                                }
                                else
                                {
                                    AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
                                }
                            }
                        }
                        else
                        {
                            ActiveBinItem(info, false);
                            AddLogMsg("所选择文件信息" + info.Des);
                            CurrentChooseFile = info.Des;
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
            get => _openFdClearCmd ?? (_openFdClearCmd = new UtilRelayCommand(delegate (object obj)
            {
                ClearSendList();
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
            get => _openFdCmd ?? (_openFdCmd = new UtilRelayCommand(delegate (object obj)
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
                            ImgItemInfos.Add(new ImgItemUi()
                            {
                                IsActived = true,
                                FnPath = Path.GetFullPath(p),
                                Des = Path.GetFileName(p),
                                Cs = "0000000000000000"
                            });
                        });
                    }
                    else
                    {
                        ImgItemInfos.Add(new ImgItemUi()
                        {
                            IsActived = true,
                            FnPath = Path.GetFullPath(ofd.FileName),
                            Des = Path.GetFileName(ofd.FileName),
                            Cs = ""
                        });
                    }
                }
            }));
        }

        private ICommand _serverConnect;
        public ICommand ServerConnect
        {
            get => _serverConnect ?? (_serverConnect = new UtilRelayCommand(delegate (object obj)
            {
                if (IsNetWorkSend)
                {
                    SockConnect();
                }
            }));
        }

        private ICommand _sendItemsCmd;
        private bool _isClientConnected;
        private bool _isAddSizeToHeader;

        private readonly TxtSimpLog _simp = new TxtSimpLog(Encoding.ASCII);
        private string _address;
        private int _port;
        private bool _highLowBytesRevert;
        public ICommand SendItemsCmd
        {
            get => _sendItemsCmd ?? (_sendItemsCmd = new UtilRelayCommand(delegate (object obj)
            {
                LogItems.Clear();
                
                ProgressData = 0;
                AddLogMsg($"开始数据发送...");
                
                new Thread(() =>
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
                }){IsBackground = true, Priority = ThreadPriority.AboveNormal}.Start();
            }));
        }

        private ICommand _itemsMoveUp;
        public ICommand ItemsMoveUp
        {
            get => _itemsMoveUp ?? (_itemsMoveUp = new UtilRelayCommand(delegate (object obj)
            {
                int index = 0;
                for (int i=1;i<ImgItemInfos.Count;i++)
                {
                    if (ImgItemInfos[i].Des.Equals(CurrentChooseFile))
                    {
                        index = i;
                        break;
                    }
                }
                if (index > 0)
                {
                    var tmp = ImgItemInfos[index - 1];
                    ImgItemInfos[index - 1] = ImgItemInfos[index];
                    ImgItemInfos[index] = tmp;
                }
                else
                {
                    AddLogMsg("CAN'T MOVE UP,BECAUSE THIS IS THE FIRST FILE!");
                }
                
            }));
        }
        private ICommand _itemsMoveDown;
        public ICommand ItemsMoveDown
        {
            get => _itemsMoveDown ?? (_itemsMoveDown = new UtilRelayCommand(delegate (object obj)
            {
                int index = 0;
                for (int i = 1; i < ImgItemInfos.Count; i++)
                {
                    if (ImgItemInfos[i].Des.Equals(CurrentChooseFile))
                    {
                        index = i;
                        break;
                    }
                }
                if (index < ImgItemInfos.Count - 1)
                {
                    var tmp = ImgItemInfos[index + 1];
                    ImgItemInfos[index + 1] = ImgItemInfos[index];
                    ImgItemInfos[index] = tmp;
                }
                else
                {
                    AddLogMsg("CAN'T MOVE DOWN,BECAUSE THIS IS THE LAST FILE!");
                }

            }));
        }
        private bool _isPowerOn = false;
        private ICommand _powerOnDevice;
        public ICommand PowerOnDevice
        {
            get => _powerOnDevice ?? (_powerOnDevice = new UtilRelayCommand(delegate (object obj)
            {
                byte[] powerOn = StringToByteArray("$c.DUT.powerOn,NormalMode\r\n");
                if (ClientRunList.Any())
                {
                    _isPowerOn = true;
                    var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                    {
                        // if (!DataSendFrame(erase, currentSockId, GblInfo.LongTimeoutForElapsed))
                        if (!pair.DataSendFrame(powerOn, 0, 3000))
                        {
                            AddLogMsg($"powerOn指令发送失败, {pair}");
                        }
                        AddLogMsg("发送PowerOn指令成功");
                    }));
                    try
                    {
                        tsks.LastOrDefault().RunSynchronously();
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                }
                else
                {
                    AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
                }
            }, pre =>
            {
                return true;
            }));
        }
        private bool _isPowerff = false;
        private ICommand _powerOffDevice;
        public ICommand PowerOffDevice
        {
            get => _powerOffDevice ?? (_powerOffDevice = new UtilRelayCommand(delegate (object obj)
            {
                byte[] powerOff = StringToByteArray("$c.DUT.powerOff\r\n");
                if (ClientRunList.Any())
                {
                    _isPowerff = true;
                    var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                    {
                        if (!pair.DataSendFrame(powerOff, 0, 1000))
                        {
                            AddLogMsg($"powerOff指令发送失败, {pair}", 1);
                            // _log.Debug($"powerOff指令发送失败, {pair}");
                        }
                        AddLogMsg("发送PowerOff指令成功");
                    }));
                    try
                    {
                        tsks.LastOrDefault().RunSynchronously();
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                }
                else
                {
                    AddLogMsg("无网络设备连接，请连接设备后再发送", 1);
                }

            }, pre =>
            {
                return true;
            }));
        }

        public ICommand _proggramAddFile;
        public ICommand ProggramAddFile
        {
            get => _proggramAddFile ?? (_proggramAddFile = new UtilRelayCommand(delegate (object obj)
            {
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = "Operation|*.txt",
                    Multiselect = true,
                };
                
                if (fileDialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                {
                    if (fileDialog.FilterIndex == 1)
                    {
                        fileDialog.FileNames.ToList().ForEach(p =>
                        {
                            ProgrammeFiles.Add(new ImgItemUi()
                            {
                                IsActived = true,
                                FnPath = Path.GetFullPath(p),
                                Des = Path.GetFileName(p),
                                Cs = ""
                            });
                            CurProgmFile = Path.GetFullPath(p);
                        });
                    }
                    else
                    {
                        ProgrammeFiles.Add(new ImgItemUi()
                        {
                            IsActived = true,
                            FnPath = Path.GetFullPath(fileDialog.FileName),
                            Des = Path.GetFileName(fileDialog.FileName),
                            Cs = ""
                        });
                    }
                }
            }, pre =>
            {
                return true;
            }));
        }
        List<List<byte[]>> _compFile = null;
        private int _index = -1;
        public ICommand _proggramFile;
        public ICommand ProggramFile
        {
            get => _proggramFile ?? (_proggramFile = new UtilRelayCommand(delegate (object obj)
            {
                var p = new AlgProgmFiles();
                if (ProgrammeFiles.Any(s=>s.IsActived))
                {
                    AddLogMsg($"开始编译...");
                    _compFile = p.CompileAllFiles(ProgrammeFiles);
                    AddLogMsg("编译完成...");
                }
                else
                {
                   AddLogMsg("无文件被打开，请打开一个文件后再编译");
                }

            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _proggramClearFile;
        public ICommand ProggramClearFile
        {
            get => _proggramClearFile ?? (_proggramClearFile = new UtilRelayCommand(delegate (object obj)
            {
               

            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _proggramSendALl;
        public ICommand ProggramSendALl
        {
            get => _proggramSendALl ?? (_proggramSendALl = new UtilRelayCommand(delegate(object obj)
            {
                DataSendALLLine();

            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _proggramOutPutFile;
        public ICommand ProggramOutPutFile
        {
            get => _proggramOutPutFile ?? (_proggramOutPutFile = new UtilRelayCommand(delegate (object obj)
            {
                if (!string.IsNullOrEmpty(CurProgmFile))
                {
                    var diag = new SaveFileDialog()
                    {
                        Filter = "Operator|*.txt",
                    };
                    if (diag.ShowDialog().HasValue)
                    {
                        if (!string.IsNullOrEmpty(diag.SafeFileName))
                        {
                            //var file = File.Open(Path.(diag.SafeFileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            foreach (var edit in App.Locator.TextModal.AllFileEditor)
                            {
                                edit.FileName = diag.FileName;
                                edit.FilePath = Path.GetFullPath(diag.FileName);
                            }
                            try
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var file = diag.OpenFile();
                                    if (file.CanWrite)
                                    {
                                        App.Locator.TextModal.SaveFile();
                                        file.Flush();
                                        file.Close();
                                        AddLogMsg("保存成功");
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                AddLogMsg("写文件错误:" + ex.Message, 1);
                            }
                        }
                        if (ProgrammeFiles.All(p => p.Des != diag.SafeFileName) && !string.IsNullOrEmpty(diag.SafeFileName))
                        {
                            ProgrammeFiles.Add(new ImgItemUi()
                            {
                                Des = Path.GetFileName(diag.SafeFileName),
                                FnPath = Path.GetFullPath(diag.SafeFileName),
                                IsActived = true,
                            });
                        }
                        if (!string.IsNullOrEmpty(diag.FileName))
                        {
                            CurProgmFile = Path.GetFullPath(diag.FileName);
                        }
                    }
                }

            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _proggramDownLoadFile;
        public ICommand ProggramDownLoadFile
        {
            get => _proggramDownLoadFile ?? (_proggramDownLoadFile = new UtilRelayCommand(delegate (object obj)
            {
                if (_compFile != null && _compFile.Any())
                {
                    ProgrammDownLoadFile();
                }
                else
                {
                    AddLogMsg("未编译文件，请先编译文件。");
                }



            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _proggramSendSingle;
        public ICommand ProggramSendSingle
        {
            get => _proggramSendSingle ?? (_proggramSendSingle = new UtilRelayCommand(delegate (object obj)
            {
                if (!string.IsNullOrEmpty(CurProgmFile))
                {
                    DataSendALine();
                }
                else
                {
                    AddLogMsg("未打开任何文件，请打开文件后再尝试！");
                }
               
            }, pre =>
            {
                return true;
            }));
        }


        private ICommand _fileItemSelectedChange;
        public ICommand FileItemSelectedChange
        {
            get => _fileItemSelectedChange ?? (_fileItemSelectedChange = new UtilRelayCommand(delegate (object obj)
            {
                var param = obj as ExCommandParameter;
                Task.Factory.StartNew(() =>
                {
                    FileManaEnable = false;
                    if (param?.Parameter is ImgItemUi info)
                    {
                        if (info.Des.Contains(".txt"))
                        {
                            CurrentChooseFile = info.Des;
                            info.IsActived = true;
                            _rsst.Reset();
                            for (int i = 0; i < ProgrammeFiles.Count; i++)
                            {
                                if (ProgrammeFiles[i].Des == info.Des)
                                {
                                    info.IsActived = true;
                                    break;
                                }
                            }
                        }
                        AddLogMsg("打开文件：" + info.FnPath);


                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                CurProgmFile = info.FnPath;
                                if (!string.IsNullOrEmpty(info.Des))
                                {
                                    var str = File.ReadAllText(info.FnPath);
                                    
                                    if (!Pane.Children.Any(p=>p.Title==info.Des))
                                    {
                                        var doc = App.Locator.TextModal.CreateAnDocumentEditor(str, info.Des,info.FnPath);
                                        Pane.Children.Add(doc);
                                        for (int i = 1; i < Pane.ChildrenCount; i++)
                                        {
                                            if (Pane.Children[i].Title == info.Des)
                                            {
                                                Pane.SelectedContentIndex = i;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 1; i < Pane.ChildrenCount; i++)
                                        {
                                            if (Pane.Children[i].Title == info.Des)
                                            {
                                                Pane.SelectedContentIndex = i;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                AddLogMsg("打开文件错误:" + ex.Message, 1);
                            }
                        });
                        FileManaEnable = true;
                    }
                });
            }, pre =>
            {
                return true;
            }));
        }

        private bool _isReadNextLine = false;
        private ICommand _readNextLine;
        public ICommand ReadNextLine
        {
            get => _readNextLine ?? (_readNextLine = new UtilRelayCommand(delegate(object obj)
            {
                _isReadNextLine = true;
                ReadFromDevice();

            }, pre =>
            {
                return true;
            }));
        }
        private ICommand _executeNextLine;
        public ICommand ExecuteNextLine
        {
            get => _executeNextLine ?? (_executeNextLine = new UtilRelayCommand(delegate (object obj)
            {
                _executeNext.Set();
            }, pre =>
            {
                return true;
            }));
        }

        private int tmpIndex = 1;
        private ICommand _programmeSaveFile;
        public ICommand ProgrammeSaveFile
        {
            get => _programmeSaveFile ?? (_programmeSaveFile = new UtilRelayCommand(delegate (object obj)
            {
                if (string.IsNullOrEmpty(CurProgmFile))
                {
                    var fname = $"New_File{tmpIndex++}.txt";
                    var path = _gbl.ProgmWorkDir +"\\" +fname;
                    Pane.Children.Add(App.Locator.TextModal.CreateAnDocumentEditor("", fname,
                        Path.GetFullPath(_gbl.ProgmWorkDir)));
                    CurProgmFile = Path.GetFullPath(_gbl.ProgmWorkDir);
                }
                else
                {
                    if (!string.IsNullOrEmpty(CurProgmFile))
                    {
                        try
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                App.Locator.TextModal.SaveFile();
                                AddLogMsg("保存成功");
                            });
                        }
                        catch (Exception ex)
                        {
                            AddLogMsg("写文件错误:" + ex.Message, 1);
                        }
                    }
                    if (ProgrammeFiles.All(p => p.Des != Path.GetFileName(CurProgmFile)) && !string.IsNullOrEmpty(CurProgmFile) && CurProgmFile.Contains(".txt"))
                    {
                        ProgrammeFiles.Add(new ImgItemUi()
                        {
                            Des = Path.GetFileName(CurProgmFile),
                            FnPath = Path.GetFullPath(CurProgmFile),
                        });
                    }
                }
            }, pre =>
            {
                return true;
            }));
        }

        public ICommand _proggramRemoveFile;
        public ICommand ProggramRemoveFile
        {
            get => _proggramRemoveFile ?? (_proggramRemoveFile = new UtilRelayCommand(delegate (object obj)
            {
                foreach (var info in ProgrammeFiles)
                {
                    var file = Path.GetFileName(CurProgmFile);
                    if (file == info.Des)
                    {
                        ProgrammeFiles.Remove(info);
                       // Editor.Clear();
                        CurProgmFile = string.Empty;
                        LastSaveProgmFile=String.Empty;
                        break;
                    }
                }
            }, pre =>
            {
                return true;
            }));
        }
        public ICommand _sendFromTextBox;
        public ICommand SendFromTextBox
        {
            get => _sendFromTextBox ?? (_sendFromTextBox = new UtilRelayCommand(delegate (object obj)
            {
                var cmd = (string)obj;
                if (ClientRunList.Any())
                {
                    var tsks = ClientRunList.Select(pair => Task.Factory.StartNew(() =>
                    {
                        var resCmd = ConvertTheText(cmd);
                        if (!pair.DataSendFrame(resCmd, 0, _gbl.LongTimeoutForElapsed))
                        {
                            AddLogMsg($"发送命令失败{cmd}");
                        }
                        else
                        {
                            _log.Debug($"---> {string.Join($" ", Array.ConvertAll(resCmd, input => $"{input:X2}"))}:{Encoding.ASCII.GetString(resCmd)}");
                            AddLogMsg($"发送命令成功{cmd}");
                        }
                       
                    }));
                    try
                    {
                        tsks.LastOrDefault().RunSynchronously();
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                }
                else
                {
                    AddLogMsg("无设备连接请连接网络编程设备");
                }
               
            }, pre =>
            {
                return true;
            }));
        }
        private ICommand _readFileFromDevice;

        public ICommand ReadFileFromDevice
        {
            get => _readFileFromDevice ?? (_readFileFromDevice =
                       new UtilRelayCommand(delegate(object obj)
                       {
                           ReadFilesFromDevice();

                       }, pre => { return true; }));
        }

        #endregion
        #region 菜单栏

        private ICommand _sendToDDR;
        public ICommand SendToDDR
        {
            get => _sendToDDR ?? (_sendToDDR = new UtilRelayCommand(delegate(object obj)
            {
                CmdNetWorkSend();

            }, pre =>
            {
                return true;
            }));
        }



        #endregion

        #region 文档管理功能

        private DockingManager _dockingManager;

        public void EditotManager(DockingManager dock)
        {


        }

        #endregion

        #region 配置功能
        public byte[] ConvertTheText(string resCmd)
        {
            int register = 0;
            string hex = string.Empty;
            Action<string,byte[],string> action = (p,b,h) => 
            {
                if (b != null)
                {
                    hex += "0x";
                    for (int i = b.Length - 1; i >= 0; i--)
                    {
                        hex += $"{b[i].ToString("x2")}";
                    }
                }
                else if(p!=null)
                {
                    int.TryParse(p, out int result);
                    var tmp = ProgrammeUtil.ParserDataLH(result);
                    hex += "0x";
                    for (int i = tmp.Length - 1; i >= 0; i--)
                    {
                        hex += $"{tmp[i].ToString("x2")}";
                    }
                }
                else if (h!=null)
                {
                    hex = $"0x{h}";
                }
            };
            switch (resCmd)
            {
                case "IOVCC:":
                    register = 1;
                    action(IovccParam, null, null);
                    break;
                case "VSP/VSN:":
                    register = 2;
                    action(VspParam, null, null);
                    break;
                case "RESX:":
                    register = 3;
                    action(ResxParam, null, null);
                    break;
                case "BRIGHT:":
                    register = 4;
                    action(BrightParam, null, null);
                    break;

                case "PLL:":
                    register = 20;
                    int.TryParse(PllParam, out int num);
                    var tmp = new List<byte>();
                    tmp.Add((byte)(num / 5));
                    tmp.Add(0xc5);
                    //tmp.AddRange(ProgrammeUtil.ParserDataLH(9));
                    action(PllParam, tmp.ToArray(), null);
                    break;

                case "REG_B6:":
                    register = 22;
                    action(null, null, Regb6Param);
                    break;
                case "REG_D6:":
                    register = 23;
                    action(null, null, Regd6Param);
                    break;
                case "REG_DE:":
                    register = 24;
                    action(null, null, RegdeParam);
                    break;

                case "PANEL_B6:":
                    register = 28;
                    action(null, null, Panelb6Param);
                    break;

                case "VSA:":
                    register = 30;
                    action(VsaParam, null, null);
                    break;
                case "HSA:":
                    register = 31;
                    action(HasParam, null, null);
                    break;
                case "VFP:":
                    register = 32;
                    action(VfpParam, null, null);
                    break;
                case "HFP:":
                    register = 33;
                    action(HfpParam, null, null);
                    break;
                case "VBP:":
                    register = 34;
                    action(VbpParam, null, null);
                    break;
                case "HBP:":
                    register = 35;
                    action(HbpParam, null, null);
                    break;
                case "HACT:":
                    register = 36;
                    action(HactParam, null, null);
                    break;
                case "VACT:":
                    register = 37;
                    action(VactParam, null, null);
                    break;
            }

            if (Compress)
            {
                register += 20;
            }
            var cmd = $"$c.register.write,{register},{hex}\r\n";
            return ProgrammeUtil.StringToByteArray(cmd);
        }

        #endregion

    }
}