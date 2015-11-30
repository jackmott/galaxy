using System;
using System.Collections.Concurrent;
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

            Sector s = new Sector(new SectorCoord(0, 0, 1));
            s.GenerateSystems(1);
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
            byte[] typeBuffer = new byte[1];
            Console.WriteLine("Being Read Loop On Client");
            while (true)
            {

                try
                {
                    
                    await client.GalaxyTcpStream.ReadAsync(typeBuffer, 0, 1);
                    
                    int bytesRead = 0;

                    while (bytesRead < 4)
                    {
                        bytesRead += await client.GalaxyTcpStream.ReadAsync(buffer, bytesRead, 4 - bytesRead);
                    } 

                    int size = BitConverter.ToInt32(buffer, 0);

                    bytesRead = 0;
                    while (bytesRead < size) 
                    {
                        bytesRead += await client.GalaxyTcpStream.ReadAsync(buffer, bytesRead+4, size - bytesRead);
                    } 

                    MemoryStream m = new MemoryStream(buffer, 0, size+4);


                    MessageWrapper wrapper;
                    wrapper.Client = client;
                    
                    switch ((MsgType)typeBuffer[0])
                    {
                        case MsgType.LoginMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<LoginMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.NewUserMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<NewUserMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.Player:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<Player>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.InputMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<InputMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.PlayerStateMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<PlayerStateMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.GoToWarpMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<GoToWarpMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.DropOutOfWarpMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<DropOutOfWarpMessage>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.Asteroid:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<Asteroid>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.Ship:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<Ship>(m, PrefixStyle.Fixed32);
                            break;
                        case MsgType.MiningMessage:
                            wrapper.Payload = Serializer.DeserializeWithLengthPrefix<MiningMessage>(m, PrefixStyle.Fixed32);
                            break;
                        default:
                            wrapper.Payload = null;
                            break;
                    }
                    
                    Messages.Add(wrapper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("handle read exception"+ex);
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



        public static void AddToSendQueue(Client client, IMessage payload)
        {
            MessageWrapper m;
            m.Client = client;
            m.Payload = payload;            
            OutgoingMessages.Add(m);
        }

        private void SendMessages()
        {                                    
            byte[] typeBuffer = new byte[1]; 
            while (true)
            {
                MessageWrapper m = OutgoingMessages.Take();               
                try
                {
                    m.Payload.Proto(m.Client.GalaxyTcpStream, typeBuffer);                    
                }
                catch (Exception)
                {
                    Console.WriteLine("Send Loop Exception");
                    CleanUpClient(m.Client);
                }

            }
        }

        //spins and processes messages
        private void ProcessMessages()
        {         
            while (true)
            {
                MessageWrapper message = Messages.Take();
                message.Payload.AcceptHandler(new LogicLayer(),message.Client);               

            }
        }


    }
}

