using System;
using System.Collections;
using System.Collections.Generic;

namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector ParentSector;
        public SystemCoord Coord;
        public Object ClientCoord;
        public float ClientDistance = float.MaxValue;
        public Star Star;
        public int Size;
        public List<Planet> Planets;
        public int Hash = 0;

        int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };



        public SolarSystem(SystemCoord coord, GalaxySector parentSector, int size, Random r)
        {
            Hash = Convert.ToInt32(coord.X + coord.Y * GalaxySector.SECTOR_SIZE + coord.Z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE);
            Hash = Hash ^ parentSector.Hash;
            Coord = coord;
            ParentSector = parentSector;
            Size = size;
            Star = new Star(this, r);
        }

        public void Generate()
        {

            Random r = new Random(Hash ^ ParentSector.Hash);

            List<int> availableOrbits = new List<int>();
            int numOrbits = 50;
            for (int i = 0; i < numOrbits; i++)
            {
                availableOrbits.Add(i);
            }

            int numPlanets = PlanetCountDistribution[r.Next(0, PlanetCountDistribution.Length)];

            Planets = new List<Planet>();
            for (int i = 0; i < numPlanets; i++)
            {
                int orbitIndex = r.Next(0, availableOrbits.Count);
                int orbit = availableOrbits[orbitIndex];
                availableOrbits.RemoveAt(orbitIndex);
                Planet p = new Planet(this, orbit,r);
                Planets.Add(p);

            }

        }

    }
}
