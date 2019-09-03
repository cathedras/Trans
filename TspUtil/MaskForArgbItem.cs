using System.ComponentModel;
using System.Runtime.CompilerServices;
using TspUtil.Annotations;

namespace TspUtil
{
    public class MaskForArgbItem : INotifyPropertyChanged
    {
        private string _expRGBA ;

        public MaskForArgbItem()
        {
        }

        public MaskForArgbItem(string expRGBA)
        {
            _expRGBA = expRGBA;
        }


        public string ExpRGBA
        {
            get => _expRGBA;
            set
            {
                if (value==_expRGBA)return;
                _expRGBA = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}