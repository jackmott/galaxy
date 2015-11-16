using XnaGeometry;
using ProtoBuf;
namespace GalaxyShared
{
    [ProtoContract]
    public class Asteroid
    {
        
        public SolarSystem ParentSystem;      
        public object GameObject;

        [ProtoMember(0)]
        public int Orbit;
        [ProtoMember(1)]
        public double OrbitAngle;
        [ProtoMember(2)]
        public double Size;
        [ProtoMember(3)]
        public Vector3 Pos;
        [ProtoMember(4)]
        public double Remaining = 100f;
        

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

        public int TypeID()
        {
            return 9;
        }
    }
}
