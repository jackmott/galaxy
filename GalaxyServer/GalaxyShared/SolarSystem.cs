using System;
namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector sector;
        public GalaxyCoord coord;

        public SolarSystem(GalaxyCoord coord, GalaxySector sector)
        {
            this.coord = coord;
            this.sector = sector;
            
        }
    }
}
