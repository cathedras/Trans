using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TspUtil.Annotations;

namespace TspUtil
{
    /// <summary>
    /// such class is save params for ViewModel class and UI window
    /// </summary>
    public class ClientList : INotifyPropertyChanged
    {
        private int _clientId;
        public string _clientIp;
        private bool _isOffLine;

        public ClientList(IDev dev, int clientId, string clientIp)
        {
            Dev = dev;
            ClientId = clientId;
            ClientIp = clientIp;
        }

        public string ClientIp { get => _clientIp; set => _clientIp = value; }
        public int ClientId { get => _clientId; set => _clientId = value; }

        public bool IsOffLine
        {
            get => _isOffLine;
            set
            {
                _isOffLine = value;
                RaisePropertyChanged();
            }
        }

        public IDev Dev { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LineBreakPoint : INotifyPropertyChanged
    {
        private string _id;
        private bool _breakDown;
        private Brush _isRunning;
        public LineBreakPoint(string id)
        {
            _id = id;
        }
       
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value; 
                RaisePropertyChanged();
            }
        }

        public bool BreakDown
        {
            get { return _breakDown; }
            set
            {
                _breakDown = value; 
                RaisePropertyChanged();
            }
        }

        public Brush IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value; 
                RaisePropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
