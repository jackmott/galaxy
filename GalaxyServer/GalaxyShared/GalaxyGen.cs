using System;

namespace GalaxyShared
{


    public class GalaxyGen
    {

        const ulong MAX_DISTANCE = 16777216;
        int inverseSystemDensity = 10; //inverse so we can use an int directly in rng
        

        public GalaxyGen()
        {
            
        }

        

        public void PopulateSector(GalaxySector sector)
        {
            int numStars = 12000; //todo lookup from image
            Random r = new Random(sector.coord.x + sector.coord.y + sector.coord.z);
            for (int i = 0; i < numStars; i++)
            {
                GalaxyCoord coord = new GalaxyCoord((ushort)r.Next(Int16.MaxValue*2), 
                                                    (ushort)r.Next(Int16.MaxValue*2),
                                                    (ushort)r.Next(Int16.MaxValue*2));

                SolarSystem system = new SolarSystem(coord,sector);                
                sector.systems.Add(system);
            }

        }

       

        


    }
}
