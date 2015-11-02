using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using GalaxyShared;

namespace GalaxyServer
{
    class GalaxyServer
    {
        ConcurrentQueue<GalaxyMessage> MessageQueue;
        BlockingCollection<GalaxyMessage> Messages;
        MessagePool MessagePool;
        int MessageThreads = 4;


        public static void Main(string[] args)
        {
            GalaxyGen gen = new GalaxyGen();

        }

        public GalaxyServer()
        {
            
            MessageQueue = new ConcurrentQueue<GalaxyMessage>();
            Messages = new BlockingCollection<GalaxyMessage>(MessageQueue);
            MessagePool = new MessagePool();
            Task.Factory.StartNew(() => AcceptClientsAsync());
            for (int i =0; i < MessageThreads; i++)
            {
                Task.Factory.StartNew(() => ProcessMessages());
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
                Console.WriteLine("Client Connected");
                GalaxyClient gClient = new GalaxyClient(client);
                Task.Factory.StartNew(() => HandleClientRead(gClient));
            }
        }


        private async Task HandleClientRead(GalaxyClient client)
        {


            while (true)
            {

                GalaxyMessage message = MessagePool.GetMessage();
                message.Client = client;

                while (message.BufferPos < 2)
                {
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.Buffer, message.BufferPos, 2);
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    message.BufferPos += bytesRead;
                }

                
                message.Size = BitConverter.ToInt16(message.Buffer, 0);
                
                

                while (message.BufferPos < message.Size)
                {                    
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.Buffer, message.BufferPos, message.Size - 2);
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    message.BufferPos += bytesRead;
                }
                Console.WriteLine("Complete Message Received, Enqueing on thread:"+Thread.CurrentThread.ManagedThreadId);
                Messages.Add(message);
                                    

            }

        }


        private void ProcessMessages()
        {
            while (true)
            {
                GalaxyMessage message = Messages.Take();                
                Console.WriteLine("message processed on thread:"+ Thread.CurrentThread.ManagedThreadId);
                
            }
        }

    }
}
