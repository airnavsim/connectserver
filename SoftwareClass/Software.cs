using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Software
{
    public class Software
    {
        private string ZDebug = "sdfsdf";
        public Cs.Debug Debug { get; set; }

        private SimConnector.ISimInterface Simulator { get; set; }
        public void Run()
        {

            this.Debug.Info("Class starting");


            //  Sett settings to x plane.
            //  TODO read from database instead.

            var asd = new Handlers.SettingsHandler();
            asd.Debug = this.Debug;
            if (!asd.Init())
            {
                //  Error reading settings.

                ZDebug = "sdfdsf";
            }

            var sdfdsf = Settings.Database;

            #region Check database connection and version of database


            #endregion

            ZDebug = "sdfdsf";


            //Settings.Simulator.SimType = SettingsModel.Handlers.SimTypeEnum.xplane11Ext;
            //Settings.Simulator.Host = "172.16.100.88";
            //Settings.Simulator.Port = 51000;



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
            if (Settings.Simulator.SimType == SettingsModel.Models.SimTypeEnum.nosim)
            {

            }
            if (Settings.Simulator.SimType == SettingsModel.Models.SimTypeEnum.xplane11Ext)
            {
                this.Simulator = new SimConnector.XplaneExt();
            }
                
            if (this.Simulator != null)
            {
                this.Simulator.Connect();
                
            }
                
        }
    }
}
