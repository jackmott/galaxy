using System;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using XnaGeometry;
using System.Reflection;

namespace GalaxyServer
{
    public class LogicLayer : IMessageHandler
    {

        public static ConcurrentDictionary<Client, Player> PlayerTable = new ConcurrentDictionary<Client, Player>();
        
        
        public static readonly Vector3 SYSTEM_START_POS = new Vector3(0, 0, -5000);


        public void HandleMessage(LoginMessage msg, object extra)
        {
            Client client = (Client)extra;
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

            //todo - this commented out for testing 
            //Player player = DataLayer.GetGalaxyPlayer(username);
            Player player = null;

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
            GalaxyServer.AddToSendQueue(client, player);
            Console.WriteLine("Player Data Sent");

        }


        public  void HandleMessage(NewUserMessage msg,object extra)
        {
            Client client = (Client)extra;
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

       

        public void HandleMessage(GoToWarpMessage msg, object extra)
        {
            Client client = (Client)extra;
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

        public void HandleMessage(DropOutOfWarpMessage msg, object extra)
        {
            
            Client client = (Client)extra;
            Player player = GetPlayer(client);
            if (!player.Location.InWarp)
            {
                Console.WriteLine("Drop out of warp msg received when not in warp");
                return;
            }
            int x = Convert.ToInt32(player.Location.Pos.X / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
            int y = Convert.ToInt32(player.Location.Pos.Y / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);
            int z = Convert.ToInt32(player.Location.Pos.Z / Sector.EXPAND_FACTOR / Sector.SECTOR_SIZE);

            
            Sector sector = new Sector(new SectorCoord(x,y, z));
            SolarSystem system = sector.GenerateSystem(msg.SystemIndex);

            double distance = Vector3.Distance(player.Location.Pos, system.Pos * Sector.EXPAND_FACTOR);
        
            //give some wiggle room since server/client will not be perfectly in sync
            if (system != null && distance <= Simulator.WARP_DISTANCE_THRESHOLD*2)
            {
                //fill in the system with any deltas if it exists in DB
                SolarSystem updatedSystem = DataLayer.GetSystem(system);
                if (updatedSystem != null)
                {
                    system = updatedSystem;
                }
                else
                {
                    system.Generate();
                    DataLayer.AddSystem(system);
                }

                
                player.Location.Pos = SYSTEM_START_POS;
                player.Location.SystemPos = system.Pos;
                player.Location.SectorCoord = sector.Coord;
                player.SolarSystem = system;
                player.Location.InWarp = false;
                msg.Location = player.Location;
                msg.Rotation = player.Rotation;
                msg.System = system;
                GalaxyServer.AddToSendQueue(client,msg);
                return;
            }

            //todo else send some sort of nope msg?

        }

        public void HandleMessage(ConstructionMessage msg, object extra = null)
        {
            Client client = (Client)extra;
            Player player = GetPlayer(client);
            Assembly a = typeof(StationModule).Assembly;
            StationModule sm = (StationModule)Activator.CreateInstance(a.GetType("GalaxyShared."+msg.ClassName));
            sm.SetDataFromJSON();
            if (sm.CanBuild(player))
            {
                msg.Progress = 0;
                msg.ResourcesNeeded = sm.BuildRequirements;
                GalaxyServer.AddToSendQueue(client, msg);
            }

        }


        public static Player GetPlayer(Client client)
        {
            Player player;
            while (!PlayerTable.TryGetValue(client, out player)) { }
            return player;
        }

        public void HandleMessage(InputMessage input, object extra)
        {
            Client client = (Client)extra;
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
                BoundingSphere sphere = new BoundingSphere(a.Pos, a.Size*Asteroid.SERVER_SIZE_MULTIPLIER);
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
                player.Ship.AddCargo(new Item(ItemType.IronOre,player.Ship.MiningLaserPower));
                Console.WriteLine("hit!:" + hit.Remaining);
                //    GalaxyServer.AddToSendQueue(client, hit);
                MiningMessage miningState = new MiningMessage();
                miningState.Add = true;
                miningState.Item = new Item(ItemType.IronOre,player.Ship.MiningLaserPower);
                miningState.AsteroidHash = hit.Hash;
                miningState.Remaining = hit.Remaining;
                GalaxyServer.AddToSendQueue(client, miningState);
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
                    if (input.PrimaryButton) AsteroidMining(client, player);
                }
            }

            player.Stopwatch.Stop();
            Simulator.ContinuedPhysics(player,player.Stopwatch.ElapsedMilliseconds);
            player.Stopwatch.Restart();
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
            player.Stopwatch.Stop();
            
            Simulator.ContinuedPhysicsWarp(player, player.Stopwatch.ElapsedMilliseconds);
            player.Stopwatch.Restart();

        }

  

        //Stuff the server doesn't need to implement
        public void HandleMessage(NewUserResultMessage msg, object extra) {
            throw new NotImplementedException();
        }

        public void HandleMessage(LoginResultMessage msg, object extra = null)
        {
            throw new NotImplementedException();
        }

        public void HandleMessage(PlayerStateMessage msg, object extra = null)
        {
            throw new NotImplementedException();
        }

       

        public void HandleMessage(MiningMessage msg, object extra = null)
        {
            throw new NotImplementedException();
        }

       

        public void HandleMessage(Player msg, object extra = null)
        {
            throw new NotImplementedException();
        }

        public void HandleMessage(Ship msg, object extra = null)
        {
            throw new NotImplementedException();
        }

        public void HandleMessage(Asteroid msg, object extra = null)
        {
            throw new NotImplementedException();
        }

        
    }
}
