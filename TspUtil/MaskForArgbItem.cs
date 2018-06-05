using System.ComponentModel;
using System.Runtime.CompilerServices;
using TspUtil.Annotations;

namespace TspUtil
{
    public class MaskForArgbItem : INotifyPropertyChanged
    {
        private bool _isA;
        private bool _isR;
        private bool _isG;
        private bool _isB;

        public bool IsA
        {
            get => _isA;
            set
            {
                if (value == _isA) return;
                _isA = value;
                OnPropertyChanged();
            }
        }

        public bool IsR
        {
            get => _isR;
            set
            {
                if (value == _isR) return;
                _isR = value;
                OnPropertyChanged();
            }
        }

        public bool IsG
        {
            get => _isG;
            set
            {
                if (value == _isG) return;
                _isG = value;
                OnPropertyChanged();
            }
        }

        public bool IsB
        {
            get => _isB;
            set
            {
                if (value == _isB) return;
                _isB = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}