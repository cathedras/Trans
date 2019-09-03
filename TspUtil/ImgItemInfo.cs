using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TspUtil.Annotations;
using System.Xml.XmlConfiguration;
using System.Xml;

namespace TspUtil
{
    public interface ImgItemInfo
    {
        bool IsActived { set; get; }
        string Des { set; get; }
        string FnPath { set; get; }
        string Cs { set; get; }
        ImgOpState ImgOpState { set; get; }
        int FileIndex { set; get; }

    }
    public class ImgItemUi  : INotifyPropertyChanged,ImgItemInfo
    {
        private bool _isActived;
        private string _des;
        private string _fnPath;
        private string _cs;
        private ImgOpState _imgOpState;
        private int _fileIndex=-1;

        
        public ImgOpState ImgOpState
        {
            get => _imgOpState;
            set
            {
                if (value == _imgOpState) return;
                _imgOpState = value;
                RaisePropertyChanged();
            }
        }

        public int FileIndex
        {
            get => _fileIndex;
            set
            {
                if (value== _fileIndex) return;
                _fileIndex = value;
                RaisePropertyChanged();
            }

        }


        public bool IsActived
        {
            get => _isActived;
            set
            {
                if (value == _isActived) return;
                _isActived = value;
                RaisePropertyChanged();
            }
        }

        public string Des
        {
            get => _des;
            set
            {
                if (value == _des) return;
                _des = value;
                RaisePropertyChanged();
            }
        }

        public string FnPath
        {
            get => _fnPath;
            set
            {
                if (value == _fnPath) return;
                _fnPath = value;
                RaisePropertyChanged();
            }
        }

        public string Cs
        {
            get => _cs;
            set
            {
                if (value == _cs) return;
                _cs = value;
                RaisePropertyChanged();
            }
        }

        

        public override string ToString()
        {
            return IsActived + "," + Des +","+ FnPath +","+ Cs;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ImgItemCmd :ImgItemInfo
    {
        private bool _isActived;
        private string _des;
        private string _fnPath;
        private string _cs;
        private ImgOpState _imgOpState;
        private int _fileIndex;


        public ImgOpState ImgOpState
        {
            get => _imgOpState;
            set
            {
                if (value == _imgOpState) return;
                _imgOpState = value;
            }
        }

        public int FileIndex
        {
            get => _fileIndex;
            set
            {
                if (value == _fileIndex) return;
                _fileIndex = value;
            }

        }


        public bool IsActived
        {
            get => _isActived;
            set
            {
                if (value == _isActived) return;
                _isActived = value;
            }
        }

        public string Des
        {
            get => _des;
            set
            {
                if (value == _des) return;
                _des = value;
            }
        }

        public string FnPath
        {
            get => _fnPath;
            set
            {
                if (value == _fnPath) return;
                _fnPath = value;
            }
        }

        public string Cs
        {
            get => _cs;
            set
            {
                if (value == _cs) return;
                _cs = value;
            }
        }

        public override string ToString()
        {
            return IsActived + "," + Des + "," + FnPath + "," + Cs;
        }
    }
    public class FileItem : ImgItemInfo,INotifyPropertyChanged
    {
        private string _des;
        private string _fnPath;
        public bool IsActived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Des
        {
            get => _des;
            set
            {
                if (value == _des) return;
                _des = value;
                RaisePropertyChanged();
            }
        }
        public string FnPath
        {
            get => _fnPath;
            set
            {
                if (value == _fnPath) return;
                _fnPath = value;
                RaisePropertyChanged();
            }
        }
        public string Cs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ImgOpState ImgOpState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int FileIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public enum ImgOpState
    {
        None,
        Success,
        Fail,
    }
}