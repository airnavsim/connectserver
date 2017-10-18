﻿using System;
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

            if (this.ExtPlane == null)
                this.ExtPlane = new ExtPlaneNetCore.ExtPlaneInterface();

            this.ExtPlane.Host = Settings.Simulator.Host;
            this.ExtPlane.Port = Settings.Simulator.Port;

            this.ExtPlane.Connect();

            if (this.ExtPlane.IsConnected())
            {
                Debug.Info("sim connected!!");
                Settings.Simulator.Connected = true;
                Settings.Simulator.DateTimeConnected = DateTime.UtcNow;
            }
            


        }

        public string Subscribe(ulong sensorId, string clientGuid)
        {
            if (!this.ExtPlane.IsConnected())
            {
                return "error:nosim";
            }

            //  get sensor informaton
            var aa = this.GetSensorInfo(sensorId);
            if (aa == null)
            {
                //  dont exist
                return "error:sensordontexist";
            }

            if (!aa.CollectingClientsGuid.Contains(clientGuid))
            {
                //  Client dont exist in sensorid Update system. Add
                aa.CollectingClientsGuid.Add(clientGuid);
            }

            if (!aa.CollectingEnable)
            {
                //  Collection of this sensor is not enable. enable it.

                aa.CollectingEnable = true;
                
                this.ExtPlane.Subscribe<string>(aa.SimCommand, aa.Subscribeaccuracy, (dataRef) =>
                {
                    SubscribeReturn(sensorId, dataRef.Name, dataRef.Value);
                });

                //this.ExtPlane.Subscribe<string>(entry.Key, entry.Value.Subscribeaccuracy, (dataRef) =>
                //{
                //    ExtPlaneSubscribeReturnData(dataRef.Name, dataRef.Value);
                //});
                return "added";
            }



            return null;
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

                foreach(var clUpdate in aa.CollectingClientsGuid)
                {
                    try
                    {
                        if (Settings.Data.Clients[clUpdate].Soc.Connected)
                        {
                            Settings.Data.Clients[clUpdate].Soc.Send(Encoding.ASCII.GetBytes($"s:{sensorId}:{refValue}\r\n"));
                            // client.Send(Encoding.ASCII.GetBytes($"SimulatorConnected: {Settings.Simulator.Connected.ToString()}\n"));
                        }
                    }
                    catch
                    {
                        Debug.Warning("Error sending to client");
                    }
                }
            }


            Debug.Info($"{sensorId} - {refName} - {refValue}");
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
            return null;
        }
    }
}
