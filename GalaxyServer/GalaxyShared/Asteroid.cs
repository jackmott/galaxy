using XnaGeometry;
using ProtoBuf;
using System.IO;
namespace GalaxyShared
{
    [ProtoContract]
    public class Asteroid : IMessage
    {
        
        public SolarSystem ParentSystem;      
        public object GameObject;

        [ProtoMember(1)]
        public int Orbit;
        [ProtoMember(2)]
        public double OrbitAngle;
        [ProtoMember(3)]
        public double Size;
        [ProtoMember(4)]
        public Vector3 Pos;
        [ProtoMember(5)]
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

        public Asteroid()
        {

        }
      

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.Asteroid;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }

    }
}
