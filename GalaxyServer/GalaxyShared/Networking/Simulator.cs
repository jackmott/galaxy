using System.Collections.Generic;

using XnaGeometry;



namespace GalaxyShared
{
    public class Simulator
    {
        public const double WARP_DISTANCE_THRESHOLD = 0.2d;

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

        public static SolarSystem GetClosestSystem(GalaxySector sector, Vector3 pos)
        {
            double minDistance = double.MaxValue;
            SolarSystem closeSystem = null;
            foreach (SolarSystem s in sector.Systems)
            {
                double distance = Vector3.Distance(pos, s.Pos*GalaxySector.EXPAND_FACTOR);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closeSystem = s;
                }
            }
            return closeSystem;
        }

     
        public static void ContinuedPhysics(GalaxyPlayer player)
        {
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle*player.Ship.TopSpeed / NetworkUtils.SERVER_TICK_RATE, player.Rotation);
        }

        public static void ProcessInput(GalaxyPlayer player, InputMessage input)
        {
                      
            if (input.XTurn != 0 || input.YTurn != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);            
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();                 
            }
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;
                                                                        

        }

        public static void ProcessTickForPlayerWarp(Queue<InputMessage> inputs, GalaxyPlayer player)
        {
            lock (inputs)
            {
                if (inputs.Count > 0)
                {
                    InputMessage input = inputs.Dequeue();
                    ProcessInputWarp(player, input);
                }
            }
            ContinuedPhysicsWarp(player);
        }

        public static void ContinuedPhysicsWarp(GalaxyPlayer player)
        {
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle * player.Ship.TopSpeed / NetworkUtils.SERVER_TICK_RATE / 200d, player.Rotation);
        }

        public static void ProcessInputWarp(GalaxyPlayer player, InputMessage input)
        {

            if (input.XTurn != 0 || input.YTurn != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();
            }
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;


        }




    }
}
