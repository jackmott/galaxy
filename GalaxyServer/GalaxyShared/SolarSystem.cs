using System;
namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector sector;
        public GalaxyCoord coord;
        public GalaxyGen.StarType type;
        public int size;

        public SolarSystem(GalaxyCoord coord, GalaxySector sector, GalaxyGen.StarType type, int size)
        {
            this.coord = coord;
            this.sector = sector;
            this.type = type;
            this.size = size;
        }
    }
}
