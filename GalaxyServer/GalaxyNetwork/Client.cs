using System.Net.Sockets;
using System.Collections.Generic;
using System;
using GalaxyShared;

namespace GalaxyServer
{ 
    public class Client
    {

        public byte ClientSendRate = 50; //millis
        public long LastSend = GalaxyServer.Millis; //millis
        public long LastPersist = GalaxyServer.Millis; //millis

        public Player Player;


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

            LogicLayer.RemoveClientFromAll(this);
            //persist player
            DataLayer.UpdateGalaxyPlayer(Player);
            if (GalaxyTcpClient != null) GalaxyTcpClient.Close();
            if (GalaxyUdpClient != null) GalaxyUdpClient.Close();
        }


    }

}