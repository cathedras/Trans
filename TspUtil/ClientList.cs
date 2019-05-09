using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
                OnPropertyChanged();
            }
        }

        public IDev Dev { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
