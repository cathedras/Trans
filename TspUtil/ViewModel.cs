using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using myzy.Util;
using Microsoft.Win32;
using TspUtil.Annotations;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace TspUtil
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<LogItem> _logItems;
        private ObservableCollection<ImgItemInfo> _imgItemInfos;
        private readonly string _cfg = @"..\tspUtil.ini";
        private readonly string _usingFileName = "_tmpUsing.bmp";
        private string _activeFn;
        private readonly Gbl _gbl;

        private int _expPixHeight;
        private int _expPixWidth;
        private string _swVersion;
        private PadLoc _padLoc;
        private string _padStr;

        private ICommand _sendItemsCmd;
        private ImageSource _imgSource;
        private ObservableCollection<MaskForArgbItem> _oddMaskArgb;
        private ObservableCollection<MaskForArgbItem> _evenMaskArgb;

        public ObservableCollection<LogItem> LogItems
        {
            get => _logItems ?? (_logItems = new ObservableCollection<LogItem>());
        }

        public ObservableCollection<ImgItemInfo> ImgItemInfos
        {
            get => _imgItemInfos??(_imgItemInfos = new ObservableCollection<ImgItemInfo>());
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
                OnPropertyChanged();
            }
        }

        public ImageSource ImgSource
        {
            get => _imgSource;
            set
            {
                if (Equals(value, _imgSource)) return;
                _imgSource = value;
                OnPropertyChanged();
            }
        }

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

        public void AddLogMsg(string msg, int level = 0)
        {
            LogItems.Add(new LogItem()
            {
                DateTime = DateTime.Now, Info = msg, Level = level
            });
        }

        public void SaveDataOnExit()
        {
            try
            {
                _gbl.Save(_cfg, typeof(Gbl));
            }
            catch (Exception e)
            {
                //
            }
        }

        public ViewModel()
        {
            SwVersion = "1.0.0";
#if DEBUG
            SwVersion = "0.0.0";
#endif
            _gbl = new Gbl();
            _gbl.LoadGbl<Gbl>(_cfg);

            ExpPixHeight = _gbl.ExpPixHeight;
            ExpPixWidth = _gbl.ExpPixWidth;
            Enum.TryParse(_gbl.PadLoc, out _padLoc);
            PadStr = _gbl.PadStr;

            for (int i = 0; i < _gbl.MaskCount; i++)
            {
                OddMaskArgb.Add(new MaskForArgbItem());
                EvenMaskArgb.Add(new MaskForArgbItem());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool ActiveImgItem(ImgItemInfo imgItem)
        {
            Debug.Assert(imgItem != null);
            ImgSource = null;
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
                var pic = Image.FromFile(imgItem.FnPath);
                pic.Save(_usingFileName, ImageFormat.Bmp);
                pic.Dispose();

                var fs = new FileStream(_usingFileName, FileMode.Open);
                var oriBytes = new byte[fs.Length];
                fs.Read(oriBytes, 0, oriBytes.Length);
                fs.Close();

                var data = new DataBmpAlg(_gbl, oriBytes);

                var img = BitmapImageUtil.CreateBitmapImageWithBys(oriBytes);
                img.Freeze();
                ImgSource = img;
                ActiveFn = imgItem.FnPath;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }


        private ICommand _imgItemSelectionChangedCmd;
        public ICommand ImgItemSelectionChangedCmd
        {
            get => _imgItemSelectionChangedCmd ?? (_imgItemSelectionChangedCmd = new RelayCommand(delegate (object obj)
            {
                var param = obj as ExCommandParameter;
                if (param?.Parameter is ImgItemInfo info)
                {
                    ActiveImgItem(info);
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
                    Filter = "Images|*.png;*.bmp"
                };
                if (ofd.ShowDialog(Application.Current.MainWindow).HasValue)
                {
                   ofd.FileNames.ToList().ForEach(p =>
                   {
                       ImgItemInfos.Add(new ImgItemInfo()
                       {
                           IsActived = true,
                           FnPath = Path.GetFullPath(p),
                           Des = Path.GetFileNameWithoutExtension(p),
                           Cs = string.Empty,
                       });
                   });
                }
            }, pre =>
            {
                return true;
            }));
        }
        
        public ICommand SendItemsCmd
        {
            get => _sendItemsCmd ?? (_sendItemsCmd = new RelayCommand(delegate (object obj)
            {

            }, pre =>
            {
                return true;
            }));
        }

    }
}
