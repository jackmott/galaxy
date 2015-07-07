using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace GalaxyServer
{
    class GalaxyServer
    {
        ConcurrentQueue<GalaxyMessage> q;
        MessagePool messagePool;


        public static void Main(string[] args)
        {
            new GalaxyServer();
        }

        public GalaxyServer()
        {
            Task.Factory.StartNew(() => AcceptClientsAsync());
            q = new ConcurrentQueue<GalaxyMessage>();
            messagePool = new MessagePool();
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
                Console.WriteLine("got one");
                GalaxyClient gClient = new GalaxyClient(client);
                Task.Factory.StartNew(() => HandleClientRead(gClient));
            }
        }


        private async Task HandleClientRead(GalaxyClient client)
        {


            while (true)
            {

                GalaxyMessage message = messagePool.GetMessage();
                message.Client = client;

                while (message.BufferPos < 2)
                {
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.buffer, message.BufferPos, 2);
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    message.BufferPos += bytesRead;
                }

                
                message.Size = BitConverter.ToInt16(message.buffer, 0);
                

                while (message.BufferPos < message.Size)
                {
                    int bytesRead = await client.GalaxyTcpStream.ReadAsync(message.buffer, message.BufferPos, GalaxyMessage.BUFFER_SIZE);
                    if (bytesRead == 0) { client.Cleanup(); return; }
                    message.BufferPos += bytesRead;
                }

                q.Enqueue(message);
                                    

            }

        }

    }
}
