using System;
using XnaGeometry;

namespace GalaxyShared
{
    [Serializable]
    public class Planet
    {
        public const float EARTH_CONSTANT = 25;
        public const float ORBIT_MULTIPLIER = 20;

        [NonSerialized]
        public SolarSystem ParentSystem;

        public int Orbit;
        public double OrbitAngle;
        public double RotationRate;        
        public double Size;
        public Vector3 Pos;

        [NonSerialized]
        FastRandom rand;

        public string DiscoveredBy = "Undisocvered";
        public string Name = "Unnamed";
        public string ClaimedBy = "Unclaimed";



        public Planet(SolarSystem parentSystem, int orbit)
        {
            rand = new FastRandom(Convert.ToInt32(parentSystem.Pos.X), Convert.ToInt32(parentSystem.Pos.Y), Convert.ToInt32(parentSystem.Pos.Z), orbit);
            
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = rand.Next(.01d, .1d);
            OrbitAngle = rand.Next(0d, MathHelper.TwoPi);
            Size = rand.Next(2.5d, 14d);
            
            Vector3 start = Vector3.Zero;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle, 0, 0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit + 1) * Planet.EARTH_CONSTANT * ORBIT_MULTIPLIER, rotation);


        }
    }
}
