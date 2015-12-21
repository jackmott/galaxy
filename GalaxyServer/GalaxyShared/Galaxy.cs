using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace GalaxyShared
{
    public static class Galaxy
    {
        public static Bitmap Hoag;
        public static GalaxyColor[][][] GalaxyVolume;
        public const int VOLUME_SCALE = 10;
        public const int GALAXY_THICKNESS = 200;

        public const int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public const int SECTOR_SIZE = 25; //light years cubed
        public const int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;
        public const double EXPAND_FACTOR = 1d / 2.5d; //multiplied by galaxy coordinates to get unity coordinates
        public const int HALF_SECTOR_SIZE = SECTOR_SIZE / 2;

        private static float LightyearsPerPixel;

        public static void Init()
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

                LightyearsPerPixel = GALAXY_SIZE_LIGHTYEARS / Hoag.Width;
            }
            SectorCoord s = new SectorCoord(-2560, 0, 41);
           Color c = GetColorAt(s);
            Console.WriteLine(c);
        }

        public static Color GetColorAt(SectorCoord coord)
        {
             

            int adjustedSectorX = coord.X + GALAXY_SIZE_SECTORS / 2;
            int adjustedSectorY = coord.Y + GALAXY_SIZE_SECTORS / 2;
            

            int pixelX = Convert.ToInt32(adjustedSectorX * SECTOR_SIZE / LightyearsPerPixel)-1;
            int pixelY = Convert.ToInt32(adjustedSectorY * SECTOR_SIZE / LightyearsPerPixel)-1;
            pixelX = Math.Max(0, pixelX);
            pixelY = Math.Max(0, pixelY);
            

            Color c1 = Hoag.GetPixel(pixelX,pixelY);

            int z = Math.Abs(coord.Z);
            float zFactor = Math.Max(0, (400.0f - z) / 400.0f);

            Color result = Color.FromArgb(Convert.ToInt32(c1.R * zFactor), Convert.ToInt32(c1.G * zFactor),Convert.ToInt32( c1.B * zFactor));
            return result;

        }


    }
}
