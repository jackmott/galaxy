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
            Location.SectorCoord = new SectorCoord(100, 2500, 2500);
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(Location.SectorCoord, 1);
            Location.SystemIndex = 0;
            Location.Pos = new Vector3(0, 0, -5000);
            Rotation = Quaternion.Identity;
            Throttle = 0;

            Ship = new BustedHeap(this);
            Ship.Location = Location;
        }

        public GalaxyPlayer()
        { }

    }
}
