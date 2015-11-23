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
        public byte Orbit;
        [ProtoMember(2)]
        public float OrbitAngle;
        [ProtoMember(3)]
        public float Size;
        [ProtoMember(4)]
        public Vector3 Pos;
        [ProtoMember(5)]
        public ushort Remaining = 100;
        [ProtoMember(6)]
        public byte Type = 0;
        [ProtoMember(7)]
        public int Hash;
        

        public Asteroid(SolarSystem parentSystem, byte orbit, float orbitAngle, float size, Vector3 posAdjust, int hash)
        {
            this.Hash = hash;
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
