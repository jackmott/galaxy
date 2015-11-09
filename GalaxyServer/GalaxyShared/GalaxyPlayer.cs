using System;
using XnaGeometry;
using System.Collections.Generic;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyPlayer
    {
        public string UserName;
        public GalaxyLocation Location;
        public Quaternion Rotation;        
        public float Throttle;
        public long Seq;

        
        public GalaxyShip Ship;



        //new player
        public GalaxyPlayer(string userName)
        {
            UserName = userName;
            Location.SectorCoord = new SectorCoord(0, 0, 0);
            GalaxySector s = new GalaxySector(Location.SectorCoord);
            Location.SystemPos = s.GenerateSystems(1)[0].Pos;                             
            Rotation = Quaternion.Identity;
            Location.Pos = (Location.SystemPos * GalaxySector.EXPAND_FACTOR) + Vector3.Transform(Vector3.Forward * .3d, Rotation);
            Throttle = 0;
            Location.InWarp = true;
            Ship = new BustedHeap(this);
            Ship.Location = Location;
        }

        public GalaxyPlayer()
        { }

    }
}
