using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Cs.Model.Data
{
    public class ClientInformationModel
    {
        public string Guid { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime DateTimeConnected { get; set; }
        public DateTime DateTimeLast { get; set; }
        public string IpAdress { get; set; }
        public string IpPort { get; set; }
        public bool Authenticated { get; set; }
        public Socket Soc { get; set; }
    }
}
