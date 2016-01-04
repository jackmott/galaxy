using XnaGeometry;
using ProtoBuf;
using System.IO;


namespace GalaxyShared
{
    [ProtoContract]
    public class Player : IMessage
    {
        [ProtoMember(1)]
        public string UserName;
        [ProtoMember(2)]
        public Location Location; 
        [ProtoMember(3)]
        public Quaternion Rotation;
        [ProtoMember(4)]
        public float Throttle;
        [ProtoMember(5)]
        public long Seq;
        [ProtoMember(6)]
        public Ship Ship;
        
        public SolarSystem SolarSystem;

        public long LastPhysicsUpdate = -10;


        //new player
        public Player(string userName)
        {
            
            UserName = userName;
            Location.SectorCoord = new SectorCoord(Galaxy.GALAXY_SIZE_SECTORS/2, Galaxy.GALAXY_SIZE_SECTORS/2, Galaxy.GALAXY_THICKNESS_SECTORS/2);
            Sector s = new Sector(Location.SectorCoord);
            Location.SystemPos = s.GenerateSystems()[0].Pos;                             
            Rotation = Quaternion.Identity;
            Location.Pos = (Location.SystemPos * Galaxy.EXPAND_FACTOR) + Vector3.Transform(Vector3.Forward * .3d, Rotation);
            Throttle = 0;
            Location.InWarp = true;
            Ship = new Ship(this);
            Ship.Location = Location;
        }

        public Player()
        {
          
        }

       

        public void Proto(Stream stream, byte[] typeBuffer)
        {
            typeBuffer[0] = (byte)MsgType.Player;
            stream.Write(typeBuffer, 0, 1);
            Serializer.SerializeWithLengthPrefix(stream, this, PrefixStyle.Fixed32);
        }

        public void AcceptHandler(IMessageHandler handler, object o = null)
        {
            handler.HandleMessage(this, o);
        }

    }
}
