using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ElCommon.Util;
using GalaSoft.MvvmLight;

namespace TspUtil.ViewModel
{
    public class KeySettingModal : ViewModelBase
    {
        private ObservableCollection<KeyBindParam> _paramLst;
        private KeyBindParam _selectParam1;
        private KeyBindParam _selectParam2;
        private KeyBindParam _selectParam3;
        private KeyBindParam _selectParam4;
        private KeyBindParam _selectParam5;
        private KeyBindParam _selectParam6;
        private int _selectIndex1 = -1;
        private int _selectIndex2 = -1;
        private int _selectIndex3 = -1;
        private int _selectIndex4 = -1;
        private int _selectIndex5 = -1;
        private int _selectIndex6 = -1;
        public int SelectIndex1
        {
            get => _selectIndex1;
            set
            {
                _selectIndex1 = value; 
                RaisePropertyChanged();
            }
        }

        public int SelectIndex2
        {
            get => _selectIndex2;
            set
            {
                _selectIndex2 = value; 
                RaisePropertyChanged();
            }
        }

        public int SelectIndex3
        {
            get => _selectIndex3;
            set
            {
                _selectIndex3 = value; 
                RaisePropertyChanged();
            }
        }

        public int SelectIndex4
        {
            get => _selectIndex4;
            set
            {
                _selectIndex4 = value; 
                RaisePropertyChanged();
            }
        }

        public int SelectIndex5
        {
            get => _selectIndex5;
            set
            {
                _selectIndex5 = value; 
                RaisePropertyChanged();
            }
        }

        public int SelectIndex6
        {
            get => _selectIndex6;
            set
            {
                _selectIndex6 = value; 
                RaisePropertyChanged();
            }
        }


        public KeyBindParam SelectParam1
        {
            get => _selectParam1;
            set
            {
                _selectParam1 = value;
                RaisePropertyChanged();
            }

        }

        public KeyBindParam SelectParam2
        {
            get => _selectParam2;
            set
            {
                _selectParam2 = value;
                RaisePropertyChanged();
            }

        }

        public KeyBindParam SelectParam3
        {
            get => _selectParam3;
            set
            {
                _selectParam3 = value;
                RaisePropertyChanged();
            }

        }

        public KeyBindParam SelectParam4
        {
            get => _selectParam4;
            set
            {
                _selectParam4 = value;
                RaisePropertyChanged();
            }

        }

        public KeyBindParam SelectParam5
        {
            get => _selectParam5;
            set
            {
                _selectParam5 = value;
                RaisePropertyChanged();
            }

        }

        public KeyBindParam SelectParam6
        {
            get => _selectParam6;
            set
            {
                _selectParam6 = value;
                RaisePropertyChanged();
            }

        }
        
        public ICommand _settingParam;
        public ICommand SettingParam
        {
            get => _settingParam ?? (_settingParam = new UtilRelayCommand(delegate(object obj)
            {
                var param = ((string) obj).Replace(":","");
                //  App.Locator.Main.SelectHexOnKeySetting = ;
                foreach (var keyBindParam in ParamLst)
                {
                    if (SelectParam1.ParamTyp == keyBindParam.ParamTyp && param == "KEY1")
                    {
                        App.Locator.Main.SelectHexOnKeySetting1 = keyBindParam.ParamVal;
                        break;
                    }
                    else if (SelectParam2.ParamTyp == keyBindParam.ParamTyp && param == "KEY2")
                    {
                        App.Locator.Main.SelectHexOnKeySetting2 = keyBindParam.ParamVal;
                        break;
                    }
                    else if (SelectParam3.ParamTyp == keyBindParam.ParamTyp && param == "KEY3")
                    {
                        App.Locator.Main.SelectHexOnKeySetting3 = keyBindParam.ParamVal;
                        break;
                    }
                    else if (SelectParam4.ParamTyp == keyBindParam.ParamTyp && param == "KEY4")
                    {
                        App.Locator.Main.SelectHexOnKeySetting4 = keyBindParam.ParamVal;
                        break;
                    }
                    else if (SelectParam5.ParamTyp == keyBindParam.ParamTyp && param == "KEY5")
                    {
                        App.Locator.Main.SelectHexOnKeySetting5 = keyBindParam.ParamVal;
                        break;
                    }
                    else if (SelectParam6.ParamTyp == keyBindParam.ParamTyp && param == "KEY6")
                    {
                        App.Locator.Main.SelectHexOnKeySetting6 = keyBindParam.ParamVal;
                        break;
                    }
                }

                var cmd = App.Locator.Main.ConvertTheText(param);
                //发送
                var dev = App.Locator.Main.ClientRunList.LastOrDefault();
                Task.Factory.StartNew(() =>
                {
                    if (!dev.DataSendFrame(cmd, 0, 2000))
                    {
                        App.Locator.Main.AddLogMsg($"key: {param} setting failed!");
                    }
                });

            }, pre => { return true; }));
        }

        public ObservableCollection<KeyBindParam> ParamLst
        {
            get { return _paramLst??(_paramLst=new ObservableCollection<KeyBindParam>()); }
        }
    }


    public class KeyBindParam:ViewModelBase
    {
        private string _paramTyp;
        private string _paramVal;


        public KeyBindParam(string paramTyp, string paramVal)
        {
            _paramTyp = paramTyp;
            _paramVal = paramVal;
        }

        public string ParamVal
        {
            get { return _paramVal; }
            set
            {
                if (_paramVal != value)
                {
                    _paramVal = value;
                    RaisePropertyChanged("ParamVal");
                }
            }
        }

        public string ParamTyp
        {
            get { return _paramTyp; }
            set
            {
                if (_paramTyp != value)
                {
                    _paramTyp = value;
                    RaisePropertyChanged("ParamTyp");
                }
            }
        }
    }
}
