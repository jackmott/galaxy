using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyShared;
using XnaGeometry;
namespace GalaxyServer
{
    class GalaxyServer
    {
        static ConcurrentQueue<GalaxyMessage> MessageQueue;
        static BlockingCollection<GalaxyMessage> Messages;

        static ConcurrentQueue<GalaxyOutgoingMessage> OutgoingMessageQueue;
        static BlockingCollection<GalaxyOutgoingMessage> OutgoingMessages;


        static MessagePool MessagePool;
        static int MessageThreads = 1;  //threads that process message data
        static int SendThreads = 1; //threads that send message data
        static int PhysicsThreads = 1;
        static TypeDictionary TypeDictionary;

        static DataLayer D;

        public static void Main(string[] args)
        {
            
                
            GalaxyServer server = new GalaxyServer();

        }

        public GalaxyServer()
        {

            MessageQueue = new ConcurrentQueue<GalaxyMessage>();
            Messages = new BlockingCollection<GalaxyMessage>(MessageQueue);

            OutgoingMessageQueue = new ConcurrentQueue<GalaxyOutgoingMessage>();
            OutgoingMessages = new BlockingCollection<GalaxyOutgoingMessage>(OutgoingMessageQueue);


            MessagePool = new MessagePool();
            TypeDictionary = new TypeDictionary();

            D = new DataLayer();

            Task.Factory.StartNew(() => AcceptClientsAsync());

            for (int i = 0; i < MessageThreads; i++)
            {
                Task.Factory.StartNew(() => ProcessMessages());
            }

            for (int i = 0; i < SendThreads; i++)
            {
                Task.Factory.StartNew(() => SendMessages());
            }

            for (int i = 0; i < PhysicsThreads; i++)
            {
                Task.Factory.StartNew(() => LogicLayer.DoPhysics());
            }

            Console.ReadLine();

        }



        private async Task AcceptClientsAsync()
        {

            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                try
                {
                    Console.WriteLine("Begin accept loop");
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    client.NoDelay = true;
                    Console.WriteLine("Client Connected");
                    GalaxyClient gClient = new GalaxyClient(client);
                    Task.Factory.StartNew(() => HandleClientRead(gClient));
                }
                catch (Exception)
                {
                    Console.WriteLine("AcceptClientsAsycn Exception");
                }
            }
        }


        private async Task HandleClientRead(GalaxyClient client)
        {

            Console.WriteLine("Being Read Loop On Client");
            while (true)
            {

                try
                {


                    GalaxyMessage message = MessagePool.GetMessage();

                    int pos = 0;
                    while (pos < GalaxyMessage.SIZE_BUFFER_SIZE)
                    {
                        int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.SizeBuffer, pos, GalaxyMessage.SIZE_BUFFER_SIZE);
                        if (bytesRead == 0) { client.Cleanup(); return; }
                        pos += bytesRead;
                    }


                    int size = BitConverter.ToInt16(message.SizeBuffer, 0);
                    message.Size = size;

                    pos = 0;
                    while (pos < size)
                    {

                        int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.Buffer, pos, size);
                        if (bytesRead == 0) { client.Cleanup(); return; }
                        pos += bytesRead;
                    }
                    message.Client = client;
                    Messages.Add(message);
                }
                catch (Exception)
                {
                    Console.WriteLine("handle read exception");
                    CleanUpClient(client);
                    return;
                }


            }

        }

        public static void CleanUpClient(GalaxyClient client)
        {
            client.Cleanup();
            GalaxyPlayer p;            
            while (!LogicLayer.PlayerTable.TryRemove(client, out p)){
                if (!LogicLayer.PlayerTable.ContainsKey(client))
                {
                    return;
                }
            }
        }



        public static void AddToSendQueue(GalaxyClient c, object o)
        {

            GalaxyOutgoingMessage m;
            m.c = c;
            m.o = o;
            OutgoingMessages.Add(m);
        }

        private void SendMessages()
        {
            IFormatter binaryFormatter = new BinaryFormatter();
            while (true)
            {
                GalaxyOutgoingMessage m = OutgoingMessages.Take();

                try
                {
                    binaryFormatter.Serialize(m.c.GalaxyTcpStream, m.o);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Send Loop Exception");
                    CleanUpClient(m.c);
                }
                Console.WriteLine("Still Truckin");
            }
        }

        //spins and processes messages
        private void ProcessMessages()
        {

            IFormatter binaryFormatter = new BinaryFormatter();

            while (true)
            {
                GalaxyMessage message = Messages.Take();
                GalaxyClient client = null;
                try
                {
                    client = (GalaxyClient)message.Client;
                    MemoryStream m = new MemoryStream(message.Buffer, 0, message.Size);
                    object result = binaryFormatter.Deserialize(m);
                    TypeDictionary.MsgType type = TypeDictionary.GetID(result);

                    switch (type)
                    {
                        case TypeDictionary.MsgType.LoginMessage:
                            LogicLayer.HandleLoginMessage((LoginMessage)result, client);
                            break;
                        case TypeDictionary.MsgType.NewUserMessage:
                            LogicLayer.HandleNewUserMessage((NewUserMessage)result, client);
                            break;
                        case TypeDictionary.MsgType.ListOfInputMessage:
                            LogicLayer.HandleInputs((List<InputMessage>)result, client);
                            break;
                        default:
                            Console.WriteLine("unknown message");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception on process messages");
                    if (client != null) CleanUpClient(client);
                }


            }
        }


    }
}
