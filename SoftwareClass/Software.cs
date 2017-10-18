﻿using System;
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
            Settings.Data = new Model.Data.DataModel()
            {
                Clients = new Dictionary<string, Model.Data.ClientInformationModel>()
            };

            
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


            //x var dsfdsf = Settings.Server;

           


            // TODO Move this to database settings.
            #region Sett Connection to simulator Manuel. 
            Settings.Simulator = new SettingsModel.Models.SimulatorSettings()
            {
                Connected = false,
                ConnectedShodBe = true,
                Host = "172.16.100.88",
                Port = 51000,
                SimType = SettingsModel.Models.SimTypeEnum.xplane11Ext
            };

            #endregion

            #region Load sensors data
            this.SensorDataLoad();
            #endregion

            this.SimulatorInit();

            #region Start socket server
            Debug.Info("Server is starting");
            this.Server = new Handlers.SocketServerHandler();
            this.Server.Debug = this.Debug;
            this.Server.Simulator = this.Simulator;
            this.Server.Start();
            Debug.Info("Server is started");
            #endregion


            this.ConnectToSimulator();


            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";
            ZDebug = "sdfdsf";


            //Settings.Simulator.SimType = SettingsModel.Handlers.SimTypeEnum.xplane11Ext;
            //Settings.Simulator.Host = "172.16.100.88";
            //Settings.Simulator.Port = 51000;

            var dsfdsf = Settings.Simulator;


            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                Debug.Info($"Connected clients: {Settings.Data.Clients.Count}");
                foreach(var aa in Settings.Data.Clients)
                {
                    Debug.Info($"Id: {aa.Value.ClientId} - last con: {aa.Value.DateTimeLast.ToString()}");
                }
                

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

        private void SimulatorInit()
        {
            if (Settings.Simulator.SimType == SettingsModel.Models.SimTypeEnum.nosim)
            {

            }
            if (Settings.Simulator.SimType == SettingsModel.Models.SimTypeEnum.xplane11Ext)
            {
                this.Simulator = new SimConnector.XplaneExt();
            }
        }
        private void ConnectToSimulator()
        {
           
                
            if (this.Simulator != null)
            {
                this.Simulator.Debug = this.Debug;
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

        private void SensorDataLoad()
        {
            Settings.Data.Sensors = new Dictionary<ulong, Model.Data.SensorModel>();

            this.SensorReloadFromDatabase();


            //Settings.Data.Sensors.Add(1, new Model.Data.SensorModel()
            //{
            //    CollectingClientsGuid = new List<string>(),
            //    CollectingEnable = false,
            //    Id = 1,
            //    Name = "latitude",
            //    SimCommand = "sim/flightmodel/position/latitude",
            //    Subscribeaccuracy = 0.00001f,
            //    _ValueExist = false
            //});
            //Settings.Data.Sensors.Add(2, new Model.Data.SensorModel()
            //{
            //    CollectingClientsGuid = new List<string>(),
            //    CollectingEnable = false,
            //    Id = 2,
            //    Name = "longitude",
            //    SimCommand = "sim/flightmodel/position/longitude",
            //    Subscribeaccuracy = 0.00001f,
            //    _ValueExist = false
            //});
            //Settings.Data.Sensors.Add(3, new Model.Data.SensorModel()
            //{
            //    CollectingClientsGuid = new List<string>(),
            //    CollectingEnable = false,
            //    Id = 3,
            //    Name = "wind",
            //    SimCommand = "sim/weather/wind_speed_kt",
            //    Subscribeaccuracy = 0.01f,
            //    _ValueExist = false
            //});
            //Settings.Data.Sensors.Add(4, new Model.Data.SensorModel()
            //{
            //    CollectingClientsGuid = new List<string>(),
            //    CollectingEnable = false,
            //    Id = 4,
            //    Name = "wind",
            //    SimCommand = "sim/weather/wind_speed_kt[2]",
            //    Subscribeaccuracy = 0.01f,
            //    _ValueExist = false
            //});
        }

        private void SensorReloadFromDatabase()
        {
            //  Get all sensors rows from database
            var TmpSensordb = DaServerData.TblSensors_GetAll();

            foreach(var aa in TmpSensordb)
            {
                if (Settings.Data.Sensors.ContainsKey(aa.Id))
                {
                    //  sensor already loaded. 
                    //+ TODO  check if data need change.
                }
                else
                {
                    var TmpModel = new Model.Data.SensorModel()
                    {
                        Id = aa.Id,
                        Group = aa.Group,
                        Name = aa.Name,
                        SimCommand = aa.Xplane11Ext,
                        CollectingClientsGuid = new List<string>(),
                        _ValueExist = false,
                        CollectingEnable = false
                    };

                    TmpModel.Subscribeaccuracy = aa.Accuracy;

                    // var dsfdsf = aa.Accuracy;
                    // ZDebug = "sdfdsf";



                    Settings.Data.Sensors.Add(aa.Id, TmpModel);
                }
            }

        }


        /*
         *             #region Position
            Settings.Data.Sensors.Add("sim/flightmodel/position/latitude", new Settings.Classes.Server.DataSensorClass("sim/flightmodel/position/latitude", true) { Subscribeaccuracy = 0.1f });
            Settings.Data.Sensors.Add("sim/flightmodel/position/longitude", new Settings.Classes.Server.DataSensorClass("sim/flightmodel/position/longitude", true) { Subscribeaccuracy = 0.1f });
            #endregion 

            //  Speed information

            // Air speed indicated - this takes into account air density and wind direction
            Settings.Data.Sensors.Add("sim/flightmodel/position/indicated_airspeed", new Settings.Classes.Server.DataSensorClass("sim/flightmodel/position/indicated_airspeed", true) { Subscribeaccuracy = 1.0f });
            Settings.Data.Sensors.Add("sim/flightmodel/position/indicated_airspeed2", new Settings.Classes.Server.DataSensorClass("sim/flightmodel/position/indicated_airspeed2", true) { Subscribeaccuracy = 1.0f });
            //  Air speed true - this does not take into account air density at altitude!
            Settings.Data.Sensors.Add("sim/flightmodel/position/true_airspeed", new Settings.Classes.Server.DataSensorClass("sim/flightmodel/position/true_airspeed", true) { Subscribeaccuracy = 1.0f });

         * 
         * */
    }
}
