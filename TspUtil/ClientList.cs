using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspUtil
{
    /// <summary>
    /// such class is save params for ViewModel class and UI window
    /// </summary>
    public class ClientList
    {
        private int _clientId;
        public string _clientIp;
        private bool isConnect;

        public ClientList(int clientId, string clientIp, bool isConnected)
        {
            ClientId = clientId;
            ClientIp = clientIp;
            IsConnect = isConnect;
        }

        public string ClientIp { get => _clientIp; set => _clientIp = value; }
        public int ClientId { get => _clientId; set => _clientId = value; }
        public bool IsConnect { get => isConnect; set => isConnect = value; }
    }
}
