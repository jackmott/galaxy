using System.Net.Sockets;
using GalaxyShared.Networking.Messages;
using System.Collections.Concurrent;
using System;

namespace GalaxyShared
{
    
    

    public class GalaxyClient
    {

        public int ClientSendRate = 50; //ms
        public DateTime LastSend = new DateTime(1970, 1, 1); //long long ago
        

        public TcpClient GalaxyTcpClient { get; set; }
        public UdpClient GalaxyUdpClient { get; set; }
        public NetworkStream GalaxyTcpStream { get; set; }
        public NetworkStream GalaxyUdpStream { get; set; }

        
        public ConcurrentQueue<InputMessage> Inputs;


        public GalaxyClient(TcpClient client)
        {
            this.GalaxyTcpClient = client;
            this.GalaxyTcpStream = client.GetStream();
            Inputs = new ConcurrentQueue<InputMessage>();
                                    
        }            

        public void Cleanup()

        {
            if (GalaxyTcpClient != null) GalaxyTcpClient.Close();
            if (GalaxyUdpClient != null) GalaxyUdpClient.Close();                        
        }

        
    }
}
