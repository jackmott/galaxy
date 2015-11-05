
using XnaGeometry;

namespace GalaxyShared
{
    public class Planet
    {
        public const float EARTH_CONSTANT = 50;

        public SolarSystem ParentSystem;
        public int Orbit;
        public double OrbitAngle;
        public float RotationRate;
        public int Hash;
        public float Size;
        public Vector3 Pos;

        public Planet(SolarSystem parentSystem, int orbit, System.Random r)
        {
                        
            Hash = orbit ^ parentSystem.Hash;            
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = GalaxyGen.RandomRange(r, .01f, .1f);
            OrbitAngle = GalaxyGen.RandomRange(r, 0f, MathHelper.TwoPi);
            Size = GalaxyGen.RandomRange(r, 2.5f, 14f);
            
            Vector3 start = Vector3.Zero;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle, 0, 0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit + 1) * Planet.EARTH_CONSTANT * 40, rotation);


        }
    }
}
