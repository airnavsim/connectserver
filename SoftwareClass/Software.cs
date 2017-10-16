using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Software
{
    public class Software
    {
        private string ZDebug = "sdfsdf";
        public Cs.Debug Debug { get; set; }

        private SimConnector.SimInterface Simulator { get; set; }
        public void Run()
        {

            this.Debug.Message("Class starting");


            //  Sett settings to xplane.
            //  TODO read from database insted.

            Settings.Simulator.SimType = SettingsModel.Handlers.SimTypeEnum.xplane11;
            Settings.Simulator.Host = "172.16.100.144";
            Settings.Simulator.Port = 51000;



            if (Cs.Settings.DebugToConsole)
            {
                ZDebug = "sdfsdf";
            }
            
            if (Settings.DebugToConsole)
            {
                ZDebug = "sdfdsf";
            }

            
        }

        private void ConnectToSimulator()
        {
            if (Settings.Simulator.SimType == SettingsModel.Handlers.SimTypeEnum.xplane11)
                this.Simulator = new SimConnector.Xplane();

            this.Simulator.Connect();
        }
    }
}
