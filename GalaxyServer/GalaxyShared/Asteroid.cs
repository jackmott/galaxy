using System;
using XnaGeometry;

namespace GalaxyShared
{
    public class Asteroid
    {
        public SolarSystem ParentSystem;
        public int Orbit;
        public double OrbitAngle;
        public float RotationRate;
        public int Hash;
        public float Size;
        public Random R;
        public Vector3 Pos;
        


        public Asteroid(SolarSystem parentSystem, int orbit, System.Random r)
        {            
            R = r;
            Hash = orbit ^ parentSystem.Hash;
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = GalaxyGen.RandomRange(r, .01f, .1f);
            OrbitAngle = GalaxyGen.RandomRange(r, 0f, MathHelper.TwoPi);
            Size = GalaxyGen.RandomRange(r, 1f, 3.5f);


            Vector3 start = Vector3.Zero;                                    
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle,0,0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit+1)*Planet.EARTH_CONSTANT*40, rotation);

            float magnitude = 25000;
            float xAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);
            float yAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);
            float zAdjust = GalaxyGen.RandomRange(R, -magnitude, magnitude);

            Pos.X += xAdjust;
            Pos.Y += yAdjust;
            Pos.Z += zAdjust;

        }
    }
}
