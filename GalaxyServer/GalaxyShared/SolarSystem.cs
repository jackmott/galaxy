using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector sector;
        public SystemCoord coord;
        public Object clientCoord;
        public float clientDistance = float.MaxValue;
        public Star star;
        public int size;
        public Color color;
        List<Planet> planets;

        public SolarSystem(SystemCoord coord, GalaxySector sector, Color color, int size, Random r)
        {
            this.coord = coord;
            this.sector = sector;
            this.color = color;
            this.size = size;
            star = new Star(sector, r);
        }

        public void Generate()
        {
            int hash1 = sector.coord.x + sector.coord.y * GalaxySector.SECTOR_SIZE + sector.coord.z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE;
            int hash2 = Convert.ToInt32(coord.x + coord.y * GalaxySector.SECTOR_SIZE + coord.z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE);

            Random r = new Random(hash1 ^ hash2);

            int numPlanets = r.Next(0, 20);
            
            for (int i = 0; i < numPlanets; i++)
            {
                Planet p = new Planet(this);
            }
                
        }

    }
}
