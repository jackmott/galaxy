using System;
using XnaGeometry;
using GalaxyShared.Networking.Messages;
using System.Collections.Generic;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyPlayer
    {
        public string UserName;
        public SectorCoord SectorPos;
        public int SystemIndex;
        public Vector3 PlayerPos;
        public Quaternion Rotation;
        public GalaxyShip Ship;
        public float Throttle;

                              

        //new player
        public GalaxyPlayer(string UserName)
        {
            SectorPos= new SectorCoord(100, 2500, 2500);
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(SectorPos, 1);
            SystemIndex = 0;
            PlayerPos = new Vector3(0, 0, -5000);
            Rotation = Quaternion.Identity;
            Throttle = 0;
                        
        }

        public GalaxyPlayer()
        { }

    }
}
