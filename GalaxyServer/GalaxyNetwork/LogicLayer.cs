using System;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GalaxyShared.Networking;

namespace GalaxyServer
{
    class LogicLayer
    {
        
        public static ConcurrentDictionary<GalaxyClient, GalaxyPlayer> PlayerTable = new ConcurrentDictionary<GalaxyClient, GalaxyPlayer>();


        public static void HandleLoginMessage(LoginMessage msg, GalaxyClient client)
        {
            Console.WriteLine("HandleLoginMessage");
                        
            GalaxyPlayerLogin login = DataLayer.GetLogin(msg.UserName);
            if (login != null && login.Password == msg.Password)
            {
                Console.WriteLine("User " + login.UserName + " logged in");
                InitiateLogin(msg.UserName,client);
            }
            else
            {
                LoginResultMessage m;
                m.success = false;
                GalaxyServer.AddToSendQueue(client, m);
            }
                                        
        }

        public static void InitiateLogin(string username, GalaxyClient client)
        {
            Console.WriteLine("InitiateLogin");
            GalaxyPlayer player = DataLayer.GetGalaxyPlayer(username);
            Console.WriteLine("Attempted to get GalaxyPlayer");
            //new player case
            if (player == null)
            {
                player = new GalaxyPlayer(username);
                DataLayer.UpdateGalaxyPlayer(player);
                Console.WriteLine("New player created and saved to db");
            }

            Console.WriteLine("About to send player data");
            while (!PlayerTable.TryAdd(client, player)) { }
            GalaxyServer.AddToSendQueue(client, player);
            Console.WriteLine("Player Data Sent");
                
        }


        public static void HandleNewUserMessage(NewUserMessage msg, GalaxyClient client)
        {            
            Console.WriteLine("HandleNewUserMessage");
            NewUserResultMessage m;
            GalaxyPlayerLogin login = new GalaxyPlayerLogin(msg.UserName, msg.Password);
            if (DataLayer.CreateNewLogin(msg.UserName,msg.Password))
            {
                InitiateLogin(msg.UserName,client);
            }
            else
            {
                m.success = false;
                GalaxyServer.AddToSendQueue(client, m);
            }
            
        }
       
        public static void HandleInputs(List<InputMessage> inputs, GalaxyClient client)
        {
            foreach (InputMessage input in inputs)
            {
                client.Inputs.Enqueue(input);
            }                                                                                             
        }

      

    }
}
