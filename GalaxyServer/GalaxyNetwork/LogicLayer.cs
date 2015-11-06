using System;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GalaxyShared.Networking;
using System.Diagnostics;
using System.Threading;
using XnaGeometry;

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
            lock (inputs)
            {
                foreach (InputMessage input in inputs)
                {
                    client.Inputs.Enqueue(input);
                }
            }
        }


        public static Stopwatch sw = new Stopwatch();

        public static void DoPhysics()
        {
            while (true)
            {
                sw.Restart();
                foreach (GalaxyClient client in LogicLayer.PlayerTable.Keys)
                {
                    GalaxyPlayer player;
                    while (!PlayerTable.TryGetValue(client, out player)) { }
                    Simulator.ProcessTickForPlayer(client.Inputs, player);

                    long deltaT = DateTime.Now.Subtract(client.LastSend).Milliseconds;
                    if (deltaT >= client.ClientSendRate)
                    {
                        PlayerStateMessage pState = new PlayerStateMessage();
                        pState.PlayerPos = player.PlayerPos;
                        pState.Rotation = player.Rotation;
                        pState.Throttle = player.Throttle;
                        pState.Seq = player.Seq;
                        GalaxyServer.AddToSendQueue(client, pState);
                        client.LastSend = DateTime.Now;
                    }

                }
                sw.Stop();
                Thread.Sleep(Convert.ToInt32(MathHelper.Clamp(NetworkUtils.SERVER_TICK_RATE - sw.ElapsedMilliseconds, 0, 100)));


            }

        }



    }
}
