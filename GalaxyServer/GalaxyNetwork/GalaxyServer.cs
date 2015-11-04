using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyShared.Networking.Messages;
using GalaxyShared.Networking;
using GalaxyShared;

namespace GalaxyServer
{
    class GalaxyServer
    {
        static ConcurrentQueue<GalaxyMessage> MessageQueue;
        static BlockingCollection<GalaxyMessage> Messages;

        static ConcurrentQueue<GalaxyOutgoingMessage> OutgoingMessageQueue;
        static BlockingCollection<GalaxyOutgoingMessage> OutgoingMessages;


        static MessagePool MessagePool;
        static int MessageThreads = 2;  //threads that process message data
        static int SendThreads = 2; //threads that send message data
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

            for (int i =0; i < MessageThreads; i++)
            {
                Task.Factory.StartNew(() => ProcessMessages());
            }

            for (int i = 0; i < SendThreads; i++)
            {
                Task.Factory.StartNew(() => SendMessages());
            }

            Console.ReadLine();

        }

                     

        private async Task AcceptClientsAsync()
        {

            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Begin accept loop");
                TcpClient client = await listener.AcceptTcpClientAsync();
                client.NoDelay = true;
                Console.WriteLine("Client Connected");
                GalaxyClient gClient = new GalaxyClient(client);
                Task.Factory.StartNew(() => HandleClientRead(gClient));
            }
        }


        private async Task HandleClientRead(GalaxyClient client)
        {

            Console.WriteLine("Being Read Loop On Client");
            while (true)
            {

                GalaxyMessage message = MessagePool.GetMessage();
                
                int pos = 0;
                while (pos < GalaxyMessage.SIZE_BUFFER_SIZE)
                {                
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.SizeBuffer, pos, GalaxyMessage.SIZE_BUFFER_SIZE);                 
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    pos += bytesRead;
                }

                Console.WriteLine("Got Size");
                int size = BitConverter.ToInt16(message.SizeBuffer, 0);
                message.Size = size;

                pos = 0;
                while (pos < size)
                {
                    Console.WriteLine("Getting Message");
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.Buffer, pos, size);
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    pos += bytesRead;
                }
                message.Client = client;
                Console.WriteLine("Complete Message Received, Enqueing on thread:"+Thread.CurrentThread.ManagedThreadId);
                Messages.Add(message);
                                    

            }

        }

      

        public static void Send(GalaxyClient c,object o)
        {
            Console.WriteLine("Send");
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
                try {
                    binaryFormatter.Serialize(m.c.GalaxyTcpStream, m.o);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine("Message Sent");
            }
        }

        //spins and processes messages
        private void ProcessMessages()
        {
            
            IFormatter binaryFormatter = new BinaryFormatter();

            while (true)
            {
                GalaxyMessage message = Messages.Take();
                GalaxyClient client = (GalaxyClient)message.Client;
                MemoryStream m = new MemoryStream(message.Buffer, 0, message.Size);
                object result = binaryFormatter.Deserialize(m);
                
                int type = TypeDictionary.GetID(result);

                switch (type)
                {
                    case 0:
                        LogicLayer.HandleLoginMessage((LoginMessage)result,client);
                        break;
                    case 1:
                        LogicLayer.HandleNewUserMessage((NewUserMessage)result,client);
                        break;
                    case 5:
                        LogicLayer.HandleInput((InputMessage)result, client);
                        break;
                    default:
                        Console.WriteLine("unknown message");
                        break;
                }
                Console.WriteLine("message processed on thread:"+ Thread.CurrentThread.ManagedThreadId);
                
            }
        }

        
    }
}
