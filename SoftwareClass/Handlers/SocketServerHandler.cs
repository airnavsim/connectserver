using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;


namespace Cs.Software.Handlers
{
    public class SocketServerHandler
    {
        private string ZDebug { get; set; }

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public Cs.Debug Debug { get; set; }

        public SimConnector.ISimInterface Simulator { get; set; }
        public SocketServerHandler()
        {

        }

        public void Start()
        {
            Debug.Info("Socket server starting");

            Debug.Info($"Port: {Settings.Server.SrvPort}");

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            /// listenSocket.Bind(new IPEndPoint(IPAddress.Loopback, 4444));
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, Convert.ToInt16(Settings.Server.SrvPort)));
            listenSocket.Listen(100);
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += AcceptCallback;
            if (!listenSocket.AcceptAsync(e))
            {
                AcceptCallback(listenSocket, e);
            }
            Console.WriteLine("Socket server Ending");

            // Console.ReadKey(true);
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine("AcceptCallback running");
            Socket listenSocket = (Socket)sender;
            do
            {
                try
                {
                    Socket newSocket = e.AcceptSocket;

                    // var aa = newSocket.RemoteEndPoint as IPEndPoint;
                    //+ TODO 
                    //  Check ip adress so that it can connect.

                    // Thread.Sleep(2000);
                    newSocket.Send(Encoding.ASCII.GetBytes($"ConnectServer {Settings.VersionInformation.AppVersionString}\r\n"));

                    


                    Task.Run(() => ReadDataLoop(newSocket));

                }
                catch
                {
                    // handle any exceptions here;
                }
                finally
                {
                    e.AcceptSocket = null; // to enable reuse
                }
            } while (!listenSocket.AcceptAsync(e));

            Console.WriteLine("AcceptCallback ending");
        }


        // private void ReadDataLoop(Socket soc, string guid)
        /// private void ReadDataLoop(Model.Data.ClientInformationModel client)
        private void ReadDataLoop(Socket soc)
        {
            var aa = soc.RemoteEndPoint as IPEndPoint;
            string TmpRespondMessage = $"Hello {aa.Address.ToString()}\r\n";
            soc.Send(Encoding.ASCII.GetBytes(TmpRespondMessage));

            string Cmd = "";
            string ClientId = "";
            string ClientPw = "";
            bool ClientAuthenticated = false;
            while (true)
            {
                if (!soc.Connected)
                {
                    ClientAuthenticated = false;
                    break;
                }
                    
                soc.Send(Encoding.ASCII.GetBytes("Authenticate\r\n"));
                Cmd = ReadData(soc).ToLower().Trim();
                

                if (Cmd.Contains(":"))
                {
                    string[] input = Cmd.Split(':');
                    ClientId = input[0];
                    ClientPw = input[1];
                    if (Settings.Server.SrvPw.ToLower() == ClientPw)
                    {
                        ClientAuthenticated = true;
                        break;
                    }
                        
                }

                //+ Auto allow all now - debug only
                //ClientAuthenticated = true;
                //break;


                if ((Cmd == "logoff") || (Cmd == "end") || (Cmd == "quit"))
                    break;
                Console.WriteLine(Cmd);
            }

            if (ClientAuthenticated)
            {
                
                var ss = System.Guid.NewGuid().ToString();
                while (true)
                {
                    if (!Settings.Data.Clients.ContainsKey(ss))
                        break;

                    ss = System.Guid.NewGuid().ToString();
                }

                var client = new Model.Data.ClientInformationModel()
                {
                    Guid = ss,
                    Authenticated = false,
                    Soc = soc,
                    ClientId = ClientId,
                    DateTimeConnected = DateTime.UtcNow,
                    DateTimeLast = DateTime.UtcNow,
                    IpAdress = aa.Address.ToString(),
                    IpPort = aa.Port.ToString(),
                    ClientName = "nodata"
                };

                Settings.Data.Clients.Add(ss, client);

                client.Soc.Send(Encoding.ASCII.GetBytes("Authenticated\r\n"));
                //  Send status to client about system
                this.SendStatus(client.Soc);

                Debug.Info($"Client id:{client.ClientId} | Name: {client.ClientName} is Authenticated");

                Boolean testvalie = false;
                while (true)
                {
                    if (!soc.Connected)
                        break;

                    string xxx = "";
                    xxx = ReadData(soc).ToLower().Trim();
                    if (string.IsNullOrEmpty(xxx))
                        continue;

                    client.DateTimeLast = DateTime.UtcNow;

                    if (xxx == "quit")
                    {
                        // Settings.Data.Clients.Remove(guid);

                        //TODO  Remove clientId from all sensors that it is monitoring
                        break;
                    }
                    else if (xxx == "hej")
                    {
                        if (testvalie)
                            testvalie = false;
                        else
                            testvalie = true;

                        soc.Send(Encoding.ASCII.GetBytes($"Value: change"));
                    }
                    else if (xxx == "aa")
                    {
                        soc.Send(Encoding.ASCII.GetBytes($"Value: {testvalie.ToString()}"));
                    }
                    else if (xxx.StartsWith("add:"))
                    {
                        //  Add Connected client to sensor update.
                        client.Soc.Send(Encoding.ASCII.GetBytes(this.SensorAdd(xxx, client.Guid)));
                        
                        // Settings.Data.Sensors["sim/time/total_flight_time_sec"].ClientUpdates.Add(guid);
                        // var data = "value:" + "sim/time/total_flight_time_sec" + ":" + Settings.Data.Sensors["sim/time/total_flight_time_sec"]?.Value + "\r\n";
                        // soc.Send(Encoding.ASCII.GetBytes(data));

                    }


                    // Console.WriteLine("{0}:{1}", client.Guid, xxx);
                }

            }

            //if (ClientLogoff)
            //{
            //    soc.Dispose();
            //    return;
            //}
                

            

            soc.Dispose();

            Console.WriteLine("ReadDataLoop Ending");

        }
        private string SensorAdd(string command, string ClientId)
        {
            //add:<sensorId>
            string[] aa = command.Split(':');
            if (aa.Length == 2)
            {
                ZDebug = "dsfdsf";
                ulong abb = Convert.ToUInt64(aa[1]);
                ZDebug = "dsfdsf";
                return this.Simulator.Subscribe(abb, ClientId);
            }

            ZDebug = "dsfdsf";
            return "hejhejochhej\r\n";
            
        }

        private void SendStatus(Socket client)
        {
            client.Send(Encoding.ASCII.GetBytes($"SimulatorConnected: {Settings.Simulator.Connected.ToString()}\r\n"));
        }
        private static string ReadData(Socket client)
        {
            string retVal;
            byte[] data = new byte[1024];

            // NetworkStream stream = client.re.Receive(); // .GetStream();


            byte[] myReadBuffer = new byte[1024];
            StringBuilder myCompleteMessage = new StringBuilder();
            int numberOfBytesRead = 0;

            // byte[] buffer = new byte[1024];
            //    newSocket.Receive(buffer, 0); // , 2); // , newSocket.ReceiveBufferSize);

            //    message = Encoding.ASCII.GetString(buffer);
            //    Console.WriteLine(message);


            //do
            while (true)
            {
                try
                {
                    numberOfBytesRead = client.Receive(myReadBuffer, 0);
                    // numberOfBytesRead = client.Receive(myReadBuffer, client.ReceiveBufferSize, SocketFlags.None); // , 0); // , myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                    
                }
                catch
                {
                    break;
                }

                if (myCompleteMessage.ToString().EndsWith("\r\n")) break;

            }
            // "\r\n\r\ndfsdfsdfsd\r\ndsfsdfsdfksdljfsdf\r\ndsfdsfsdf\r\n<end>"
            // while (client.ReceiveBufferSize > numberOfBytesRead);
            //while (stream.DataAvailable);

            // client.ReceiveBufferSize



            retVal = myCompleteMessage.ToString();
            retVal = retVal.Replace("\r\n", "");

            return retVal;
        }

        public void SendValuesToConnectedClients(string sensorName)
        {
            Console.WriteLine("????????" + sensorName);
            //foreach (string entry in Settings.Data.Sensors[sensorName]?.ClientUpdates)
            //{

            //    Console.WriteLine("client: {0}", entry.ToString());
            //    //if (Settings.Data.Clients.ContainsKey(entry))
            //    //{
            //    //    if (Settings.Data.Clients[entry].Client.Connected)
            //    //    {
            //    //        var data = "value:" + sensorName + ":" + Settings.Data.Sensors[sensorName]?.Value + "\r\n";
            //    //        Settings.Data.Clients[entry].Client.Send(Encoding.ASCII.GetBytes(data));
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    //  Client dont exist!!! remove client updates from sensor list.
            //    //    var itemToRemove = Settings.Data.Sensors[sensorName].ClientUpdates.Remove(entry);
            //    //    //.SingleOrDefault(r => r.Id == 2);
            //    //    // if (itemToRemove != null)
            //    //    // resultList.Remove(itemToRemove);
            //    //}

            //    // newSocket.Send(Encoding.ASCII.GetBytes("Hello socket!"));
            //}
        }

    }




}
