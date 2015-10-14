using System.Collections.Generic;

namespace GalaxyShared
{
    public class GalaxySector
    {
        public GalaxyCoord coord;

        public List<SolarSystem> systems;

        public GalaxySector(GalaxyCoord coord)
        {
            this.coord = coord;
            systems = new List<SolarSystem>();
        }

    }
}
