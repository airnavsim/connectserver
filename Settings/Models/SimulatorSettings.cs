using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.SettingsModel.Models
{
    public class SimulatorSettings
    {
        public SimTypeEnum SimType { get; set; }
        
        public bool Connected { get; set; }
        public bool ConnectedShodBe { get; set; }
        public bool InFlight { get; set; }
        public ulong InFlightId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }

        public DateTime DateTimeConnected { get; set; }
        public DateTime DateTimeLastMessage { get; set; }
    }
    public enum SimTypeEnum { nosim = 0, xplane11Ext = 1}
}
