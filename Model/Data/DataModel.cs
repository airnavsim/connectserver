using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;


namespace Cs.Model.Data
{
    public class DataModel
    {
       public Dictionary<string, ClientInformationModel> Clients { get; set; }

        public DataModel()
        {
            this.Clients = new Dictionary<string, ClientInformationModel>();
        }

    }
}
