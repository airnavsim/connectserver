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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public Cs.Debug Debug { get; set; }

        public SocketServerHandler()
        {

        }

        public void Start()
        {
            Debug.Info("Socket server starting");
            

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Loopback, 4444));
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

                    var aa = newSocket.RemoteEndPoint as IPEndPoint;
                    //+ TODO 
                    //  Check ip adress so that it can connect.

                    // var ss = System.Guid.NewGuid().ToString();

                    //while (true)
                    //{
                    //    if (!Settings.Data.Clients.ContainsKey(ss))
                    //        break;
                        
                    //    ss = System.Guid.NewGuid().ToString();
                    //}

                    //if (Settings.Data.Clients == null)
                    //{
                    //    var dsfdsf = "sdfsdf";
                    //}
                    
                    //Settings.Data.Clients.Add(ss, new Model.Data.ClientInformationModel()
                    //{
                    //    Guid = ss,
                    //    Authenticated = false,
                    //    Client = newSocket,
                    //    ClientId = "nodata",
                    //    DateTimeConnected = DateTime.UtcNow,
                    //    DateTimeLast = DateTime.UtcNow,
                    //    IpAdress = aa.Address.ToString(),
                    //    IpPort = aa.Port.ToString()
                    //});

                    // string TmpRespondMessage = $"ConnectServer {Settings.VersionInformation.AppVersionString}\nHello {aa.Address.ToString()}\nAuthenticate\n";
                    newSocket.Send(Encoding.ASCII.GetBytes($"ConnectServer {Settings.VersionInformation.AppVersionString}\n"));

                    


                    //  Add client to settingd DATA
                    // Settings.Data.Clients.Add(ss, new Settings.Classes.Server.ClientConnectedClass() { Client = newSocket });
                    Task.Run(() => ReadDataLoop(newSocket));
                    // Task.Run(() => ReadDataLoop(newSocket, ss));
                    // Task.Run(() => ReadDataLoop(Settings.Data.Clients[ss]));

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
            string TmpRespondMessage = $"Hello {aa.Address.ToString()}\n";
            soc.Send(Encoding.ASCII.GetBytes(TmpRespondMessage));

            string Cmd = "";
            while (true)
            {
                soc.Send(Encoding.ASCII.GetBytes("Authenticate\n"));
                Cmd = ReadData(soc).ToLower().Trim();
                Console.WriteLine(Cmd);
            }
            Console.WriteLine("ReadDataLoop starting");
            Boolean testvalie = false;
            while (true)
            {
                if (!soc.Connected)
                    break;

                string xxx = "";
                xxx = ReadData(soc).ToLower().Trim();
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
                    //  Add client to sensor Id
                    // Settings.Data.Sensors["sim/time/total_flight_time_sec"].ClientUpdates.Add(guid);
                    // var data = "value:" + "sim/time/total_flight_time_sec" + ":" + Settings.Data.Sensors["sim/time/total_flight_time_sec"]?.Value + "\r\n";
                    // soc.Send(Encoding.ASCII.GetBytes(data));

                }


                // Console.WriteLine("{0}:{1}", client.Guid, xxx);
            }

            soc.Dispose();

            Console.WriteLine("ReadDataLoop Ending");

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
                numberOfBytesRead = client.Receive(myReadBuffer, 0);
                // numberOfBytesRead = client.Receive(myReadBuffer, client.ReceiveBufferSize, SocketFlags.None); // , 0); // , myReadBuffer.Length);

                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

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
