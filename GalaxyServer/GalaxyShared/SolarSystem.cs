using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyShared
{
    public class SolarSystem
    {
        public GalaxySector ParentSector;
        public Vector3 Coord;
        public Vector3 ClientCoord;
        public float ClientDistance = float.MaxValue;
        public Star Star;
        public int Size;
        public List<Planet> Planets;
        public List<Asteroid> Asteroids;
        public int Hash = 0;
        public static GameObject go;
        int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };



        public SolarSystem(Vector3 coord, GalaxySector parentSector, int size, System.Random r)
        {
            Hash = Convert.ToInt32(coord.x + coord.y * GalaxySector.SECTOR_SIZE + coord.z * GalaxySector.SECTOR_SIZE * GalaxySector.SECTOR_SIZE);
            Hash = Hash ^ parentSector.Hash;
            Coord = coord;
            ParentSector = parentSector;
            Size = size;
            Star = new Star(this, r);
        }

        public void Generate()
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            System.Random r = new System.Random(Hash ^ ParentSector.Hash);

            List<int> availableOrbits = new List<int>();
            int numOrbits = 50;
            for (int i = 0; i < numOrbits; i++)
            {
                availableOrbits.Add(i);
            }
            
            int numPlanets = PlanetCountDistribution[GalaxyGen.RandomRange(r,0, PlanetCountDistribution.Length)];

            Planets = new List<Planet>();
            Asteroids = new List<Asteroid>();
            for (int i = 0; i < numPlanets; i++)
            {
                int orbitIndex = GalaxyGen.RandomRange(r,0, availableOrbits.Count); 
                int orbit = availableOrbits[orbitIndex];
                availableOrbits.RemoveAt(orbitIndex);
                Planet p = new Planet(this, orbit,r);
                Planets.Add(p);

            }

            foreach (int orbit in availableOrbits)
            {
                int asteroidChance = GalaxyGen.RandomRange(r, 0, 100);
                if (asteroidChance == 0)
                {
                    int numAsteroids = GalaxyGen.RandomRange(r,20 * orbit, 100 * orbit);
                    for (int i = 0; i < numAsteroids;i++)
                    {
                        Asteroids.Add(new Asteroid(this, orbit, r));
                    }

                }
            }

        }

    }
}
