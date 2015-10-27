using System.Collections.Generic;

namespace GalaxyShared
{
    public class GalaxySector
    {
        public SectorCoord coord;

        public List<SolarSystem> systems;

        public GalaxySector(SectorCoord coord)
        {
            this.coord = coord;
            systems = new List<SolarSystem>();
        }

    }
}
