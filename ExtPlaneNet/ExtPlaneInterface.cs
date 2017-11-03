using System;
using System.Globalization;
using System.Collections.Concurrent;
using ExtPlaneNetCore.Commands;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ExtPlaneNetCore.InputProcessors;

// https://github.com/jrunestone/ExtPlaneNet

namespace ExtPlaneNetCore
{
    public class ExtPlaneInterface
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public float UpdateInterval { get; set; }

        protected readonly IDataRefRepository DataRefs = new DataRefRepository();
        protected readonly ICommandQueue Commands = new CommandQueue();
        protected readonly InputHandler InputHandler = new InputHandler();

        protected TcpClient Socket { get; set; }

        protected Task SenderTask { get; set; }
        protected Task ReceiverTask { get; set; }

        protected CancellationTokenSource CancellationSource { get; set; }

        public ExtPlaneInterface(string host = "127.0.0.1", int port = 51000, float updateInterval = 0.33f)
        {
            Host = host;
            Port = port;
            UpdateInterval = updateInterval;

            InputHandler.AddInputProcessor(new VersionProcessor());
            InputHandler.AddInputProcessor(new DataRefProcessor(DataRefs));
        }
        public bool IsConnected()
        {
            if (Socket != null)
            {
                // var sdf = Socket.Connected;

                // var dsfds = "sdfdsf";
                return Socket.Connected;

            }
            if (Socket != null)
                return true;

            return false;
        }
        public void Connect()
        {
            if (Socket != null)
            {
                if (Socket.Connected)
                {
                    //  Already connected.
                    return;

                }
            }
                // throw new InvalidOperationException("Can't connect: already connected");

            try
            {
                // AddressFamily dd = new AddressFamily();
                
                  

                // Socket = new TcpClient(Host, Port);
                Socket = new TcpClient();
                //  Socket.ConnectAsync(Host, Port);
                Socket.Client.Connect(Host, Port);

                // IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 90);
                // sSocket.ConnectAsync(ipEnd);

            }
            catch (SocketException)
            {
                Socket = null;
                return;
                throw;
            }

            if (Socket.Connected)
            {
                Console.WriteLine("connected");
            }
            else
            {
                Console.WriteLine("not connected!!!");
                // Socket.ConnectAsync(Host, Port);

            }
            CancellationSource = new CancellationTokenSource();

            SenderTask = Task.Factory.StartNew(() => new Sender(Socket.GetStream(), Commands, CancellationSource.Token).Run(), CancellationSource.Token);
            ReceiverTask = Task.Factory.StartNew(() => new Receiver(Socket.GetStream(), InputHandler, CancellationSource.Token).Run(), CancellationSource.Token);
        }

        public void Disconnect()
        {
            CancellationSource.Cancel();

            if (SenderTask != null)
            {
                //  Remove this line and se if that fix the problem.
                // SenderTask.Wait();
                SenderTask = null;
            }

            if (ReceiverTask != null)
            {
                ReceiverTask.Wait();
                ReceiverTask = null;
            }

            if (Socket != null)
            {
                
                // Socket.Close();
                Socket = null;
            }
        }

        public void Subscribe<T>(string dataRef, float accuracy = 0.0f, Action<DataRef<T>> changed = null)
        {
            if (string.IsNullOrWhiteSpace(dataRef))
                throw new ArgumentNullException("dataRef");

            if (DataRefs.Contains(dataRef))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Can't subscribe to dataref: dataref {0} already subscribed to", dataRef));

            var refObj = new DataRef<T>(dataRef, accuracy);

            if (changed != null)
                refObj.Changed += (o, e) => changed(e);

            DataRefs.Add<T>(refObj);
            Commands.Enqueue(new SubscribeCommand(dataRef, accuracy));
        }

        public void Unsubscribe(string dataRef)
        {
            if (string.IsNullOrWhiteSpace(dataRef))
                throw new ArgumentNullException("dataRef");

            DataRefs.Remove(dataRef);
            Commands.Enqueue(new UnsubscribeCommand(dataRef));
        }

        public DataRef<T> GetDataRef<T>(string dataRef)
        {
            if (string.IsNullOrWhiteSpace(dataRef))
                throw new ArgumentNullException("dataRef");

            if (!DataRefs.Contains(dataRef))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Can't get dataref value: dataref {0} isn't subscribed to", dataRef));

            return DataRefs.Get<T>(dataRef);
        }

        public void SetDataRef<T>(string dataRef, T value)
        {
            if (string.IsNullOrWhiteSpace(dataRef))
                throw new ArgumentNullException("dataRef");

            if (!DataRefs.Contains(dataRef))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Can't set dataref value: dataref {0} isn't subscribed to", dataRef));

            Commands.Enqueue(new SetDataRefCommand<T>(dataRef, value));
        }
    }
}