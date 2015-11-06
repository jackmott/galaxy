using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyShared;
using XnaGeometry;



namespace GalaxyShared
{
    public class Simulator
    {


        public static void ProcessTickForPlayer(Queue<InputMessage> inputs, GalaxyPlayer player)
        {
            lock (inputs)
            {
                if (inputs.Count > 0)
                {
                    InputMessage input = inputs.Dequeue();
                    ProcessInput(player, input);
                }
            }
            ContinuedPhysics(player);            
        }

        public static void ContinuedPhysics(GalaxyPlayer player)
        {
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle*player.Ship.TopSpeed / NetworkUtils.SERVER_TICK_RATE, player.Rotation);
        }

        public static void ProcessInput(GalaxyPlayer player, InputMessage input)
        {

            //   go.transform.Translate(Vector3.forward * input.Throttle * 40 * GalaxyClient.TICK_RATE);
           // Matrix rotation = Matrix.CreateFromQuaternion(player.Rotation);
            if (input.XTurn != 0 || input.YTurn != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
                //Matrix changeRotation = Matrix.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();
                 

            }
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;
                                                                        

        }
    }
}
