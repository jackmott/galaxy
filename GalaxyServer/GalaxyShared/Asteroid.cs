using System;
using XnaGeometry;

namespace GalaxyShared
{
    [Serializable]
    public class Asteroid
    {

        public SolarSystem ParentSystem;
        public int Orbit;
        public double OrbitAngle;
        
        public double Size;        
        public Vector3 Pos;
        

        public Asteroid(SolarSystem parentSystem, int orbit, double orbitAngle, double size, Vector3 posAdjust)
        {

            

            ParentSystem = parentSystem;
            Orbit = orbit;

            OrbitAngle = orbitAngle;
            Size = size;


            Vector3 start = Vector3.Zero;                                    
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle,0,0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit+1)*Planet.EARTH_CONSTANT*40, rotation);

          

            Pos += posAdjust;

        }
    }
}
