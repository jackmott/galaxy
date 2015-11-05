using System;
using System.Diagnostics;
using System.Drawing;


namespace GalaxyShared
{


    public class GalaxyGen
    {
        
        Bitmap Hoag;
        

        public GalaxyGen()
        {
            loadGalaxyBitmap();                      

        }

        public void loadGalaxyBitmap()
        {
            try
            {
                Hoag = (Bitmap)Bitmap.FromFile("Assets/GalaxyShared/hoag-ring.bmp");
            }
            catch
            {
                Hoag = (Bitmap)Bitmap.FromFile("assets/hoag-ring.bmp");
            }

        }

        public static int RandomRange(Random rand, int min, int max)
        {
            return rand.Next(min, max);
        }

        public static float RandomRange(Random rand, float min, float max)
        {
                        
            return (float)rand.NextDouble() * (max - min) + min;
            
            
        }

        public static double RandomRange(Random rand,double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }


        public GalaxySector GetSector(SectorCoord coord, int everyNth)
        {
            try {
                return new GalaxySector(coord, Hoag, everyNth);
            } catch (Exception)
            {
                return null;
            }
        }



    }


}

