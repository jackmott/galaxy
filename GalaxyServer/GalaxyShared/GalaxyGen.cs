using System;

namespace GalaxyShared
{


    public class GalaxyGen
    {
        static int GALAXY_SIZE = 128000; //light years, cube
        public static int SECTOR_SIZE = 100; //light years cubed
        static double STELLAR_DENSITY = .004; //stars per cubic light year

        public GalaxyGen()
        {
            
        }

       


        public void PopulateSector(GalaxySector sector)
        {
            
            Random r = new Random(sector.coord.x + sector.coord.y * SECTOR_SIZE + sector.coord.z * SECTOR_SIZE*SECTOR_SIZE);

            int min = (-SECTOR_SIZE / 2) +1 ;
            int max = (SECTOR_SIZE / 2);

            int numStars = Convert.ToInt32(SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE * STELLAR_DENSITY);

            for (int i = 0; i < numStars; i++)
            {
                GalaxyCoord coord = new GalaxyCoord(r.Next(min,max), 
                                                    r.Next(min,max),
                                                    r.Next(min,max));

                SolarSystem system = new SolarSystem(coord,sector);                
                sector.systems.Add(system);
            }

        }

       

        


    }
}
