using System;
using XnaGeometry;
using System.Collections.Generic;

namespace GalaxyShared
{
    [Serializable]
    public class Player
    {
        public string UserName;
        public Location Location;
        public Quaternion Rotation;        
        public float Throttle;
        public long Seq;

        
        public Ship Ship;

        [NonSerialized]
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
            Ship = new BustedHeap(this);
            Ship.Location = Location;
        }

        public Player()
        { }

    }
}
