using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.SettingsModel.Handlers
{
    public class SimulatorSettings
    {
        public SimTypeEnum SimType { get; set; }
        // public XplaneSettings Xplane { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }

        public DateTime ConnectedTime { get; set; }
        public DateTime LastMessage { get; set; }
    }
    public enum SimTypeEnum { xplane10 = 0, xplane11 = 1}
}
