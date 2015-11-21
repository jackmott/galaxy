using XnaGeometry;
using ProtoBuf;

namespace GalaxyShared
{
    [ProtoContract]
    public class Player
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

        
        public Ship Ship;
        
        public SolarSystem SolarSystem;


        //new player
        public Player(string userName)
        {
            UserName = userName;
            Location.SectorCoord = new SectorCoord(0, 0, 0);
            Sector s = new Sector(Location.SectorCoord);
            Location.SystemPos = s.GenerateSystems(1)[0].Pos;                             
            Rotation = Quaternion.Identity;
            Location.Pos = (Location.SystemPos * Sector.EXPAND_FACTOR) + Vector3.Transform(Vector3.Forward * .3d, Rotation);
            Throttle = 0;
            Location.InWarp = true;
            Ship = new Ship(this);
            Ship.Location = Location;
        }

        public int TypeID()
        {
            return 4;
        }

    }
}
