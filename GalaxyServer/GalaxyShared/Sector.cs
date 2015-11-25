using System;
using System.Collections.Generic;
using System.Drawing;
using XnaGeometry;


namespace GalaxyShared
{
    public class Sector
    {
        public static Bitmap Hoag;

        public SectorCoord Coord;
        public Color Color;
        public List<SolarSystem> Systems;
        public int DominantStarType;
        public float StellarDensity;
        

        public const int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public const int SECTOR_SIZE = 25; //light years cubed
        public const int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;
        public const double EXPAND_FACTOR = 1d / 2.5d; //multiplied by galaxy coordinates to get unity coordinates
        public const int HALF_SECTOR_SIZE = SECTOR_SIZE / 2;

        FastRandom r;


        public Sector(SectorCoord coord)
        {
            if (Hoag == null)
            {
                try
                {
                    Hoag = (Bitmap)Bitmap.FromFile("Assets/Plugins/GalaxyShared/hoag-ring.bmp");
                }
                catch
                {
                    Hoag = (Bitmap)Bitmap.FromFile("assets/hoag-ring.bmp");
                }
            }
            this.Coord = coord;            
            Systems = new List<SolarSystem>();            
            r = new FastRandom(coord.X,coord.Y,coord.Z);

            
            //things to be looked up from data somehow
            float lightYearsPerPixel = GALAXY_SIZE_LIGHTYEARS / Hoag.Width;

            int adjustedSectorX = coord.X + GALAXY_SIZE_SECTORS / 2;
            int adjustedSectorY = coord.Y + GALAXY_SIZE_SECTORS / 2;

            int pixelX = Convert.ToInt32(adjustedSectorX * SECTOR_SIZE / lightYearsPerPixel);
            int pixelY = Convert.ToInt32(adjustedSectorY * SECTOR_SIZE / lightYearsPerPixel);
            Color = Hoag.GetPixel(pixelX, pixelY);
            float saturation = Color.GetSaturation();
            if (saturation < .4) DominantStarType = Star.F;
            else
            {
                DominantStarType = Star.hueToType[Convert.ToInt32(Color.GetHue() / 30)];
            }

            float sectorIntensity = Color.GetBrightness();
            if (sectorIntensity < .1) sectorIntensity = .01f;
            StellarDensity = .014f * sectorIntensity; //stars per cubic light year
            //if (STELLAR_DENSITY == 0) STELLAR_DENSITY = .00001f;
           
        }

        public List<SolarSystem> GenerateSystems(int everyNth)
        {
            int sectorCubed = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;  //how many lights years cubed are there?
            float avgDeltaF = 1f / StellarDensity;
            int avgDelta = Convert.ToInt32(avgDeltaF * 2f); //for average
            avgDelta *= everyNth;


            //System.Diagnostics.Debug.WriteLine("pixel X,Y=(" + pixelX + "," + pixelY + ")");
            // System.Diagnostics.Debug.WriteLine("intensity=" + sectorIntensity);
            //System.Diagnostics.Debug.WriteLine("StellarDen=" + STELLAR_DENSITY);
            //System.Diagnostics.Debug.WriteLine("avgDelta2=" + avgDelta);

            int i = 0;

            while (true)
            {
                int delta = r.Next(1, avgDelta + 1);

                i = i + delta;
                if (i >= sectorCubed)
                {
                    break;
                }


                double x = r.Next(-.5d, .5d);
                double y = r.Next(-.5d, .5d);
                double z = r.Next(-.5d, .5d);

                int index = i;
                x += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                y += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                z += index - HALF_SECTOR_SIZE;


                SolarSystem system = new SolarSystem(index,Coord,new Vector3(x + Coord.X * SECTOR_SIZE, y + Coord.Y * SECTOR_SIZE, z + Coord.Z * SECTOR_SIZE));
                Systems.Add(system);
              
            }
            return Systems;
        }

    }
}
