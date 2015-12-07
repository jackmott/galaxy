using System;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using XnaGeometry;
using System.Reflection;

namespace GalaxyServer
{
    public class LogicLayer : IMessageHandler
    {
        private static object ClientMovementLock = new object();
        public static ConcurrentDictionary<string, SolarSystem> LoadedSystems = new ConcurrentDictionary<string, SolarSystem>();
        public static LinkedList<Client> ClientsInWarp = new LinkedList<Client>();


        public static readonly Vector3 SYSTEM_START_POS = new Vector3(0, 0, -5000);


        public static void MoveClientToWarp(Client c)
        {
            lock (ClientMovementLock)
            {
                if (c.Player.SolarSystem != null)
                {
                    SolarSystem system = c.Player.SolarSystem;
                    c.Player.SolarSystem.Clients.Remove(c);
                    if (system.Clients.Count == 0) LoadedSystems.TryRemove(system.key(), out system);

                }
                c.Player.SolarSystem = null;
                ClientsInWarp.AddLast(c);
                c.Player.Location.InWarp = true;
            }
        }

        public static void MoveClientToSystem(Client c, SolarSystem s)
        {
            lock (ClientMovementLock)
            {
                ClientsInWarp.Remove(c);
                if (!LoadedSystems.ContainsKey(s.key()))
                {
                    LoadedSystems.TryAdd(s.key(), s);
                }
                s.Clients.AddLast(c);
                c.Player.SolarSystem = s;
                c.Player.Location.InWarp = false;
            }
        }

        public static void RemoveClientFromAll(Client c)
        {
            lock (ClientMovementLock)
            {
                if (c.Player.SolarSystem != null)
                {
                    c.Player.SolarSystem.Clients.Remove(c);
                }
                ClientsInWarp.Remove(c);
            }
        }

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
            client.Player = player;
            ClientsInWarp.AddLast(client);
            GalaxyServer.AddToSendQueue(client, player);
            Console.WriteLine("Player Data Sent");

        }


        public void HandleMessage(NewUserMessage msg, object extra)
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

            msg.Rotation = client.Player.Rotation;

            Vector3 systemPos = client.Player.Location.SystemPos;
            Vector3 startPos = new Vector3(systemPos.X * Sector.EXPAND_FACTOR, systemPos.Y * Sector.EXPAND_FACTOR, systemPos.Z * Sector.EXPAND_FACTOR);
            startPos += Vector3.Transform(Vector3.Forward * .3d, client.Player.Rotation);
            client.Player.Location.Pos = startPos;

            //client has left, clear em out
            SolarSystem system = client.Player.SolarSystem;
            MoveClientToWarp(client);

            msg.Location = client.Player.Location;

            GalaxyServer.AddToSendQueue(client, msg);

        }

        public void HandleMessage(DropOutOfWarpMessage msg, object extra)
        {

            Client client = (Client)extra;
            Player player = client.Player;
            if (!player.Location.InWarp)
            {
                Console.WriteLine("Drop out of warp msg received when not in warp");
                return;
            }
            SolarSystem system;
            //First see if system is loaded in memory already
            if (!LoadedSystems.TryGetValue(msg.SystemKey, out system))
            {
                Sector sector = new Sector(msg.SectorCoord);
                //If not check the data store
                system = DataLayer.GetSystem(msg.SystemKey);
                //If not, generate the system
                if (system == null)
                {                    
                    system = sector.GenerateSystem(msg.SystemIndex);
                    system.ParentSector = sector;
                    system.Generate();
                    DataLayer.AddSystem(system);
                    Console.WriteLine("System Generated and Added to Redis");
                } else
                {
                    Console.WriteLine("System Loaded From Reids");
                }
                system.ParentSector = sector;
                system.Clients = new LinkedList<object>();
            }

            double distance = Vector3.Distance(player.Location.Pos, system.Pos * Sector.EXPAND_FACTOR);
            Console.WriteLine("Distance:" + distance);
            //give some wiggle room since server/client will not be perfectly in sync
            if (system != null && distance <= Simulator.WARP_DISTANCE_THRESHOLD * 2)
            {
                player.Location.Pos = SYSTEM_START_POS;
                player.Location.SystemPos = system.Pos;
                player.Location.SectorCoord = system.ParentSector.Coord;

                MoveClientToSystem(client, system);

                msg.Location = player.Location;
                msg.Rotation = player.Rotation;
                msg.System = system;
                GalaxyServer.AddToSendQueue(client, msg);
                return;
            }

            //todo else send some sort of nope msg?

        }

        public void HandleMessage(ConstructionMessage msg, object extra = null)
        {
            Client client = (Client)extra;
            Player player = client.Player;
            Assembly a = typeof(StationModule).Assembly;
            StationModule sm = (StationModule)Activator.CreateInstance(a.GetType("GalaxyShared." + msg.ClassName));
            sm.SetDataFromJSON();
            if (sm.CanBuild(player))
            {
                msg.Progress = 0;
                msg.ResourcesNeeded = sm.BuildRequirements;
                GalaxyServer.AddToSendQueue(client, msg);
            }

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
                BoundingSphere sphere = new BoundingSphere(a.Pos, a.Size * Asteroid.SERVER_SIZE_MULTIPLIER);
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
                player.Ship.AddCargo(new Item(ItemType.IronOre, player.Ship.MiningLaserPower));
                Console.WriteLine("hit!:" + hit.Remaining);
                //    GalaxyServer.AddToSendQueue(client, hit);
                MiningMessage miningState = new MiningMessage();
                miningState.Add = true;
                miningState.Item = new Item(ItemType.IronOre, player.Ship.MiningLaserPower);
                miningState.AsteroidHash = hit.Hash;
                miningState.Remaining = hit.Remaining;
                GalaxyServer.AddToSendQueue(client, miningState);
                if (hit.Remaining <= 0) asteroids.Remove(hit);
                DataLayer.AddSystem(player.SolarSystem);
            }
        }


        private static void SendStateUpdate(Client client)
        {
            if (GalaxyServer.Millis - client.LastSend > client.ClientSendRate)
            {
                Player player = client.Player;
                PlayerStateMessage pState = new PlayerStateMessage();
                pState.PlayerPos = player.Location.Pos;
                pState.Rotation = player.Rotation;
                pState.Throttle = player.Throttle;
                pState.Seq = player.Seq;
                GalaxyServer.AddToSendQueue(client, pState);
                player.Seq++;
                client.LastSend = GalaxyServer.Millis;
            }
        }

        private static void Persist(Client client)
        {
            if (GalaxyServer.Millis - client.LastPersist > DataLayer.PLAYER_STATE_PERSIST_RATE)
            {
                DataLayer.UpdateGalaxyPlayer(client.Player);
                client.LastPersist = GalaxyServer.Millis;
            }
        }

        private static void ProcessWarpPlayers()
        {           
                foreach (Client client in ClientsInWarp)
                {
                    Player player = client.Player;
                    ProcessTickForPlayerWarp(client, client.Player);
                    SendStateUpdate(client);
                    Persist(client);
                }                        
        }

        private static void ProcessSystemStuff()
        {
            foreach (SolarSystem system in LoadedSystems.Values)
            {
                ProcessSystemPlayers(system);
                ProcessSystemConstructionModules(system);
                
            }
        }

        private static void ProcessSystemConstructionModules(SolarSystem system)
        {

        }
        
        private static void ProcessSystemPlayers(SolarSystem system)
        {
            foreach (object o in system.Clients)
            {
                Client client = (Client)o;
                Player player = client.Player;

                ProcessTickForPlayer(client, player);
                SendStateUpdate(client);
                Persist(client);

            }
        }
        //player physics will update once per tick
        public static void DoPhysics()
        {
            
            while (true)
            {
                long millis = GalaxyServer.Millis;
                ProcessSystemStuff();
                ProcessWarpPlayers();
                millis = GalaxyServer.Millis - millis;
                Thread.Sleep(Math.Max(0,(int)(NetworkUtils.SERVER_TICK_RATE - millis)));
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

            
            Simulator.ContinuedPhysics(player,GalaxyServer.Millis - player.LastPhysicsUpdate);
            player.LastPhysicsUpdate = GalaxyServer.Millis;
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
            
            Simulator.ContinuedPhysicsWarp(player, GalaxyServer.Millis - player.LastPhysicsUpdate);
            player.LastPhysicsUpdate = GalaxyServer.Millis;
            
        }



        //Stuff the server doesn't need to implement
        public void HandleMessage(NewUserResultMessage msg, object extra)
        {
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
