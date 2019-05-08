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

        public ClientList(int clientId, string clientIp)
        {
            ClientId = clientId;
            ClientIp = clientIp;
        }

        public string ClientIp { get => _clientIp; set => _clientIp = value; }
        public int ClientId { get => _clientId; set => _clientId = value; }
    }
}
