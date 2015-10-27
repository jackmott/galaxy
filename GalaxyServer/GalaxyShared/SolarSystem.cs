using System.Drawing;

namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector sector;
        public SystemCoord coord;
        public GalaxyGen.StarType type;
        public int size;
        public Color color;

        public SolarSystem(SystemCoord coord, GalaxySector sector, Color color, int size)
        {
            this.coord = coord;
            this.sector = sector;
            this.color = color;
            this.size = size;
        }
    }
}
