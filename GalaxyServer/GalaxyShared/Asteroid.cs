using System;
using XnaGeometry;

namespace GalaxyShared
{
    public class Asteroid
    {
        public SolarSystem ParentSystem;
        public int Orbit;
        public double OrbitAngle;
        public double RotationRate;        
        public double Size;        
        public Vector3 Pos;

        public FastRandom rand;

        public Asteroid(SolarSystem parentSystem, int orbit)
        {

            rand = new FastRandom(Convert.ToInt32(parentSystem.Pos.X), Convert.ToInt32(parentSystem.Pos.Y), Convert.ToInt32(parentSystem.Pos.Z), orbit);

            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = rand.Next( .01f, .1f);
            OrbitAngle = rand.Next( 0f, MathHelper.TwoPi);
            Size = rand.Next( 1f, 3.5f);


            Vector3 start = Vector3.Zero;                                    
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle,0,0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit+1)*Planet.EARTH_CONSTANT*40, rotation);

            double magnitude = 25000;
            double xAdjust = rand.Next( -magnitude, magnitude);
            double yAdjust = rand.Next( -magnitude, magnitude);
            double zAdjust = rand.Next( -magnitude, magnitude);

            Pos.X += xAdjust;
            Pos.Y += yAdjust;
            Pos.Z += zAdjust;

        }
    }
}
