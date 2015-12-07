using System;
using System.Collections.Generic;

using XnaGeometry;



namespace GalaxyShared
{
    public class Simulator
    {
        public const double WARP_DISTANCE_THRESHOLD = 0.2d;

                   
        public static void ContinuedPhysics(Player player, long deltaTime)
        {                        
            double speed = player.Ship.TopSpeed * GetGravityInfluence(player);
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle*speed * deltaTime/1000d, player.Rotation);
        }

        public static void ProcessInput(Player player, InputMessage input)
        {

            if (input.Yaw != 0 || input.Pitch != 0 || input.Roll != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.Yaw, input.Pitch, input.Roll);            
                player.Rotation = player.Rotation * changeRotation;
                player.Rotation.Normalize();                 
            }
          
            player.Seq = input.Seq;
            player.Throttle = input.Throttle;
                                                                        

        }

       

        public static void ContinuedPhysicsWarp(Player player,long deltaTime)
        {
           
            player.Location.Pos += Vector3.Transform(Vector3.Forward * player.Throttle * player.Ship.TopSpeed * (deltaTime / 1500000d), player.Rotation);
        }

        public static void ProcessInputWarp(Player player, InputMessage input)
        {

            if (input.Yaw != 0 || input.Pitch != 0 || input.Roll != 0)
            {
                //rotate            
                Quaternion changeRotation = Quaternion.CreateFromYawPitchRoll(input.Yaw, input.Pitch, input.Roll);
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
