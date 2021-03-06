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

            #region Set default sensors to read
            Settings.Simulator.SensorsAutoCollect = new List<ulong>
            {
                1,               //   Latitude
                2,               //   longitude
                10,             //  Simulator running time in sec
                51,              //   heading mag compass
                110,             //  airspeed
                120,            //  ground speed
                210,             //  Altitude
                601,             //  Fuel flow
                700,             //  Weight total
                5000,5001
            };

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

            System.Threading.Thread.Sleep(5000);
            DateTime StatusToClientLastSend = Convert.ToDateTime("2000-01-01 01:01:01");
            Dictionary<string, DateTime> Timers = new Dictionary<string, DateTime>();
            Boolean SimulatorReconnect = false;

            List<double> TempFulesData = new List<double>();

            while (true)
            {
                if (!this.Simulator.IsConnected())
                {
                    Debug.Warning("No simulator connected");
                    //  Simulator is not online. can we get it online.
                    this.Simulator.Connect();
                }


                // var dddf = this.Simulator.IsConnected();

                if ((DateTime.UtcNow - StatusToClientLastSend).TotalSeconds >=60)
                {

                
                    foreach (var cl in Settings.Data.Clients)
                    {
                        Server.SendMessageToClient(cl.Key, "status", true);
                    }
                    StatusToClientLastSend = DateTime.UtcNow;

                }


                if (this.Simulator.IsConnected())
                {
                    if (!Timers.ContainsKey("fuel"))
                        Timers.Add("fuel", DateTime.UtcNow);
                    if (!Timers.ContainsKey("dbsaveflight"))
                        Timers.Add("dbsaveflight", DateTime.UtcNow);


                    if (!Settings.Simulator.InFlight)
                    {
                        if ((Settings.Data.Sensors[110] != null) && (Settings.Data.Sensors[210] != null))
                        {

                            if ((Settings.Data.Sensors[110]._ValueExist) && (Settings.Data.Sensors[210]._ValueExist))
                            {
                                //  Debug code
                                var ddsfsdf = Settings.Data.Sensors[110]._Value.Replace(".", ",");
                                var dsfsdfg = Settings.Data.Sensors[210]._Value.Replace(".", ",");



                                if ((Convert.ToDouble(Settings.Data.Sensors[110]._Value.Replace(".", ",")) >= 30) && (Convert.ToDouble(Settings.Data.Sensors[210]._Value.Replace(".", ",")) >= 500))
                                {
                                    Settings.Simulator.InFlight = true;
                                    Debug.Info("Inflight mode active");
                                }
                            }
                            
                        }
                    }

                    // Console.WriteLine($"sensor: {Settings.Data.Sensors[51].Id}  (heading)  exist: {Settings.Data.Sensors[51]._ValueExist.ToString()}  Value:  {Settings.Data.Sensors[51]._Value}");
                    //Console.WriteLine($"sensor: {Settings.Data.Sensors[110].Id} (speed)    exist: {Settings.Data.Sensors[110]._ValueExist.ToString()}  Value:  {Settings.Data.Sensors[110]._Value}");
                    //Console.WriteLine($"sensor: {Settings.Data.Sensors[210].Id} (altitude) exist: {Settings.Data.Sensors[210]._ValueExist.ToString()}  Value:  {Settings.Data.Sensors[210]._Value}");



                    // Fuel calculator
                    //if ((Settings.Data.Sensors[601] != null) && (Settings.Data.Sensors[700] != null))
                    //{
                    //    if (!string.IsNullOrEmpty(Settings.Data.Sensors[601]._Value))
                    //    {
                    //        // Console.WriteLine(Settings.Data.Sensors[601]._Value);

                    //        string[] aa = Settings.Data.Sensors[601]._Value.Split('|');
                    //        double TotValue = 0;
                    //        foreach (var ab in aa)
                    //        {
                    //            TotValue += Convert.ToDouble(ab.Replace(".",","));
                    //        }

                    //        Console.WriteLine($"Fuel flow: {TotValue * 60} kg per min | Weight: {Settings.Data.Sensors[700]._Value}");
                    //    }
                    //}

                    #region Infligt operations
                    if (Settings.Simulator.InFlight)
                    {
                        #region Collect fuel usage every 10 sek
                        if (Settings.Data.Sensors[601] != null)
                        {
                            if ((DateTime.UtcNow - Timers["fuel"]).TotalSeconds >= 10)
                            {
                                string[] aa = Settings.Data.Sensors[601]._Value.Split('|');
                                double TotValue = 0;
                                foreach (var ab in aa)
                                {
                                    TotValue += Convert.ToDouble(ab.Replace(".", ","));
                                }
                                TempFulesData.Add(TotValue);
                                Timers["fuel"] = DateTime.UtcNow;
                            }
                        }
                        #endregion


                        #region Save flight info to database.
                        if ((DateTime.UtcNow - Timers["dbsaveflight"]).TotalSeconds >= 60)
                        {
                            string latitude, longitude, heading, speed, speedgs, altitude, fuelflow, weightTotal;

                            if (Settings.Data.Sensors[1] != null)
                                latitude = Settings.Data.Sensors[1]._Value;
                            else
                                latitude = "nodata";

                            if (Settings.Data.Sensors[2] != null)
                                longitude = Settings.Data.Sensors[2]._Value;
                            else
                                longitude = "nodata";


                            if (Settings.Data.Sensors[51] != null)
                                heading = Settings.Data.Sensors[51]._Value;
                            else
                                heading = "nodata";

                            if (Settings.Data.Sensors[110] != null)
                                speed = Settings.Data.Sensors[110]._Value;
                            else
                                speed = "nodata";

                            if (Settings.Data.Sensors[120] != null)
                                speedgs = Settings.Data.Sensors[120]._Value;
                            else
                                speedgs = "nodata";

                            if (Settings.Data.Sensors[210] != null)
                                altitude = Settings.Data.Sensors[210]._Value;
                            else
                                altitude = "nodata";

                            if (TempFulesData.Count == 0)
                                fuelflow = "nodata";
                            else
                            {
                                double Flow = 0;
                                foreach (var fdata in TempFulesData)
                                {
                                    Flow += fdata;
                                }
                                fuelflow = ((Flow / TempFulesData.Count) * 60).ToString().Replace(",", ".");
                                TempFulesData.Clear();
                            }

                            //  Fuel flow information.



                            if (Settings.Data.Sensors[700] != null)
                                weightTotal = Settings.Data.Sensors[700]._Value;
                            else
                                weightTotal = "nodata";

                            this.DaServerData.TblFlightInfoTemp_InsertNewRow(Settings.Simulator.InFlightId.ToString(), latitude, longitude, speed, speedgs, altitude, heading, fuelflow, weightTotal);
                            Timers["dbsaveflight"] = DateTime.UtcNow;
                        }
                        #endregion

                    }
                    #endregion


                    if ((DateTime.UtcNow - Settings.Simulator.DateTimeLastMessage).TotalSeconds >= 20)
                    {
                        Console.WriteLine("no simulator message for more then 20 sek!!!");
                        // var dsfd = this.Simulator.IsConnected();
                        this.Simulator.Disconnect();
                        Settings.Simulator.Connected = false;
                        SimulatorReconnect = true;
                        ZDebug = "sdfdsf";
                    }



                    if (Settings.Data.Sensors[5000] != null)
                    {
                        if (Settings.Data.Sensors[5000]._Value != null)
                        {
                            Console.WriteLine($"5000 - {Settings.Data.Sensors[5000]._Value.ToString()}");
                        }
                            
                    }
                    if (Settings.Data.Sensors[5001] != null)
                    {
                        if (Settings.Data.Sensors[5001]._Value != null)
                        {
                            Console.WriteLine($"5001 - {Settings.Data.Sensors[5001]._Value.ToString()}");
                        }

                    }
                }

                
                /*
                 *                     this.Subscribe(51, null);
                    this.Subscribe(110, null);
                    this.Subscribe(210, null);
                 * */


                System.Threading.Thread.Sleep(5000);
                //Debug.Info($"Connected clients: {Settings.Data.Clients.Count}");
                //foreach(var aa in Settings.Data.Clients)
                //{
                //    Debug.Info($"Id: {aa.Value.ClientId} - last con: {aa.Value.DateTimeLast.ToString()}");
                //}
                

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



            #region old system


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

            #endregion


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

