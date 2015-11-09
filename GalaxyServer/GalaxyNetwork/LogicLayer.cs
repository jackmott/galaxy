using System;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using XnaGeometry;

namespace GalaxyServer
{
    class LogicLayer
    {

        public static ConcurrentDictionary<GalaxyClient, GalaxyPlayer> PlayerTable = new ConcurrentDictionary<GalaxyClient, GalaxyPlayer>();
        
        
        public static readonly Vector3 SYSTEM_START_POS = new Vector3(0, 0, -5000);


        public static void HandleLoginMessage(LoginMessage msg, GalaxyClient client)
        {
            Console.WriteLine("HandleLoginMessage");

            GalaxyPlayerLogin login = DataLayer.GetLogin(msg.UserName);
            if (login != null && login.Password == msg.Password)
            {
                Console.WriteLine("User " + login.UserName + " logged in");
                InitiateLogin(msg.UserName, client);
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
            if (DataLayer.CreateNewLogin(msg.UserName, msg.Password))
            {
                InitiateLogin(msg.UserName, client);
            }
            else
            {
                m.success = false;
                GalaxyServer.AddToSendQueue(client, m);
            }

        }

        public static void HandleGotoWarpMessage(GoToWarpMessage msg, GalaxyClient client)
        {

            GalaxyPlayer player;
            while (!PlayerTable.TryGetValue(client, out player)) { }

            msg.Rotation = player.Rotation;

            SolarSystem system = new SolarSystem(player.Location.SystemPos);
            Vector3 startPos = new Vector3(system.Pos.X * GalaxySector.EXPAND_FACTOR, system.Pos.Y * GalaxySector.EXPAND_FACTOR, system.Pos.Z * GalaxySector.EXPAND_FACTOR);
            startPos += Vector3.Transform(Vector3.Forward * .3d, player.Rotation);
            player.Location.Pos = startPos;

            player.Location.InWarp = true;
            msg.Location = player.Location;
            
            GalaxyServer.AddToSendQueue(client, msg);

        }

        public static void HandleDropOutOfWarpMessage(DropOutOfWarpMessage msg, GalaxyClient client)
        {
            GalaxyPlayer player = GetPlayer(client);
            int x = Convert.ToInt32(player.Location.Pos.X / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);
            int y = Convert.ToInt32(player.Location.Pos.Y / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);
            int z = Convert.ToInt32(player.Location.Pos.Z / GalaxySector.EXPAND_FACTOR / GalaxySector.SECTOR_SIZE);

            
            GalaxySector sector = new GalaxySector(new SectorCoord(x,y, z));
            sector.GenerateSystems(1);
            SolarSystem closeSystem = Simulator.GetClosestSystem(sector, player.Location.Pos);
           

            if (closeSystem != null && Vector3.Distance(player.Location.Pos,closeSystem.Pos* GalaxySector.EXPAND_FACTOR) <= Simulator.WARP_DISTANCE_THRESHOLD)
            {
                player.Location.InWarp = false;
                player.Location.Pos = SYSTEM_START_POS;
                player.Location.SystemPos = closeSystem.Pos;
                player.Location.SectorCoord = sector.Coord;                
                msg.Location = player.Location;
                msg.Rotation = player.Rotation;
                GalaxyServer.AddToSendQueue(client,msg);
                return;
            }

            //todo else send some sort of nope msg?

        }

        public static GalaxyPlayer GetPlayer(GalaxyClient client)
        {
            GalaxyPlayer player;
            while (!PlayerTable.TryGetValue(client, out player)) { }
            return player;
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
            int persistCounter = 0;
            while (true)
            {
                sw.Restart();
                foreach (GalaxyClient client in LogicLayer.PlayerTable.Keys)
                {
                    GalaxyPlayer player;
                    while (!PlayerTable.TryGetValue(client, out player)) { }
                    if (player.Location.InWarp)
                    {
                        Simulator.ProcessTickForPlayerWarp(client.Inputs, player);
                    }
                    else
                    {
                        Simulator.ProcessTickForPlayer(client.Inputs, player);
                    }

                    long deltaT = DateTime.Now.Subtract(client.LastSend).Milliseconds;
                    if (deltaT >= client.ClientSendRate)
                    {

                        PlayerStateMessage pState = new PlayerStateMessage();
                        pState.PlayerPos = player.Location.Pos;
                        pState.Rotation = player.Rotation;
                        pState.Throttle = player.Throttle;
                        pState.Seq = player.Seq;
                        GalaxyServer.AddToSendQueue(client, pState);
                        client.LastSend = DateTime.Now;
                    }

                    //Persist the player to the Database
                    if (persistCounter * NetworkUtils.SERVER_TICK_RATE > DataLayer.PLAYER_STATE_PERSIST_RATE)
                    {
                        DataLayer.UpdateGalaxyPlayer(player);
                        persistCounter = 0;
                    }

                }

                persistCounter++;
                sw.Stop();
                Thread.Sleep(Convert.ToInt32(MathHelper.Clamp(NetworkUtils.SERVER_TICK_RATE - sw.ElapsedMilliseconds, 0, 100)));
            }

        }



    }
}
