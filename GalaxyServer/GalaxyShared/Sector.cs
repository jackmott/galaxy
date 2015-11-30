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

        private FastRandom r = new FastRandom(0);


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

        public SolarSystem GenerateSystem(int index)
        {
            r.Init(Coord.X, Coord.Y, Coord.Z, index);
            Vector3 starCoord = new Vector3(r.Next(-SECTOR_SIZE/2d,SECTOR_SIZE/2d) + Coord.X * SECTOR_SIZE, r.Next(-SECTOR_SIZE/2d, SECTOR_SIZE/2d) + Coord.Y * SECTOR_SIZE, r.Next(-SECTOR_SIZE/2d, SECTOR_SIZE/2d) + Coord.Z * SECTOR_SIZE);
            SolarSystem system = new SolarSystem(index, Coord, starCoord);
            Console.WriteLine(system.Pos);
            return system;
        }

        public List<SolarSystem> GenerateSystems(int everyNth)
        {
            int sectorCubed = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;  //how many lights years cubed are there?
            int starCount = (int)Math.Floor(StellarDensity * sectorCubed);
            Systems = new List<SolarSystem>(starCount);
            for (int i = 0; i < starCount; i=i+everyNth)
            {
                Systems.Add(GenerateSystem(i));
            }
            return Systems;
        }


    }
}
