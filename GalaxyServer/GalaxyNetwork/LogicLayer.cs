using System;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using XnaGeometry;

namespace GalaxyServer
{
    public class LogicLayer : IMessageHandler
    {

        public static ConcurrentDictionary<Client, Player> PlayerTable = new ConcurrentDictionary<Client, Player>();
        
        
        public static readonly Vector3 SYSTEM_START_POS = new Vector3(0, 0, -5000);


        public void HandleMessage(LoginMessage msg, object o)
        {
            Client client = (Client)o;
            Console.WriteLine("HandleLoginMessage");

            LoginMessage login = DataLayer.GetLogin(msg.UserName);
            
            if (login.UserName != null && login.Password == msg.Password)
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

        public static void InitiateLogin(string username, Client client)
        {
            Console.WriteLine("InitiateLogin");
            Player player = DataLayer.GetGalaxyPlayer(username);
            Console.WriteLine("Attempted to get GalaxyPlayer");
            //new player case
            if (player == null)
            {
                player = new Player(username);
                DataLayer.UpdateGalaxyPlayer(player);
                Console.WriteLine("New player created and saved to db");
            }

            Console.WriteLine("About to send player data");
            while (!PlayerTable.TryAdd(client, player)) { }
          //  GalaxyServer.AddToSendQueue(client, player);
            Console.WriteLine("Player Data Sent");

        }


        public  void HandleMessage(NewUserMessage msg,object o)
        {
            Client client = (Client)o;
            Console.WriteLine("HandleNewUserMessage");
            NewUserResultMessage m;
            LoginMessage login;
            login.UserName = msg.UserName;
            login.Password = msg.Password;
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

        public static void HandleGotoWarpMessage(GoToWarpMessage msg, Client client)
        {

            Player player;
            while (!PlayerTable.TryGetValue(client, out player)) { }

            msg.Rotation = player.Rotation;

            Vector3 systemPos = player.Location.SystemPos;
            Vector3 startPos = new Vector3(systemPos.X * Sector.EXPAND_FACTOR, systemPos.Y * Sector.EXPAND_FACTOR, systemPos.Z * Sector.EXPAND_FACTOR);
            startPos += Vector3.Transform(Vector3.Forward * .3d, player.Rotation);
            player.Location.Pos = startPos;

            player.Location.InWarp = true;
            msg.Location = player.Location;
            
            GalaxyServer.AddToSendQueue(client, msg);

        }

        public static void HandleDropOutOfWarpMessage(DropOutOfWarpMessage msg, Client client)
        {
            Player player = GetPlayer(client);
            int x = Convert.ToInt32(player.Location.Pos.X / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
            int y = Convert.ToInt32(player.Location.Pos.Y / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
            int z = Convert.ToInt32(player.Location.Pos.Z / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);

            
            Sector sector = new Sector(new SectorCoord(x,y, z));
            sector.GenerateSystems(1);
            SolarSystem closeSystem = Simulator.GetClosestSystem(sector, player.Location.Pos);

           

            if (closeSystem != null && Vector3.Distance(player.Location.Pos,closeSystem.Pos* Sector.EXPAND_FACTOR) <= Simulator.WARP_DISTANCE_THRESHOLD)
            {
                SolarSystem system = DataLayer.GetSystem(closeSystem.Pos);
                if (system != null)
                {
                    closeSystem = system;
                }
                else
                {
                    closeSystem.Generate();
                    DataLayer.AddSystem(closeSystem);
                }

                player.Location.InWarp = false;
                player.Location.Pos = SYSTEM_START_POS;
                player.Location.SystemPos = closeSystem.Pos;
                player.Location.SectorCoord = sector.Coord;
                player.SolarSystem = closeSystem;
                msg.Location = player.Location;
                msg.Rotation = player.Rotation;
                msg.System = closeSystem;
                GalaxyServer.AddToSendQueue(client,msg);
                return;
            }

            //todo else send some sort of nope msg?

        }

        public static Player GetPlayer(Client client)
        {
            Player player;
            while (!PlayerTable.TryGetValue(client, out player)) { }
            return player;
        }

        public static void HandleInput(InputMessage input, Client client)
        {
            lock (client.Inputs)
            {               
                client.Inputs.Enqueue(input);               
            }
        }


        public class AsteroidComparer : IComparer<Asteroid>
        {
            Vector3 Pos;
            public AsteroidComparer(Vector3 pos)
            {
                Pos = pos;
            }
            public int Compare(Asteroid x, Asteroid y)
            {
                double distanceX = Vector3.Distance(x.Pos, Pos);
                double distanceY = Vector3.Distance(y.Pos, Pos);
                if (distanceX > distanceY) return 1;
                return -1;
            }
        }

        public static void AsteroidMining(Client client, Player player)
        {
            List<Asteroid> asteroids = player.SolarSystem.Asteroids;
            Asteroid hit = null;
            Vector3 pos = player.Location.Pos;
            asteroids.Sort(new AsteroidComparer(pos));
            int count = 0;
            foreach (Asteroid a in asteroids)
            {
                Ray ray = new Ray(pos, Vector3.Transform(Vector3.Forward, player.Rotation));
                BoundingSphere sphere = new BoundingSphere(a.Pos, a.Size*Planet.EARTH_CONSTANT);
                double? result = ray.Intersects(sphere);
                if (result != null)
                {
                    hit = a;
                    break;
                }
                count++;
            }
            Console.WriteLine("Count:" + count);
            if (hit != null)
            {
                hit.Remaining -= player.Ship.MiningLaserPower;
                player.Ship.AddCargo(new IronOre(player.Ship.MiningLaserPower));
                Console.WriteLine("hit!:" + hit.Remaining);
            //    GalaxyServer.AddToSendQueue(client, hit);
                CargoStateMessage cargoState = new CargoStateMessage();
                cargoState.add = true;
                cargoState.item = new IronOre(player.Ship.MiningLaserPower);
                GalaxyServer.AddToSendQueue(client, cargoState);
                if (hit.Remaining <= 0) asteroids.Remove(hit);
                DataLayer.AddSystem(player.SolarSystem);
            }
        }


        public static Stopwatch sw = new Stopwatch();

        public static void DoPhysics()
        {
            int persistCounter = 0;
            while (true)
            {
                sw.Restart();
                foreach (Client client in LogicLayer.PlayerTable.Keys)
                {
                    Player player;
                    while (!PlayerTable.TryGetValue(client, out player)) { }
                    if (player.Location.InWarp)
                    {
                        ProcessTickForPlayerWarp(client, player);
                    }
                    else
                    {
                        ProcessTickForPlayer(client, player);
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

        public static void ProcessTickForPlayer(Client client, Player player)
        {
            lock (client.Inputs)
            {
                if (client.Inputs.Count > 0)
                {
                    InputMessage input = client.Inputs.Dequeue();
                    Simulator.ProcessInput(player, input);
                    if (input.SecondaryButton) AsteroidMining(client, player);
                }
            }
            Simulator.ContinuedPhysics(player);
        }

        public static void ProcessTickForPlayerWarp(Client client, Player player)
        {
            lock (client.Inputs)
            {
                if (client.Inputs.Count > 0)
                {
                    InputMessage input = client.Inputs.Dequeue();
                    Simulator.ProcessInputWarp(player, input);
                }
            }
            Simulator.ContinuedPhysicsWarp(player);
        }
    }
}
