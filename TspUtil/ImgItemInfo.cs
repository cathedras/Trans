using System.ComponentModel;
using System.Runtime.CompilerServices;
using TspUtil.Annotations;

namespace TspUtil
{
    public class ImgItemInfo  : INotifyPropertyChanged
    {
        private bool _isActived;
        private string _des;
        private string _fnPath;
        private string _cs;
        private ImgOpState _imgOpState;

        public ImgOpState ImgOpState
        {
            get => _imgOpState;
            set
            {
                if (value == _imgOpState) return;
                _imgOpState = value;
                OnPropertyChanged();
            }
        }


        public bool IsActived
        {
            get => _isActived;
            set
            {
                if (value == _isActived) return;
                _isActived = value;
                OnPropertyChanged();
            }
        }

        public string Des
        {
            get => _des;
            set
            {
                if (value == _des) return;
                _des = value;
                OnPropertyChanged();
            }
        }

        public string FnPath
        {
            get => _fnPath;
            set
            {
                if (value == _fnPath) return;
                _fnPath = value;
                OnPropertyChanged();
            }
        }

        public string Cs
        {
            get => _cs;
            set
            {
                if (value == _cs) return;
                _cs = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return IsActived + "," + Des +","+ FnPath +","+ Cs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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