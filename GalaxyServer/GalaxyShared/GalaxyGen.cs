using System;

namespace GalaxyShared
{


    public class GalaxyGen
    {
        static int GALAXY_SIZE = 128000; //light years, cube
        public static int SECTOR_SIZE = 50; //light years cubed
        
        public enum StarType { White, Red, Blue, Yellow };

        static int MAX_STAR_SIZE = 10;
        
        public GalaxyGen()
        {
            
        }

       


        public void PopulateSector(GalaxySector sector)
        {
            
            Random r = new Random(sector.coord.x + sector.coord.y * SECTOR_SIZE + sector.coord.z * SECTOR_SIZE*SECTOR_SIZE);
            int min = (-SECTOR_SIZE / 2) +1 ;
            int max = (SECTOR_SIZE / 2);

            //things to be looked up from data somehow
            double STELLAR_DENSITY = .004; //stars per cubic light year
            int percentRed = 5;
            int percentYellow = 10;
            int percentBlue = 30;
            int percentWhite = 100;

            StarType[] starTypeLookup = new StarType[100];
            for (int i = 0; i < 100;i++)
            {
                if (i < percentRed) starTypeLookup[i] = StarType.Red;
                else if (i < percentYellow) starTypeLookup[i] = StarType.Yellow;
                else if (i < percentBlue) starTypeLookup[i] = StarType.Blue;
                else if (i < percentWhite) starTypeLookup[i] = StarType.White;
            }


            int numStars = Convert.ToInt32(SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE * STELLAR_DENSITY);

            for (int i = 0; i < numStars; i++)
            {
                GalaxyCoord coord = new GalaxyCoord(r.Next(min,max), 
                                                    r.Next(min,max),
                                                    r.Next(min,max));

                StarType type = starTypeLookup[r.Next(0, 100)];
                SolarSystem system = new SolarSystem(coord,sector,type,r.Next(1,MAX_STAR_SIZE+1));                
                sector.systems.Add(system);
            }

        }

       

        


    }
}
