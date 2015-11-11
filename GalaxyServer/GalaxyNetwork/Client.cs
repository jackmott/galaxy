using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GalaxyShared
{
    
    

    public class Client
    {

        public int ClientSendRate = 50; //ms
        public DateTime LastSend = new DateTime(1970, 1, 1); //long long ago
        

        public TcpClient GalaxyTcpClient { get; set; }
        public UdpClient GalaxyUdpClient { get; set; }
        public NetworkStream GalaxyTcpStream { get; set; }
        public NetworkStream GalaxyUdpStream { get; set; }

        
        public Queue<InputMessage> Inputs;
        


        public Client(TcpClient client)
        {
           GalaxyTcpClient = client;
           GalaxyTcpStream = client.GetStream();

            Inputs = new Queue<InputMessage>();
                                    
        }            

        public void Cleanup()

        {
            if (GalaxyTcpClient != null) GalaxyTcpClient.Close();
            if (GalaxyUdpClient != null) GalaxyUdpClient.Close();                        
        }

        
    }
}
