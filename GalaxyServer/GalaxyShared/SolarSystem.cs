using System;
using System.Collections;
using System.Collections.Generic;
using XnaGeometry;

namespace GalaxyShared
{
    
    public class SolarSystem
    {
        
        public SectorCoord ParentSectorCoord;

        public Vector3 Pos;                
        public Star Star;
        
        public List<Planet> Planets;
        public List<Asteroid> Asteroids;

        FastRandom rand;
                
        int[] PlanetCountDistribution = { 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        


        public SolarSystem(Vector3 pos)
        {
            rand = new FastRandom(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), Convert.ToInt32(pos.Z));
            Pos = pos;                    
            Star = new Star(this);
        }

        public void Generate()
        {
            
           

            List<int> availableOrbits = new List<int>();
            int numOrbits = 50;
            for (int i = 0; i < numOrbits; i++)
            {
                availableOrbits.Add(i);
            }
            
            int numPlanets = PlanetCountDistribution[rand.Next(0, PlanetCountDistribution.Length)];

            Planets = new List<Planet>();
            Asteroids = new List<Asteroid>();
            for (int i = 0; i < numPlanets; i++)
            {
                int orbitIndex = rand.Next(0, availableOrbits.Count); 
                int orbit = availableOrbits[orbitIndex];
                availableOrbits.RemoveAt(orbitIndex);
                Planet p = new Planet(this, orbit);
                Planets.Add(p);

            }

            foreach (int orbit in availableOrbits)
            {
                int asteroidChance = rand.Next(0, 100);
                if (asteroidChance == 0)
                {
                    int numAsteroids = rand.Next(20 * orbit, 100 * orbit);
                    for (int i = 0; i < numAsteroids;i++)
                    {
                        Asteroids.Add(new Asteroid(this, orbit));
                    }

                }
            }

        }

    }
}
