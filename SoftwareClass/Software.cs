using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Software
{
    public class Software
    {
        private string ZDebug = "sdfsdf";
        public Cs.Debug Debug { get; set; }

        private Cs.Communication.Database.DataAccess.DaServerData DaServerData { get; set; }
        private SimConnector.ISimInterface Simulator { get; set; }
        private Handlers.SocketServerHandler Server { get; set; }
        public void Run()
        {
            Settings.VersionInformation = new Model.Settings.VersionInformationModel()
            {
                AppVersion = 0,
                AppRelease = 0,
                AppBuild = 1,
                DbVersionAtleast = 1,
                DbVersionRightNow = 0
            };


            this.DaServerData = new Cs.Communication.Database.DataAccess.DaServerData();

            this.Debug.Info("Class starting");


            #region Read settings from config file
           
            var asd = new Handlers.SettingsHandler();
            asd.Debug = this.Debug;
            if (!asd.Init())
            {
                //  Error reading settings.
                Debug.Error("Error reading settings");
                return;
            }

            #endregion

            #region Get settings from database and check database version.

            this.GetSettingsFromDatabase();

            if (Settings.VersionInformation.DbVersionRightNow != Settings.VersionInformation.DbVersionAtleast)
            {
                Debug.Error("Database wrong version");
                return;
            }

            #endregion

            // var sdfdsf = Settings.Database;


            ZDebug = "sdfdsf";


            var dsfdsf = Settings.Server;


            this.Server = new Handlers.SocketServerHandler();
            this.Server.Debug = this.Debug;
            this.Server.Start();


            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";

            //Settings.Simulator.SimType = SettingsModel.Handlers.SimTypeEnum.xplane11Ext;
            //Settings.Simulator.Host = "172.16.100.88";
            //Settings.Simulator.Port = 51000;

            while (true)
            {
                System.Threading.Thread.Sleep(1000);

            }



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

        private bool GetSettingsFromDatabase()
        {
            //  Get settings from database
            var TmpSettings = DaServerData.TblSetting_GetAll();
            if (!DaServerData.QueryWasDone)
            {
                Debug.Error("Database connection error");
                return false;
            }

            if (TmpSettings.ContainsKey("dbversion"))
            {
                try
                {
                    Settings.VersionInformation.DbVersionRightNow = Convert.ToInt16(TmpSettings["dbversion"]);
                }
                catch
                {
                    Settings.VersionInformation.DbVersionRightNow = 0;
                    Debug.Error("Error reading from database");
                    return false;

                }
                 
            }
            else
            { 
                Settings.VersionInformation.DbVersionRightNow = 0;
                Debug.Error("Database missing values");
                return false;
            }

            return true;
        }
    }
}
