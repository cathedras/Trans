using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myzy.Util
{
    public class LogItem : INotifyPropertyChanged
    {
        public LogItem()
        {
            DateTime = DateTime.Now;
        }
        private DateTime _dateTime;
        private string _info;
        private int _level;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime == value) return;

                _dateTime = value;
                OnPropertyChanged();
            }
        }

        public string Info
        {
            get { return _info; }
            set
            {
                if (value == _info) return;
                _info = value;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                if (value == _level) return;
                _level = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
