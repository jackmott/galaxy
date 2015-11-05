using System;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using XnaGeometry;


namespace GalaxyServer
{
    class LogicLayer
    {
        
        private static ConcurrentDictionary<GalaxyClient, GalaxyPlayer> PlayerTable = new ConcurrentDictionary<GalaxyClient, GalaxyPlayer>();


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
            PlayerTable.TryAdd(client, player);
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

       
        public static void HandleInputs(object o, GalaxyClient client)
        {
           
            List<InputMessage> msgs = (List<InputMessage>)o;           
            GalaxyPlayer player;
         
            PlayerTable.TryGetValue(client, out player);
         
            foreach (InputMessage msg in msgs )
            {
                UpdateState(player, msg);
            }
            
                        
            PlayerStateMessage pState = new PlayerStateMessage();
            pState.PlayerPos = player.PlayerPos;
            pState.rotation = player.rotation;     
            GalaxyServer.Send(client, pState);
            

        }

        public static  void UpdateState(GalaxyPlayer player, InputMessage input)
        {
          
            //   go.transform.Translate(Vector3.forward * input.Throttle * 40 * GalaxyClient.TICK_RATE);

            //rotate
            Matrix startRotation = Matrix.CreateFromQuaternion(player.rotation);
            Matrix changeRotation = Matrix.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
            Matrix result = startRotation * changeRotation;
            player.rotation = Quaternion.CreateFromRotationMatrix(result);

            //translate
          
            player.PlayerPos += Vector3.Transform(Vector3.Forward*input.Throttle, result);
            player.Throttle = input.Throttle;
          

        }

    }
}
