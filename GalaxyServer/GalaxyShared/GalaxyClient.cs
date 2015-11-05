using System.Net.Sockets;

namespace GalaxyShared
{
    
    

    public class GalaxyClient
    {

        public const float TICK_RATE = .03f; //s

        public TcpClient GalaxyTcpClient { get; set; }
        public UdpClient GalaxyUdpClient { get; set; }
        public NetworkStream GalaxyTcpStream { get; set; }
        public NetworkStream GalaxyUdpStream { get; set; }
        

        
        
        public GalaxyClient(TcpClient client)
        {
            this.GalaxyTcpClient = client;
            this.GalaxyTcpStream = client.GetStream();
                                    
        }            

        public void Cleanup()

        {
            if (GalaxyTcpClient != null) GalaxyTcpClient.Close();
            if (GalaxyUdpClient != null) GalaxyUdpClient.Close();            
        }

        
    }
}
