using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace GalaxyShared
{
    public class GalaxySector
    {
        public SectorCoord Coord;
        public System.Drawing.Color Color;
        public List<SolarSystem> Systems;
        public int DominantStarType;
        

        public const int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public const int SECTOR_SIZE = 25; //light years cubed
        public const int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;

        public int Hash = 0;

        public GalaxySector(SectorCoord coord, Bitmap hoag, int everyNth = 1)
        {
            this.Coord = coord;

            Hash = coord.X + coord.Y * SECTOR_SIZE + coord.Z * SECTOR_SIZE * SECTOR_SIZE;
            Systems = new List<SolarSystem>();

            
            System.Random r = new System.Random(Hash);

            int HALF_SECTOR_SIZE = SECTOR_SIZE / 2;
            //things to be looked up from data somehow
            float lightYearsPerPixel = GALAXY_SIZE_LIGHTYEARS / hoag.Width;

            int adjustedSectorX = coord.X + GALAXY_SIZE_SECTORS / 2;
            int adjustedSectorY = coord.Y + GALAXY_SIZE_SECTORS / 2;

            int pixelX = Convert.ToInt32(adjustedSectorX * SECTOR_SIZE / lightYearsPerPixel);
            int pixelY = Convert.ToInt32(adjustedSectorY * SECTOR_SIZE / lightYearsPerPixel);
            Color = hoag.GetPixel(pixelX, pixelY);
            float saturation = Color.GetSaturation();
            if (saturation < .4) DominantStarType = Star.F;
            else
            {
                DominantStarType = Star.hueToType[Convert.ToInt32(Color.GetHue() / 30)];
            }

            float sectorIntensity = Color.GetBrightness();
            if (sectorIntensity < .1) sectorIntensity = .01f;
            float STELLAR_DENSITY = .014f * sectorIntensity; //stars per cubic light year
            //if (STELLAR_DENSITY == 0) STELLAR_DENSITY = .00001f;
            int sectorCubed = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;  //how many lights years cubed are there?
            float avgDeltaF = 1f / STELLAR_DENSITY;
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

                
                float x = GalaxyGen.RandomRange(r, -.5f, .5f);
                float y = GalaxyGen.RandomRange(r, -.5f, .5f);
                float z = GalaxyGen.RandomRange(r, -.5f, .5f);

                int index = i;
                x += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                y += (index % SECTOR_SIZE) - HALF_SECTOR_SIZE;
                index = index / SECTOR_SIZE;
                z += index - HALF_SECTOR_SIZE;


                SolarSystem system = new SolarSystem(new Vector3(x, y, z), this, 1,i, r);
                Systems.Add(system);

            }
        }

    }
}
