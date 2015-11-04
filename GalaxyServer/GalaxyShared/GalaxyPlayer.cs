using System;

namespace GalaxyShared
{
    [Serializable]
    public class GalaxyPlayer
    {
        public string UserName;
        public SectorCoord SectorPos;
        public int SystemIndex;
        public Coord PlayerPos;
        public GalaxyShip ship;

        [NonSerialized]        
        public GalaxyClient Client;

       


        //new player
        public GalaxyPlayer(string UserName)
        {
            SectorPos= new SectorCoord(100, 2500, 2500);
            GalaxyGen gen = new GalaxyGen();
            GalaxySector sector = gen.GetSector(SectorPos, 1);
            SystemIndex = 0;
            PlayerPos = new Coord(1000, 0, 0);
                        
        }

        public GalaxyPlayer()
        { }

    }
}
