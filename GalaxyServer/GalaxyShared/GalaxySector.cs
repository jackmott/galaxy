using System;
using System.Collections.Generic;
using System.Drawing;

namespace GalaxyShared
{
    public class GalaxySector
    {
        public SectorCoord coord;
        public Color sectorColor;
        public List<SolarSystem> systems;
        public int dominantStarType;

        public static int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public static int SECTOR_SIZE = 25; //light years cubed
        public static int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;

        public GalaxySector(SectorCoord coord, Bitmap hoag, int everyNth)
        {
            this.coord = coord;            
            systems = new List<SolarSystem>();

            
            Random r = new Random(coord.x + coord.y * SECTOR_SIZE + coord.z * SECTOR_SIZE * SECTOR_SIZE);

            int HALF_SECTOR_SIZE = SECTOR_SIZE / 2;
            //things to be looked up from data somehow
            float lightYearsPerPixel = GALAXY_SIZE_LIGHTYEARS / hoag.Width;

            int adjustedSectorX = coord.x + GALAXY_SIZE_SECTORS / 2;
            int adjustedSectorY = coord.y + GALAXY_SIZE_SECTORS / 2;

            int pixelX = Convert.ToInt32(adjustedSectorX * SECTOR_SIZE / lightYearsPerPixel);
            int pixelY = Convert.ToInt32(adjustedSectorY * SECTOR_SIZE / lightYearsPerPixel);
            sectorColor = hoag.GetPixel(pixelX, pixelY);
            float saturation = sectorColor.GetSaturation();
            if (saturation < .4) dominantStarType = Star.F;
            else
            {
                dominantStarType = Star.hueToType[Convert.ToInt32(sectorColor.GetHue() / 30)];
            }

            float sectorIntensity = sectorColor.GetBrightness();
            if (sectorIntensity < .1) sectorIntensity = .01f;
            float STELLAR_DENSITY = .014f * sectorIntensity; //stars per cubic light year
            //if (STELLAR_DENSITY == 0) STELLAR_DENSITY = .00001f;
            int sectorCubed = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;  //how many lights years cubed are there?
            float avgDeltaF = 1f / STELLAR_DENSITY;
            int avgDelta = Convert.ToInt32(avgDeltaF * 2f); //for average
            avgDelta *= everyNth;

            /* Debug.WriteLine("X,Y=(" + pixelX + "," + pixelY + ")");
             Debug.WriteLine("intensity=" + sectorIntensity);
             Debug.WriteLine("StellarDen=" + STELLAR_DENSITY);
             Debug.WriteLine("avgDelta2=" + avgDelta);
             */
            int i = 0;

            while (true)
            {
                int delta = r.Next(1, avgDelta + 1);

                i = i + delta;
                if (i >= sectorCubed)
                {
                    break;
                }

                float x = (float)r.NextDouble() - .5f;
                float y = (float)r.NextDouble() - .5f;
                float z = (float)r.NextDouble() - .5f;

                int index = i;
                x += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                y += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                z += index - HALF_SECTOR_SIZE;


                SolarSystem system = new SolarSystem(new SystemCoord(x, y, z), this, sectorColor, 1, r);
                systems.Add(system);

            }
        }

    }
}
