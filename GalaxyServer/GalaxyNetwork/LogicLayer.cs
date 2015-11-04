using System;
using GalaxyShared.Networking.Messages;
using GalaxyShared;

namespace GalaxyServer
{
    class LogicLayer
    {


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
                GalaxyServer.Send(client, m);
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
            GalaxyServer.Send(client, player);
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
                GalaxyServer.Send(client, m);
            }
            
        }

    }
}
