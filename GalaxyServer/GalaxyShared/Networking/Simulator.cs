using System.Collections.Generic;

using XnaGeometry;



namespace GalaxyShared
{
    public class Simulator
    {
        public const double WARP_DISTANCE_THRESHOLD = 0.2d;

        public static void ProcessTickForPlayer(Queue<InputMessage> inputs, Player player)
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

        public static SolarSystem GetClosestSystem(Sector sector, Vector3 pos)
        {
            double minDistance = double.MaxValue;
            SolarSystem closeSystem = null;
            foreach (SolarSystem s in sector.Systems)
            {
                double distance = Vector3.Distance(pos, s.Pos*Sector.EXPAND_FACTOR);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closeSystem = s;
                }
            }
            return closeSystem;
        }

     
        public static void ContinuedPhysics(Player player)
        {            
            double speed = player.Ship.TopSpeed * GetGravityInfluence(player);
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle*speed / NetworkUtils.SERVER_TICK_RATE, player.Rotation);
        }

        public static void ProcessInput(Player player, InputMessage input)
        {

            if (input.XTurn != 0 || input.YTurn != 0 || input.RollTurn != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);            
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();                 
            }
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;
                                                                        

        }

        public static void ProcessTickForPlayerWarp(Queue<InputMessage> inputs, Player player)
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

        public static void ContinuedPhysicsWarp(Player player)
        {
           
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle * player.Ship.TopSpeed / NetworkUtils.SERVER_TICK_RATE / 2000d, player.Rotation);
        }

        public static void ProcessInputWarp(Player player, InputMessage input)
        {

            if (input.XTurn != 0 || input.YTurn != 0 || input.RollTurn != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.XTurn, input.YTurn, input.RollTurn);
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();
            }
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;


        }

        //Finds the closest planet, computes 'gravity' to use to scale down ship speed
        public static double GetGravityInfluence(Player player)
        {
            SolarSystem system = player.SolarSystem;
            if (system.Planets != null)
            {
                double closestDistance = double.MaxValue;
                Planet closestPlanet = null;
                foreach (Planet p in system.Planets)
                {
                    

                    double distance = Vector3.Distance(p.Pos, player.Location.Pos);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlanet = p;
                    }

                }
                closestDistance = closestDistance - closestPlanet.Size/2d * Planet.EARTH_CONSTANT;
                return MathHelper.Clamp(closestDistance / 4000d, .001d, 1d);
            } else
            {
                return 0d;
            }
        }



    }
}
