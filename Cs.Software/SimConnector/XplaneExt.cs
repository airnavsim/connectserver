using System;
using System.Collections.Generic;
using System.Text;


/*
 * can this be used?
 * https://github.com/eikcam/XPlaneUdpData
 * */
namespace Cs.Software.SimConnector
{
    public class XplaneExt : ISimInterface
    {
        public Cs.Debug Debug { get; set; }

        private ExtPlaneNetCore.ExtPlaneInterface ExtPlane { get; set; }
        public void Connect()
        {
            Settings.Simulator.Connected = false;

            if (this.ExtPlane == null)
                this.ExtPlane = new ExtPlaneNetCore.ExtPlaneInterface();

            this.ExtPlane.UpdateInterval = 0.33f;
            this.ExtPlane.Host = Settings.Simulator.Host;
            this.ExtPlane.Port = Settings.Simulator.Port;

            if (Settings.Simulator.ConnectedShodBe)
            {
                this.ExtPlane.Connect();
                if (this.ExtPlane.IsConnected())
                {
                    Debug.Info("sim connected!!");
                    Settings.Simulator.Connected = true;
                    Settings.Simulator.DateTimeConnected = DateTime.UtcNow;
                    


                    //  Auto subscribe this.
                    Debug.Info("subscribe default sensors");
                    foreach(var aa in Settings.Simulator.SensorsAutoCollect)
                    {
                        this.Subscribe(aa, null);
                    }
                    //this.Subscribe(51, null);
                    //this.Subscribe(110, null);
                    //this.Subscribe(210, null);
                    
                    
                }
                else
                {
                    Debug.Info("Sim not connected");
                    Settings.Simulator.Connected = false;
                    Settings.Simulator.InFlight = false;
                    Settings.Simulator.InFlightId = 0;
                }
                    
            }

        }

        public void Disconnect()
        {
            if (this.ExtPlane != null)
            {
                if (this.ExtPlane.IsConnected())
                {

                    this.ExtPlane.Disconnect();
                    Debug.Info("Simulator disconnected");
                }
            }
        }

        public bool IsConnected()
        {
            if (this.ExtPlane == null)
                return false;

            if (this.ExtPlane != null)
            {
                var dfdsf = this.ExtPlane.IsConnected();

                var dsfdsf = "sdfdsf";
                return this.ExtPlane.IsConnected();
            }
                

            return false;

        }


        public string Subscribe(ulong sensorId, string clientGuid)
        {
            if (!this.ExtPlane.IsConnected())
            {
                return "error:nosim\r\n";
            }

            //  get sensor informaton
            var aa = this.GetSensorInfo(sensorId);
            if (aa == null)
            {
                // TODO  Load from sensor id from database
                //  dont exist
                return "supdate:{sensorId}:dontexist\r\n";
            }

            if (!string.IsNullOrEmpty(clientGuid))
            {
                if (!aa.CollectingClientsGuid.Contains(clientGuid))
                {
                    //  Client dont exist in sensorid Update system. Add
                    aa.CollectingClientsGuid.Add(clientGuid);
                }
            }


            if (!aa.CollectingEnable)
            {
                //  Collection of this sensor is not enable. enable it.

                aa.CollectingEnable = true;

                try
                {
                    this.ExtPlane.Subscribe<string>(aa.SimCommand, aa.Subscribeaccuracy, (dataRef) =>
                    {
                        SubscribeReturn(sensorId, dataRef.Name, dataRef.Value);
                    });
                }
                catch
                {
                    var dsfdsf = "fsdf";
                }


                //this.ExtPlane.Subscribe<string>(entry.Key, entry.Value.Subscribeaccuracy, (dataRef) =>
                //{
                //    ExtPlaneSubscribeReturnData(dataRef.Name, dataRef.Value);
                //});
                return $"supdate:{sensorId}:add\r\n";
            }
            if (aa._ValueExist)
                return $"supdate:{sensorId}:add\r\nsvalue:{sensorId}:{aa._Value}\r\n";


            return $"supdate:{sensorId}:add\r\n";



            // return null;
        }
        private void SubscribeReturn(ulong sensorId, string refName, string refValue)
        {
            var aa = this.GetSensorInfo(sensorId);
            aa.CollectingLast = DateTime.UtcNow;
            if (aa._Value != refValue)
            {
                aa._ValueLast = aa._Value;
                aa._Value = refValue;
                aa._ValueLastUpdated = DateTime.UtcNow;
                aa._ValueExist = true;

                
                List<string> newList = new List<string>(aa.CollectingClientsGuid);
                // foreach (var clUpdate in aa.CollectingClientsGuid)
                foreach (var clUpdate in newList)
                {
                    try
                    {
                        if (Settings.Data.Clients[clUpdate].Soc.Connected)
                        {
                            Settings.Data.Clients[clUpdate].Soc.Send(Encoding.ASCII.GetBytes($"svalue:{sensorId}:{refValue}\r\n"));
                            // client.Send(Encoding.ASCII.GetBytes($"SimulatorConnected: {Settings.Simulator.Connected.ToString()}\n"));
                        }
                    }
                    catch
                    {
                        Debug.Warning("Error sending to client");
                    }
                }
            }

            //  update settings simulator when last message was recived
            Settings.Simulator.DateTimeLastMessage = DateTime.UtcNow;

            // Debug.Info($"{sensorId} - {refName} - {refValue}");
        }
        public string UnSubscribe(ulong sensorId, string clientGuid)
        {
            return "error:notimp";
        }

        private Model.Data.SensorModel GetSensorInfo(ulong sensorId)
        {
            if (Settings.Data.Sensors.ContainsKey(sensorId))
            {
                return Settings.Data.Sensors[sensorId];
            }

            // TODO  Check if sensor exist in database.

            return null;
        }


        public void test()
        {
            
        }

        public string GetValue(ulong sensorId)
        {
            throw new NotImplementedException();
        }
    }
}
