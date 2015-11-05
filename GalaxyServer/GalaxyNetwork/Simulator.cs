using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared.Networking.Messages;
using GalaxyShared;
using XnaGeometry;
using System.Diagnostics;
using System.Threading;
using GalaxyShared.Networking;

namespace GalaxyServer
{
    public class Simulator
    {


        public static Stopwatch sw = new Stopwatch();

        public static void DoPhysics()
        {
            while (true)
            {
                sw.Restart();
                foreach (GalaxyClient c in LogicLayer.PlayerTable.Keys)
                {
                    GalaxyPlayer p;
                    while (!LogicLayer.PlayerTable.TryGetValue(c, out p)) { }
                    ProcessTickForPlayer(c, p);
                }
                sw.Stop();
                Thread.Sleep(Convert.ToInt32(MathHelper.Clamp(NetworkUtils.SERVER_TICK_RATE - sw.ElapsedMilliseconds, 0, 100)));
                
                
            }
            
        }

        public static void ProcessTickForPlayer(GalaxyClient client, GalaxyPlayer player)
        {
            
            
            InputMessage input;
            if (client.Inputs.TryDequeue(out input))
            {
                ProcessInput(player, input);
            }
            

            //keep them moving
            //todo 50 is a placeholder for ship throttle multiplier
            player.PlayerPos += Vector3.Transform(Vector3.Forward * player.Throttle/NetworkUtils.SERVER_TICK_RATE*50, player.Rotation);
            
            long deltaT = DateTime.Now.Subtract(client.LastSend).Milliseconds;
            if (deltaT >= client.ClientSendRate)
            {
                PlayerStateMessage pState = new PlayerStateMessage();
                pState.PlayerPos = player.PlayerPos;
                pState.Rotation = player.Rotation;
                pState.Throttle = player.Throttle;
                GalaxyServer.AddToSendQueue(client, pState);                
                client.LastSend = DateTime.Now;
            }
        }

        public static void ProcessInput(GalaxyPlayer player, InputMessage input)
        {

            //   go.transform.Translate(Vector3.forward * input.Throttle * 40 * GalaxyClient.TICK_RATE);
            Matrix rotation = Matrix.CreateFromQuaternion(player.Rotation);
            if (input.XTurn != 0 || input.YTurn != 0)
            {
                //rotate                
                Matrix changeRotation = Matrix.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
                rotation = rotation * changeRotation;
                player.Rotation = Quaternion.CreateFromRotationMatrix(rotation);             
            }
            player.Throttle = input.Throttle;
                                                                        

        }
    }
}
