using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Software.SimConnector
{
    public interface ISimInterface
    {
        Cs.Debug Debug { get; set; }
        void Connect();

        string Subscribe(ulong sensorId, string clientGuid);

        string UnSubscribe(ulong sensorId, string clientGuid);
    }
}
