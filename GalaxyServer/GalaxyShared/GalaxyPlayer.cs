using System;
using XnaGeometry;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyPlayer
    {
        public string UserName;
        public SectorCoord SectorPos;
        public int SystemIndex;
        public Vector3 PlayerPos;
        public Quaternion rotation;
        public GalaxyShip ship;
        public float Throttle;

        [NonSerialized]        
        public GalaxyClient Client;

       


        //new player
        public GalaxyPlayer(string UserName)
        {
            SectorPos= new SectorCoord(100, 2500, 2500);
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(SectorPos, 1);
            SystemIndex = 0;
            PlayerPos = new Vector3(0, 0, -5000);
            rotation = Quaternion.Identity;
            Throttle = 0;
                        
        }

        public GalaxyPlayer()
        { }

    }
}
