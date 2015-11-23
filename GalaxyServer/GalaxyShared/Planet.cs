using System;
using XnaGeometry;
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public class Planet
    {
        public const float EARTH_CONSTANT = 25;
        public const float ORBIT_MULTIPLIER = 20;

        
        public SolarSystem ParentSystem;
        FastRandom rand;

        [ProtoMember(1)]
        public int Orbit;
        [ProtoMember(2)]
        public float OrbitAngle;
        [ProtoMember(3)]
        public float RotationRate;
        [ProtoMember(4)]
        public float Size;
        [ProtoMember(5)]
        public Vector3 Pos;

        [ProtoMember(6)]
        public string DiscoveredBy = "Undisocvered";
        [ProtoMember(7)]
        public string Name = "Unnamed";
        [ProtoMember(8)]
        public string ClaimedBy = "Unclaimed";


        public Planet() { }
        public Planet(SolarSystem parentSystem, int orbit)
        {
            rand = new FastRandom(Convert.ToInt32(parentSystem.Pos.X), Convert.ToInt32(parentSystem.Pos.Y), Convert.ToInt32(parentSystem.Pos.Z), orbit);
            
            ParentSystem = parentSystem;
            Orbit = orbit;
            RotationRate = rand.Next(.01f, .1f);
            OrbitAngle = rand.Next(0f, (float)MathHelper.TwoPi);
            Size = rand.Next(2.5f, 14f);
            
            Vector3 start = Vector3.Zero;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(OrbitAngle, 0, 0);
            Pos = start + Vector3.Transform(Vector3.Forward * (Orbit + 1) * Planet.EARTH_CONSTANT * ORBIT_MULTIPLIER, rotation);


        }
    }
}
