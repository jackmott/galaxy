using System;
using System.Collections.Generic;
using System.Drawing;
using XnaGeometry;


namespace GalaxyShared
{
    public class Sector
    {
      
       
        public SectorCoord Coord;
        public Color Color;
        public List<SolarSystem> Systems;
        public int DominantStarType;
        public float StellarDensity;
        

       

        private FastRandom r = new FastRandom(0);


        public Sector(SectorCoord coord)
        {
           
            this.Coord = coord;            
                                            
            //things to be looked up from data somehow           
            Color = Galaxy.GetColorAt(coord);
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
            Vector3 starCoord = new Vector3(r.Next(-Galaxy.SECTOR_SIZE /2d, Galaxy.SECTOR_SIZE /2d) + Coord.X * Galaxy.SECTOR_SIZE, r.Next(-Galaxy.SECTOR_SIZE /2d, Galaxy.SECTOR_SIZE /2d) + Coord.Y * Galaxy.SECTOR_SIZE, r.Next(-Galaxy.SECTOR_SIZE /2d, Galaxy.SECTOR_SIZE /2d) + Coord.Z * Galaxy.SECTOR_SIZE);
            SolarSystem system = new SolarSystem(index, this, starCoord,r);            
            return system;
        }

        public List<SolarSystem> GenerateSystems(int everyNth)
        {
            int sectorCubed = Galaxy.SECTOR_SIZE * Galaxy.SECTOR_SIZE * Galaxy.SECTOR_SIZE;  //how many lights years cubed are there?
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
