using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using GalaxyShared;
using ProtoBuf;


namespace GalaxyServer
{

    class GalaxyServer
    {
        static ConcurrentQueue<MessageWrapper> MessageQueue;
        static BlockingCollection<MessageWrapper> Messages;

        static ConcurrentQueue<MessageWrapper> OutgoingMessageQueue;
        static BlockingCollection<MessageWrapper> OutgoingMessages;


        
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

            MessageQueue = new ConcurrentQueue<MessageWrapper>();
            Messages = new BlockingCollection<MessageWrapper>(MessageQueue);

            OutgoingMessageQueue = new ConcurrentQueue<MessageWrapper>();
            OutgoingMessages = new BlockingCollection<MessageWrapper>(OutgoingMessageQueue);

            
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
                    Client gClient = new Client(client);
                    Task.Factory.StartNew(() => HandleClientRead(gClient));
                }
                catch (Exception)
                {
                    Console.WriteLine("AcceptClientsAsycn Exception");
                }
            }
        }


        private async Task HandleClientRead(Client client)
        {
            byte[] buffer = new byte[NetworkUtils.SERVER_READ_BUFFER_SIZE];
            Console.WriteLine("Being Read Loop On Client");
            while (true)
            {

                try
                {

                    int size = 0;
                    int bytesRead = 0;
                    do
                    {
                        bytesRead += await client.GalaxyTcpStream.ReadAsync(buffer, bytesRead, NetworkUtils.SERVER_READ_BUFFER_SIZE);
                    } while (!Serializer.TryReadLengthPrefix(buffer, 0, bytesRead, PrefixStyle.Base128, out size));


                    while (bytesRead < size)
                    {
                        bytesRead += await client.GalaxyTcpStream.ReadAsync(buffer, bytesRead, size - bytesRead);
                    }

                    MemoryStream m = new MemoryStream(buffer, 0, bytesRead);
                    object message;
                    if (!Serializer.NonGeneric.TryDeserializeWithLengthPrefix(m, PrefixStyle.Base128, TypeDictionary.TypeResolver, out message))
                    {
                        Console.WriteLine("Failed to deserialize");
                    }
                    
                    MessageWrapper wrapper;
                    wrapper.c = client;
                    wrapper.o = message;
                    Messages.Add(wrapper);
                }
                catch (Exception)
                {
                    Console.WriteLine("handle read exception");
                    CleanUpClient(client);
                    return;
                }


            }

        }

        public static void CleanUpClient(Client client)
        {
            client.Cleanup();
            Player p;
            while (!LogicLayer.PlayerTable.TryRemove(client, out p))
            {
                if (!LogicLayer.PlayerTable.ContainsKey(client))
                {
                    return;
                }
            }
        }



        public static void AddToSendQueue(Client c, object o)
        {

            MessageWrapper m;
            m.c = c;
            m.o = o;
            OutgoingMessages.Add(m);
        }

        private void SendMessages()
        {
            //IFormatter binaryFormatter = new BinaryFormatter();
            while (true)
            {
                MessageWrapper m = OutgoingMessages.Take();

                try
                {
                    //binaryFormatter.Serialize(m.c.GalaxyTcpStream, m.o);                    
                    Serializer.SerializeWithLengthPrefix(m.c.GalaxyTcpStream, m.o, PrefixStyle.Base128, (int)TypeDictionary.GetID(m.o));
                }
                catch (Exception)
                {
                    Console.WriteLine("Send Loop Exception");
                    CleanUpClient(m.c);
                }

            }
        }

        //spins and processes messages
        private void ProcessMessages()
        {

            // IFormatter binaryFormatter = new BinaryFormatter();

            while (true)
            {
                MessageWrapper message = Messages.Take();
                
                Client client = null;
                try
                {
                    client = message.c;
                    
                                                            
                    switch (TypeDictionary.GetID(message.o))
                    {
                        case MsgType.LoginMessage:
                            LogicLayer.HandleLoginMessage((LoginMessage)message.o, client);
                            break;
                        case MsgType.NewUserMessage:
                            LogicLayer.HandleNewUserMessage((NewUserMessage)message.o, client);
                            break;
                        case MsgType.ListOfInputMessage:
                            LogicLayer.HandleInputs((List<InputMessage>)message.o, client);
                            break;
                        case MsgType.GoToWarpMessage:
                            LogicLayer.HandleGotoWarpMessage((GoToWarpMessage)message.o, client);
                            break;
                        case MsgType.DropOutOfWarpMessage:
                            LogicLayer.HandleDropOutOfWarpMessage((DropOutOfWarpMessage)message.o, client);
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
