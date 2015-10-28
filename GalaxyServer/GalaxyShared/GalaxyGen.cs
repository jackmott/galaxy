using System;
using System.Diagnostics;
using System.Drawing;


namespace GalaxyShared
{


    public class GalaxyGen
    {
        public static int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public static int SECTOR_SIZE = 50; //light years cubed
        public static int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;
        
        public enum StarType { White, Red, Blue, Yellow };

        

        Bitmap hoag;
        
        public GalaxyGen()
        {
            loadGalaxyBitmap();

         //   GenStars(0, 0, 0, 1);
        }

        public void loadGalaxyBitmap()
        {
            try
            {
                hoag = (Bitmap)Bitmap.FromFile("Assets/GalaxyShared/hoag-ring.bmp");
            }
            catch
            {
                hoag = (Bitmap)Bitmap.FromFile("assets/hoag-ring.bmp");
            }

            /*
            int width = hoag.Width;
            int height = hoag.Height;

            ImageConverter converter = new ImageConverter();
            byte[] b = (byte[])converter.ConvertTo(hoag, typeof(byte[]));
            for (int i = 0; i < b.Length; i++)
            {
                Console.WriteLine(b[i] + ",");
            }*/

           


        }


        public void GenStars(int x, int y, int z, int everyNth)
        {

           

            
            GalaxySector sector = new GalaxySector(new SectorCoord(x, y, z));
            PopulateSector(sector, everyNth);


        

            int count = 0;
            foreach (SolarSystem system in sector.systems)
            {

                count++;
            }
        //    Debug.WriteLine("System starcount:" + count);


        }


        public void PopulateSector(GalaxySector sector, int everyNth)
        {
            
            Random r = new Random(sector.coord.x + sector.coord.y * SECTOR_SIZE + sector.coord.z * SECTOR_SIZE*SECTOR_SIZE);
            
            int HALF_SECTOR_SIZE = SECTOR_SIZE/2;
            //things to be looked up from data somehow
            float lightYearsPerPixel = GALAXY_SIZE_LIGHTYEARS / hoag.Width;

            int adjustedSectorX = sector.coord.x + GALAXY_SIZE_SECTORS / 2;
            int adjustedSectorY = sector.coord.y + GALAXY_SIZE_SECTORS / 2;

            int pixelX = Convert.ToInt32(adjustedSectorX * SECTOR_SIZE/ lightYearsPerPixel);
            int pixelY = Convert.ToInt32(adjustedSectorY * SECTOR_SIZE / lightYearsPerPixel);

            Color sectorColor = hoag.GetPixel(pixelX, pixelY);
            float sectorIntensity = sectorColor.GetBrightness();
            if (sectorIntensity < .1) sectorIntensity = .01f;
            float STELLAR_DENSITY = .014f * sectorIntensity; //stars per cubic light year
            //if (STELLAR_DENSITY == 0) STELLAR_DENSITY = .00001f;
            int sectorCubed = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;  //how many lights years cubed are there?
            int avgDelta = (int)(1 / STELLAR_DENSITY);
            avgDelta *= 2; //for average
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

                int index = i;
                int x = (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                int y = (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                int z = index - HALF_SECTOR_SIZE;

                                
                SolarSystem system = new SolarSystem(new SystemCoord(x, y, z), sector, sectorColor, 1);
                sector.systems.Add(system);                

            }
                       
            

        }

       

        


    }
}
